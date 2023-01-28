using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using HandyControl.Data;
using LeagueOfLegendsBoxer.Application.Game;
using LeagueOfLegendsBoxer.Models;
using LeagueOfLegendsBoxer.Windows;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace LeagueOfLegendsBoxer.ViewModels.Pages
{
    public class SummonerDetailViewModel : ObservableObject
    {
        private bool _loaded = false;
        private Account _account;
        public Account Account
        {
            get => _account;
            set => SetProperty(ref _account, value);
        }

        public SummonerAnalyseViewModel SummonerAnalyseViewModel { get; set; }

        private Record _record;
        public Record Record
        {
            get => _record;
            set => SetProperty(ref _record, value);
        }

        private ObservableCollection<Champ> _champs;
        public ObservableCollection<Champ> Champs
        {
            get => _champs;
            set => SetProperty(ref _champs, value);
        }

        private ObservableCollection<Record> _records;
        public ObservableCollection<Record> Records
        {
            get => _records;
            set => SetProperty(ref _records, value);
        }

        public int _pageIndex;
        public int PageIndex
        {
            get => _pageIndex;
            set => SetProperty(ref _pageIndex, value);
        }

        public AsyncRelayCommand LoadCommandAsync { get; set; }
        public AsyncRelayCommand GameSelectionChangedCommandAsync { get; set; }
        public AsyncRelayCommand<long> FetchPlayerDetailCommandAsync { get; set; }
        public AsyncRelayCommand<FunctionEventArgs<int>> SelectPageCommandAsync { get; set; }
        public RelayCommand<Tuple<ParticipantIdentity, Participant>> CopyNameCommand { get; set; }
        public RelayCommand<Tuple<ParticipantIdentity, Participant>> BlackAccountCommand { get; set; }

        private readonly IGameService _gameService;
        public SummonerDetailViewModel(IGameService gameService)
        {
            _gameService = gameService;
            LoadCommandAsync = new AsyncRelayCommand(LoadAsync);
            FetchPlayerDetailCommandAsync = new AsyncRelayCommand<long>(FetchPlayerDetailAsync);
            GameSelectionChangedCommandAsync = new AsyncRelayCommand(GameSelectionChangedAsync);
            SelectPageCommandAsync = new AsyncRelayCommand<FunctionEventArgs<int>>(SelectPageAsync);
            CopyNameCommand = new RelayCommand<Tuple<ParticipantIdentity, Participant>>(CopyName);
            BlackAccountCommand = new RelayCommand<Tuple<ParticipantIdentity, Participant>>(BlackAccountMethod);
        }

        private async Task SelectPageAsync(FunctionEventArgs<int> e)
        {
            var records = await _gameService.GetRecordsByPage((e.Info - 1) * 10, e.Info * 10, Account.Puuid);
            var recordsData = JToken.Parse(records);
            var recordObjs = new ObservableCollection<Record>(recordsData["games"]["games"].ToObject<ObservableCollection<Record>>().Reverse());
            Records = recordObjs;
            await LoadRecordDetailDataGroup(Records);
        }

        public async Task LoadAsync()
        {
            if (_loaded) return;
            try
            {
                var champData = await _gameService.QuerySummonerSuperChampDataAsync(Account.SummonerId);
                var list = JsonSerializer.Deserialize<ObservableCollection<Champ>>(champData);
                int rank = 0;
                foreach (var champ in list)
                {
                    champ.Rank = ++rank;
                }
                Champs = list;
                Records = Account.Records.Count() >= 10 ? new ObservableCollection<Record>(Account.Records.Take(10)) : Account.Records;
                await LoadRecordDetailDataGroup(Records);
                PageIndex = 1;
                _loaded = true;
            }
            catch (Exception ex)
            {
                Growl.InfoGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "拉取战绩失败!" + ex.Message,
                    ShowDateTime = false
                });
            }
        }

        private async Task LoadRecordDetailDataGroup(ObservableCollection<Record> recordObjs)
        {
            Record = recordObjs.FirstOrDefault();
            foreach (var record in recordObjs)
            {
                var details = await _gameService.QueryGameDetailAsync(record.GameId);
                var detailRecordsData = JToken.Parse(details);
                record.DetailRecord = detailRecordsData.ToObject<Record>();
                IList<Tuple<ParticipantIdentity, Participant>> members = new List<Tuple<ParticipantIdentity, Participant>>();
                foreach (var index in Enumerable.Range(0, record.DetailRecord.ParticipantIdentities.Count()))
                {
                    var lidentity = record.DetailRecord.ParticipantIdentities[index];
                    lidentity.IsCurrentUser = Account.SummonerId == lidentity.Player.SummonerId;
                    members.Add(new Tuple<ParticipantIdentity, Participant>(lidentity, record.DetailRecord.Participants[index]));
                }

                record.LeftParticipants = new ObservableCollection<Tuple<ParticipantIdentity, Participant>>(members.Where(x => x.Item2.TeamId == 100));
                record.RightParticipants = new ObservableCollection<Tuple<ParticipantIdentity, Participant>>(members.Where(x => x.Item2.TeamId == 200));

                record.DetailRecord.Team1GoldEarned = record.LeftParticipants.Sum(x => x.Item2.Stats.GoldEarned);
                record.DetailRecord.Team2GoldEarned = record.RightParticipants.Sum(x => x.Item2.Stats.GoldEarned);
                record.DetailRecord.Team1Kills = record.LeftParticipants.Sum(x => x.Item2.Stats.Kills);
                record.DetailRecord.Team2Kills = record.RightParticipants.Sum(x => x.Item2.Stats.Kills);

                var team1TotalDamage = record.LeftParticipants.Sum(x => x.Item2.Stats.TotalDamageDealtToChampions);
                var team2TotalDamage = record.RightParticipants.Sum(x => x.Item2.Stats.TotalDamageDealtToChampions);

                foreach (var item in record.LeftParticipants)
                {
                    if (team1TotalDamage == 0) item.Item2.Stats.DamageConvert = "NaN%";
                    else item.Item2.Stats.DamageConvert = ((item.Item2.Stats.TotalDamageDealtToChampions * 1.0 / team1TotalDamage) / (item.Item2.Stats.GoldEarned * 1.0 / record.DetailRecord.Team1GoldEarned) * 100).ToString("0.00") + "%";
                }
                foreach (var item in record.RightParticipants)
                {
                    if (team2TotalDamage == 0) item.Item2.Stats.DamageConvert = "NaN%";
                    else item.Item2.Stats.DamageConvert = ((item.Item2.Stats.TotalDamageDealtToChampions * 1.0 / team2TotalDamage) / (item.Item2.Stats.GoldEarned * 1.0 / record.DetailRecord.Team2GoldEarned) * 100).ToString("0.00") + "%";
                }
                var maxdmg = record.LeftParticipants.Concat(record.RightParticipants).Max(x => x.Item2.Stats.TotalDamageDealtToChampions);
                foreach (var item in record.LeftParticipants.Concat(record.RightParticipants))
                {
                    item.Item2.Stats.BarWidth = 350 * (item.Item2.Stats.TotalDamageDealtToChampions * 1.0 / maxdmg);
                }

                if (record.QueueId == 420 || record.QueueId == 430 || record.QueueId == 440 || record.QueueId == 450)
                {
                    var my = record.LeftParticipants.FirstOrDefault(x => x.Item1.Player.SummonerId == Account.SummonerId) != null
                        ? record.LeftParticipants : record.RightParticipants;
                    var other = record.LeftParticipants.FirstOrDefault(x => x.Item1.Player.SummonerId == Account.SummonerId) != null
                        ? record.RightParticipants : record.LeftParticipants;

                    bool myIsWin = record.Participants.FirstOrDefault().Stats.Win;
                    Tuple<ParticipantIdentity, Participant> mvp = null;
                    Tuple<ParticipantIdentity, Participant> svp = null;
                    if (myIsWin)
                    {
                        mvp = my.OrderByDescending(x => x.Item2.GetScore()).FirstOrDefault();
                        mvp.Item1.IsMvp = true;
                        svp = other.OrderByDescending(x => x.Item2.GetScore()).FirstOrDefault();
                        svp.Item1.IsSvp = true;
                        record.IsLeftWin = my == record.LeftParticipants ? true : false;
                    }
                    else
                    {
                        mvp = other.OrderByDescending(x => x.Item2.GetScore()).FirstOrDefault();
                        mvp.Item1.IsMvp = true;
                        svp = my.OrderByDescending(x => x.Item2.GetScore()).FirstOrDefault();
                        svp.Item1.IsSvp = true;
                        record.IsLeftWin = my == record.LeftParticipants ? false : true;
                    }

                    if (mvp.Item1.Player.SummonerId == Account.SummonerId)
                    {
                        record.IsMvp = true;
                    }
                    if (svp.Item1.Player.SummonerId == Account.SummonerId)
                    {
                        record.IsSvp = true;
                    }
                }
            }
        }

        public async Task GameSelectionChangedAsync()
        {

        }

        public async Task FetchPlayerDetailAsync(long id)
        {
            await SummonerAnalyseViewModel.LoadPageAsync(id);
        }

        private void CopyName(Tuple<ParticipantIdentity, Participant> tuple)
        {
            if (tuple.Item1.Player == null)
                return;

            Clipboard.SetDataObject(tuple.Item1.Player?.SummonerName);
        }

        private void BlackAccountMethod(Tuple<ParticipantIdentity, Participant> tuple)
        {
            if (tuple.Item1.Player == null)
                return;

            var _window = App.ServiceProvider.GetRequiredService<BlackTip>();
            (_window.DataContext as BlackTipViewModel).Load(tuple.Item1.Player.SummonerId, tuple.Item1.Player.SummonerName);
            _window.ShowDialog();
        }
    }
}
