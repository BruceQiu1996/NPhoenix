using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using HandyControl.Data;
using LeagueOfLegendsBoxer.Application.Account;
using LeagueOfLegendsBoxer.Models;
using LeagueOfLegendsBoxer.Resources;
using LeagueOfLegendsBoxer.ViewModels.Pages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using LeagueOfLegendsBoxer.Application.Teamup;
using LeagueOfLegendsBoxer.Application.Teamup.Dtos;
using LeagueOfLegendsBoxer.Application.Game;

namespace LeagueOfLegendsBoxer.ViewModels
{
    public class BlackListViewModel : ObservableObject
    {
        private ObservableCollection<Tuple<ParticipantIdentity, Participant>> _currentParticipants = new ObservableCollection<Tuple<ParticipantIdentity, Participant>>();
        public ObservableCollection<Tuple<ParticipantIdentity, Participant>> CurrentParticipants
        {
            get => _currentParticipants;
            set => SetProperty(ref _currentParticipants, value);
        }

        private ObservableCollection<Tuple<ParticipantIdentity, Participant>> _leftParticipants = new ObservableCollection<Tuple<ParticipantIdentity, Participant>>();
        public ObservableCollection<Tuple<ParticipantIdentity, Participant>> LeftParticipants
        {
            get => _leftParticipants;
            set => SetProperty(ref _leftParticipants, value);
        }

        private ObservableCollection<Tuple<ParticipantIdentity, Participant>> _rightParticipants = new ObservableCollection<Tuple<ParticipantIdentity, Participant>>();
        public ObservableCollection<Tuple<ParticipantIdentity, Participant>> RightParticipants
        {
            get => _rightParticipants;
            set => SetProperty(ref _rightParticipants, value);
        }

        public AsyncRelayCommand<Tuple<ParticipantIdentity, Participant>> SubmitBlackListCommanmdAsync { get; set; }
        public RelayCommand<Tuple<ParticipantIdentity, Participant>> ToggleBlackInfoCommand { get; set; }
        public AsyncRelayCommand<Tuple<ParticipantIdentity, Participant>> SearchRecordCommand { get; set; }
        public RelayCommand SwitchTeamCommand { get; set; }

        private readonly IniSettingsModel _iniSettingsModel;
        private readonly ILogger<BlackListViewModel> _logger;
        private readonly IAccountService _accountService;
        private readonly IGameService _gameService;
        private readonly ITeamupService _teamupService;

        public BlackListViewModel(IniSettingsModel iniSettingsModel,
                                  IGameService gameService,
                                  ILogger<BlackListViewModel> logger, 
                                  IAccountService accountService, 
                                  ITeamupService teamupService)
        {
            _iniSettingsModel = iniSettingsModel;
            _logger = logger;
            _gameService = gameService;
            _teamupService = teamupService;
            SubmitBlackListCommanmdAsync = new AsyncRelayCommand<Tuple<ParticipantIdentity, Participant>>(SubmitBlackListAsync);
            ToggleBlackInfoCommand = new RelayCommand<Tuple<ParticipantIdentity, Participant>>(ToggleBlackInfo);
            SearchRecordCommand = new AsyncRelayCommand<Tuple<ParticipantIdentity, Participant>>(SearchRecord);
            SwitchTeamCommand = new RelayCommand(SwitchTeam);
            _accountService = accountService;
        }

