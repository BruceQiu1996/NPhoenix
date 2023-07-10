using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LeagueOfLegendsBoxer.Application.Game;
using LeagueOfLegendsBoxer.Models;
using LeagueOfLegendsBoxer.Resources;
using LeagueOfLegendsBoxer.Windows;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace LeagueOfLegendsBoxer.ViewModels
{
    public class Team1V2WindowViewModel : ObservableObject
    {
        public RelayCommand<Account> SearchAccountRecordCommand { get; set; }

        public ObservableCollection<Account> _team1Accounts;
        public ObservableCollection<Account> Team1Accounts
        {
            get { return _team1Accounts; }
            set { SetProperty(ref _team1Accounts, value); }
        }

        public ObservableCollection<Account> _team2Accounts;
        public ObservableCollection<Account> Team2Accounts
        {
            get { return _team2Accounts; }
            set { SetProperty(ref _team2Accounts, value); }
        }

        public bool _already14Minutes;
        public bool Already14Minutes
        {
            get { return _already14Minutes; }
            set { SetProperty(ref _already14Minutes, value); }
        }

        private readonly IniSettingsModel _iniSettingsModel;
        private readonly IGameService _gameService;
        public Team1V2WindowViewModel(IniSettingsModel iniSettingsModel, IGameService gameService)
        {
            SearchAccountRecordCommand = new RelayCommand<Account>(SearchAccountRecord);
            _iniSettingsModel = iniSettingsModel;
            _gameService = gameService;
        }

        public async Task LoadDataAsync(IList<Account> t1, IList<Account> t2)
        {
            Already14Minutes = false;
            Team1Accounts = new ObservableCollection<Account>(t1);
            Team2Accounts = new ObservableCollection<Account>(t2);
            var gameInfo = await _gameService.GetCurrentGameInfoAsync();
            var mode = JToken.Parse(gameInfo)["gameData"]["queue"]["gameMode"].ToString();
            var queue = JToken.Parse(gameInfo)["gameData"]["queue"]["id"].Value<int>();
            foreach (Account item in Team1Accounts.Concat(Team2Accounts))
            {
                try
                {
                    if (mode == "ARAM")
                    {
                        item._isAram = true;
                    }

                    if (item.Records == null || item.Records.Count <= 0)
                    {
                        item.WinRate = "未知";
                        item.Horse = "未知的马";
                    }
                    else
                    {
                        var sameRecords = item.Records?.Where(x => x.QueueId == queue);
                        item.CurrentModeRecord = new ObservableCollection<Record>(sameRecords);
                        item.WinRate = sameRecords == null || sameRecords.Count() <= 4 ? "未知" : (sameRecords.Where(x => x.Participants.FirstOrDefault().Stats.Win).Count() * 100.0 / sameRecords.Count()).ToString("0.00") + "%";
                        item.Horse = item.GetHorse();
                        item.KDA = item.GetKDA();
                        item.SurRate = item.GetSurrenderRate();
                    }
                    var champData = await _gameService.QuerySummonerSuperChampDataAsync(item.SummonerId);
                    if (!string.IsNullOrEmpty(champData))
                    {
                        item.Champs = JsonConvert.DeserializeObject<ObservableCollection<Champ>>(champData);
                    }
                    item.IsInBlackList = _iniSettingsModel.BlackAccounts?.FirstOrDefault(x => x.Id == item.SummonerId) != null;
                    if (item.IsInBlackList)
                    {
                        var sb = new StringBuilder();
                        var records = _iniSettingsModel.BlackAccounts?.Where(x => x.Id == item.SummonerId);
                        foreach (var record in records.OrderByDescending(x => x.CreateTime))
                        {
                            sb.Append(record.Reason + record.CreateTime.ToString("d"));
                            sb.Append("\n");
                        }

                        item.BlackInfo = sb.ToString();
                    }
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
        }

        private async void SearchAccountRecord(Account account)
        {
            var summonerAnalyse = App.ServiceProvider.GetRequiredService<SummonerAnalyse>();
            var summonerAnalyseViewModel = App.ServiceProvider.GetRequiredService<SummonerAnalyseViewModel>();
            summonerAnalyseViewModel.LoadPageByAccount(account);
            summonerAnalyse.Topmost = true;
            summonerAnalyse.Show();
            await Task.Delay(500);
            summonerAnalyse.Topmost = false;
        }
    }
}
