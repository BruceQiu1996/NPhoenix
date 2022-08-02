using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using HandyControl.Data;
using LeagueOfLegendsBoxer.Application.Game;
using LeagueOfLegendsBoxer.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

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

        private Record _detailRecord;
        public Record DetailRecord
        {
            get => _detailRecord;
            set => SetProperty(ref _detailRecord, value);
        }

        private ObservableCollection<Tuple<ParticipantIdentity,Participant>> _leftParticipants;
        public ObservableCollection<Tuple<ParticipantIdentity,Participant>> LeftParticipants
        {
            get => _leftParticipants;
            set => SetProperty(ref _leftParticipants, value);
        }

        private ObservableCollection<Tuple<ParticipantIdentity, Participant>> _rightParticipants;
        public ObservableCollection<Tuple<ParticipantIdentity, Participant>> RightParticipants
        {
            get => _rightParticipants;
            set => SetProperty(ref _rightParticipants, value);
        }

        private ObservableCollection<Champ> _champs;
        public ObservableCollection<Champ> Champs
        {
            get => _champs;
            set => SetProperty(ref _champs, value);
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

        private readonly IGameService _gameService;
        public SummonerDetailViewModel(IGameService gameService)
        {
            _gameService = gameService;
            LoadCommandAsync = new AsyncRelayCommand(LoadAsync);
            FetchPlayerDetailCommandAsync = new AsyncRelayCommand<long>(FetchPlayerDetailAsync);
            GameSelectionChangedCommandAsync = new AsyncRelayCommand(GameSelectionChangedAsync);
            SelectPageCommandAsync = new AsyncRelayCommand<FunctionEventArgs<int>>(SelectPageAsync);
            LeftParticipants = new ObservableCollection<Tuple<ParticipantIdentity, Participant>>();
            RightParticipants = new ObservableCollection<Tuple<ParticipantIdentity, Participant>>();
        }

        private async Task SelectPageAsync(FunctionEventArgs<int> e)
        {
            var records = await _gameService.GetRecordsByPage((e.Info - 1) * 20, e.Info * 20, Account.Puuid);
            var recordsData = JToken.Parse(records);
            var recordObjs = new ObservableCollection<Record>(recordsData["games"]["games"].ToObject<ObservableCollection<Record>>().Reverse());
            Record = recordObjs.FirstOrDefault();
            Account.Records = recordObjs;
        }

        public async Task LoadAsync()
        {
            if (_loaded) return;
            try
            {
                var list = JsonSerializer.Deserialize<ObservableCollection<Champ>>(await _gameService.QuerySummonerSuperChampDataAsync(Account.SummonerId));
                int rank = 0;
                foreach (var champ in list)
                {
                    champ.Rank = ++rank;
                }
                Champs = list;
                Record = Account.Records.FirstOrDefault();
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

        public async Task GameSelectionChangedAsync()
        {
            var details = await _gameService.QueryGameDetailAsync(Record.GameId);
            var recordsData = JToken.Parse(details);
            DetailRecord = recordsData.ToObject<Record>();
            LeftParticipants.Clear();
            RightParticipants.Clear();
            foreach (var index in Enumerable.Range(0, 5)) 
            {
                DetailRecord.Team1GoldEarned += DetailRecord.Participants[index].Stats.GoldEarned;
                DetailRecord.Team2GoldEarned += DetailRecord.Participants[index + 5].Stats.GoldEarned;
                DetailRecord.Team1Kills += DetailRecord.Participants[index].Stats.Kills;
                DetailRecord.Team2Kills += DetailRecord.Participants[index + 5].Stats.Kills;
                var lidentity = DetailRecord.ParticipantIdentities[index];
                lidentity.IsCurrentUser = Account.SummonerId == lidentity.Player.SummonerId;
                var ridentity = DetailRecord.ParticipantIdentities[index + 5];
                ridentity.IsCurrentUser = Account.SummonerId == ridentity.Player.SummonerId;
                LeftParticipants.Add(new Tuple<ParticipantIdentity, Participant>(lidentity, DetailRecord.Participants[index]));
                RightParticipants.Add(new Tuple<ParticipantIdentity, Participant>(ridentity, DetailRecord.Participants[index + 5]));
            }
        }

        public async Task FetchPlayerDetailAsync(long id) 
        {
            await SummonerAnalyseViewModel.LoadPageAsync(id);
        }
    }
}
