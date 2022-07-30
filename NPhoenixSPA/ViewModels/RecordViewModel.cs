using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using HandyControl.Data;
using LeagueOfLegendsBoxer.Application.Account;
using LeagueOfLegendsBoxer.Application.Game;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPhoenixSPA.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Account = NPhoenixSPA.Models.Account;
using Record = NPhoenixSPA.Models.Record;

namespace NPhoenixSPA.ViewModels
{
    public class RecordViewModel : ObservableObject
    {
        private bool _loaded = false;

        private Account _account;
        public Account Account
        {
            get => _account;
            set => SetProperty(ref _account, value);
        }

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

        private string _summonerName;
        public string SummonerName
        {
            get => _summonerName;
            set => SetProperty(ref _summonerName, value);
        }

        private ObservableCollection<Tuple<ParticipantIdentity, Participant>> _leftParticipants;
        public ObservableCollection<Tuple<ParticipantIdentity, Participant>> LeftParticipants
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
        public AsyncRelayCommand SearchAccountCommandAsync { get; set; }
        public AsyncRelayCommand GameSelectionChangedCommandAsync { get; set; }
        public AsyncRelayCommand<long> FetchPlayerDetailCommandAsync { get; set; }
        public AsyncRelayCommand<FunctionEventArgs<int>> SelectPageCommandAsync { get; set; }

        private readonly IGameService _gameService;
        private readonly IAccountService _accountService;
        private readonly ILogger<RecordViewModel> _logger;

        public RecordViewModel(IGameService gameService, 
                               ILogger<RecordViewModel> logger, 
                               IAccountService accountService)
        {
            LoadCommandAsync = new AsyncRelayCommand(LoadAsync);
            SearchAccountCommandAsync = new AsyncRelayCommand(SearchAccountAsync);
            GameSelectionChangedCommandAsync = new AsyncRelayCommand(GameSelectionChangedAsync);
            SelectPageCommandAsync = new AsyncRelayCommand<FunctionEventArgs<int>>(SelectPageAsync);
            FetchPlayerDetailCommandAsync = new AsyncRelayCommand<long>(FetchPlayerDetailAsync);
            LeftParticipants = new ObservableCollection<Tuple<ParticipantIdentity, Participant>>();
            RightParticipants = new ObservableCollection<Tuple<ParticipantIdentity, Participant>>();
            _gameService = gameService;
            _accountService = accountService;
            _logger = logger;
        }

        public async Task LoadAsync()
        {
            if (Account == null)
            {
                _loaded = true;
                return;
            }

            if (_loaded) return;
            try
            {
                SummonerName = Account.DisplayName;
                var list = JsonConvert.DeserializeObject<ObservableCollection<Champ>>(
                                            await _gameService.QuerySummonerSuperChampDataAsync(Account.SummonerId));
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
                _logger.LogError(ex.ToString());
                Growl.InfoGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "拉取战绩失败!" + ex.Message,
                    ShowDateTime = false
                });
            }
        }

        public async Task SearchAccountAsync()
        {
            if (string.IsNullOrEmpty(SummonerName) || Account?.DisplayName == SummonerName)
            {
                return;
            }

            try
            {
                var data = await _accountService.GetSummonerInformationAsync(SummonerName);
                if (data == null)
                {
                    Growl.InfoGlobal(new GrowlInfo()
                    {
                        WaitTime = 2,
                        Message = "查无此人",
                        ShowDateTime = false
                    });
                }
                else 
                {
                    await UserProfileToAccount(data);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Growl.InfoGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "查询账号信息失败!" + ex.Message,
                    ShowDateTime = false
                });
            }
        }

        private async Task UserProfileToAccount(string profile) 
        {
            if (string.IsNullOrEmpty(profile))
                throw new ArgumentNullException("用户信息为空");

            Account = JsonConvert.DeserializeObject<Account>(profile);
            var rankData = JToken.Parse(await _accountService.GetSummonerRankInformationAsync(Account.Puuid));
            var list = JsonConvert.DeserializeObject<ObservableCollection<Champ>>(
                                    await _gameService.QuerySummonerSuperChampDataAsync(Account.SummonerId));
            int rank = 0;
            foreach (var champ in list)
            {
                champ.Rank = ++rank;
            }
            Champs = list;
            Account.Rank = rankData["queueMap"].ToObject<Rank>();
            var record = await _accountService.GetRecordInformationAsync(Account.SummonerId);
            if (string.IsNullOrEmpty(record)) 
            {
                throw new InvalidOperationException("战绩拉取失败");
            }
            var recordsData = JToken.Parse(record);
            Account.Records = new ObservableCollection<Record>(recordsData["games"]["games"].ToObject<IEnumerable<Record>>().Reverse());
            Record = Account.Records.FirstOrDefault();
            PageIndex = 1;
        }

        private async Task SelectPageAsync(FunctionEventArgs<int> e)
        {
            var records = await _gameService.GetRecordsByPage((e.Info - 1) * 20, e.Info * 20, Account.Puuid);
            var recordsData = JToken.Parse(records);
            var recordObjs = new ObservableCollection<Record>(recordsData["games"]["games"].ToObject<ObservableCollection<Record>>().Reverse());
            Record = recordObjs.FirstOrDefault();
            Account.Records = recordObjs;
        }

        public async Task FetchPlayerDetailAsync(long id)
        {
            try
            {
                var infromation = await _accountService.GetSummonerInformationAsync(id);
                if (string.IsNullOrEmpty(infromation))
                {
                    Growl.InfoGlobal(new GrowlInfo()
                    {
                        WaitTime = 2,
                        Message = "查无此人",
                        ShowDateTime = false
                    });
                }

                await UserProfileToAccount(infromation);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex.ToString());
                Growl.InfoGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "查询账号信息失败!" + ex.Message,
                    ShowDateTime = false
                });
            }
        }

        public async Task GameSelectionChangedAsync()
        {
            try
            {
                if (Record == null)
                    return;

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
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Growl.InfoGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "拉取对局详情失败" + ex.Message,
                    ShowDateTime = false
                });
            }
        }
    }
}