        public async Task LoadAccount(Record record)
        {
            IList<Tuple<ParticipantIdentity, Participant>> members = new List<Tuple<ParticipantIdentity, Participant>>();
            foreach (var index in Enumerable.Range(0, record.Participants.Count()))
            {
                var lidentity = record.ParticipantIdentities[index];
                lidentity.IsCurrentUser = Constant.Account.SummonerId == lidentity.Player.SummonerId;
                members.Add(new Tuple<ParticipantIdentity, Participant>(lidentity, record.Participants[index]));
            }

            _leftParticipants = new ObservableCollection<Tuple<ParticipantIdentity, Participant>>(members.Where(x => x.Item2.TeamId == 100));
            _rightParticipants = new ObservableCollection<Tuple<ParticipantIdentity, Participant>>(members.Where(x => x.Item2.TeamId == 200));
            var my = _leftParticipants.FirstOrDefault(x => x.Item1.Player.SummonerId == Constant.Account.SummonerId) != null
                   ? _leftParticipants : _rightParticipants;
            CurrentParticipants = my;
            var other = _leftParticipants.FirstOrDefault(x => x.Item1.Player.SummonerId == Constant.Account.SummonerId) != null
                ? _rightParticipants : _leftParticipants;

            var team1GoldEarned = _leftParticipants.Sum(x => x.Item2.Stats.GoldEarned);
            var team2GoldEarned = _rightParticipants.Sum(x => x.Item2.Stats.GoldEarned);

            var team1TotalDamage = _leftParticipants.Sum(x => x.Item2.Stats.TotalDamageDealtToChampions);
            var team2TotalDamage = _rightParticipants.Sum(x => x.Item2.Stats.TotalDamageDealtToChampions);

            foreach (var item in _leftParticipants)
            {
                if (team1TotalDamage == 0) item.Item2.Stats.DamageConvert = "NaN%";
                else item.Item2.Stats.DamageConvert = ((item.Item2.Stats.TotalDamageDealtToChampions * 1.0 / team1TotalDamage) / (item.Item2.Stats.GoldEarned * 1.0 / team1GoldEarned) * 100).ToString("0.") + "%";
            }
            foreach (var item in _rightParticipants)
            {
                if (team2TotalDamage == 0) item.Item2.Stats.DamageConvert = "NaN%";
                else item.Item2.Stats.DamageConvert = ((item.Item2.Stats.TotalDamageDealtToChampions * 1.0 / team2TotalDamage) / (item.Item2.Stats.GoldEarned * 1.0 / team2GoldEarned) * 100).ToString("0.") + "%";
            }
            if (record.QueueId == 420 || record.QueueId == 430 || record.QueueId == 440 || record.QueueId == 450)
            {
                bool myIsWin = my.FirstOrDefault().Item2.Stats.Win;
                Tuple<ParticipantIdentity, Participant> mvp = null;
                Tuple<ParticipantIdentity, Participant> svp = null;
                if (myIsWin)
                {
                    mvp = my.OrderByDescending(x => x.Item2.GetScore()).FirstOrDefault();
                    mvp.Item1.IsMvp = true;
                    svp = other.OrderByDescending(x => x.Item2.GetScore()).FirstOrDefault();
                    svp.Item1.IsSvp = true;
                }
                else
                {
                    mvp = other.OrderByDescending(x => x.Item2.GetScore()).FirstOrDefault();
                    mvp.Item1.IsMvp = true;
                    svp = my.OrderByDescending(x => x.Item2.GetScore()).FirstOrDefault();
                    svp.Item1.IsSvp = true;
                }

                var me = _leftParticipants.Concat(_rightParticipants).FirstOrDefault(x => x.Item1.Player.SummonerId == Constant.Account.SummonerId);

                if (me != null && Constant.ConnectTeamupSuccessful)
                {
                    var isLeast = _leftParticipants.Concat(_rightParticipants).OrderBy(x => x.Item2.GetScore()).FirstOrDefault() == me;
                    //上传战绩
                    await _teamupService.UploadRecordAsync(new CreateGameRecordByClientDto()
                    {
                        UserId = Constant.Account.SummonerId,
                        GameId = record.GameId,
                        GameMode = record.QueueId,
                        Kill = me.Item2.Stats.Kills,
                        Death = me.Item2.Stats.Deaths,
                        Assit = me.Item2.Stats.Assists,
                        IsMvp = mvp.Item1.Player.SummonerId == me.Item1.Player.SummonerId,
                        IsSvp = svp.Item1.Player.SummonerId == me.Item1.Player.SummonerId,
                        IsLeastScore = isLeast
                    });
                }
            }
        }

        private async Task SubmitBlackListAsync(Tuple<ParticipantIdentity, Participant> account)
        {
            try
            {
                var blackAccount = new BlackAccount()
                {
                    Id = account.Item1.Player.SummonerId,
                    AccountName = account.Item1.Player.SummonerName,
                    CreateTime = DateTime.Now,
                    Reason = account.Item1.Reason,
                };
                await _iniSettingsModel.WriteBlackAccountAsync(blackAccount);
                Growl.SuccessGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "拉黑成功",
                    ShowDateTime = false
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Growl.WarningGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "拉黑失败",
                    ShowDateTime = false
                });

            }
        }

        private void ToggleBlackInfo(Tuple<ParticipantIdentity, Participant> item)
        {
            item.Item1.IsOpenBlack = !item.Item1.IsOpenBlack;
        }

        private async Task SearchRecord(Tuple<ParticipantIdentity, Participant> item)
        {
            var infromation = await _accountService.GetSummonerInformationAsync(item.Item1.Player.SummonerId);
            var account = JsonConvert.DeserializeObject<Account>(infromation);
            var rankData = JToken.Parse(await _accountService.GetSummonerRankInformationAsync(account.Puuid));
            account.Rank = rankData["queueMap"].ToObject<Rank>();
            var recordsData = JToken.Parse(await _gameService.GetRecordsByPage(id: account.Puuid));
            account.Records = new ObservableCollection<Record>(recordsData["games"]["games"].ToObject<IEnumerable<Record>>().OrderByDescending(x => x.GameCreation));
            App.ServiceProvider.GetRequiredService<TeammateViewModel>().ShowRecord(account);
        }

        private void SwitchTeam()
        {
            CurrentParticipants = CurrentParticipants == _leftParticipants ? _rightParticipants : _leftParticipants;
        }
    }
}
