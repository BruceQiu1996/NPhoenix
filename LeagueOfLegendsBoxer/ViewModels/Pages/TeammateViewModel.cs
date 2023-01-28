using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LeagueOfLegendsBoxer.Application.Account;
using LeagueOfLegendsBoxer.Application.Game;
using LeagueOfLegendsBoxer.Models;
using LeagueOfLegendsBoxer.Resources;
using LeagueOfLegendsBoxer.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueOfLegendsBoxer.ViewModels.Pages
{
    public class TeammateViewModel : ObservableObject
    {
        private ObservableCollection<Account> _accounts;
        public ObservableCollection<Account> Accounts
        {
            get => _accounts;
            set => SetProperty(ref _accounts, value);
        }

        public AsyncRelayCommand LoadCommandAsync { get; set; }
        public AsyncRelayCommand SendTeamMateDataCommandAsync { get; set; }
        public RelayCommand<Account> TeamMateChangedCommand { get; set; }

        private readonly IGameService _gameService;
        private readonly IAccountService _accountService;
        private readonly IniSettingsModel _iniSettingsModel;
        private readonly ILogger<TeammateViewModel> _logger;

        private string _currentChatID = string.Empty;
        public TeammateViewModel(IGameService gameService, IAccountService accountService, IniSettingsModel iniSettingsModel, ILogger<TeammateViewModel> logger)
        {
            _gameService = gameService;
            _accountService = accountService;
            _iniSettingsModel = iniSettingsModel;
            _logger = logger;
            LoadCommandAsync = new AsyncRelayCommand(LoadAsync);
            SendTeamMateDataCommandAsync = new AsyncRelayCommand(SendTeamMateDataAsync);
            TeamMateChangedCommand = new RelayCommand<Account>(TeamMateChanged);
        }

        public async Task LoadAsync()
        {
            await Task.Delay(500);
            Accounts = new ObservableCollection<Account>();
            var conversations = await _gameService.GetChatConversation();
            var token = JArray.Parse(conversations).FirstOrDefault(x => x.Value<string>("type") == "championSelect");
            if (token != null)
            {
                _currentChatID = token.Value<string>("id");
                Constant._chatId = _currentChatID;
                var messages = await _gameService.GetChatConversationMessages(_currentChatID);
                var ids = JArray.Parse(messages).Select(x => x.Value<long>("fromSummonerId")).ToHashSet();
                if (ids != null && ids.Count > 0)
                {
                    var blacks = _iniSettingsModel.BlackAccounts.Where(x => ids.Contains(x.Id));
                    if (blacks != null && blacks.Count() > 0)
                    {
                        var _window = App.ServiceProvider.GetRequiredService<BlackRecord>();
                        (_window.DataContext as BlackRecordViewModel).Load(blacks.Select(x => x.Id).ToList());
                        _window.Show();
                    }
                    var gameInfo = await _gameService.GetCurrentGameInfoAsync();
                    var mode = JToken.Parse(gameInfo)["gameData"]["queue"]["gameMode"].ToString();

                    foreach (var id in ids)
                    {
                        try
                        {
                            var account = await GetAccountAsync(id);
                            if (account != null)
                            {
                                var sameRecords = account.Records?.Where(x => x.GameMode == mode);
                                account.CurrentModeRecord = new ObservableCollection<Record>(sameRecords?.Take(5));
                                var champData = await _gameService.QuerySummonerSuperChampDataAsync(account.SummonerId);
                                account.Champs = JsonConvert.DeserializeObject<ObservableCollection<Champ>>(champData);
                                account.Champs = new ObservableCollection<Champ>(account.Champs.Take(5));
                                account.WinRate = sameRecords == null || sameRecords.Count() <= 4 ? "未知" : (sameRecords.Where(x => x.Participants.FirstOrDefault().Stats.Win).Count() * 100.0 / sameRecords.Count()).ToString("0.00") + "%";
                                account.Horse = account.GetHorse();
                                Accounts.Add(account);
                            }
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }
                    }
                }
            }
        }

        private async Task SendTeamMateDataAsync()
        {
            foreach (var account in Accounts)
            {
                var desc = GetHorseInformation(account);
                await _gameService.SendMessageAsync(_currentChatID, desc);
                await Task.Delay(300);
            }
        }

        public string GetHorseInformation(Account account)
        {
            try
            {
                var sb = new StringBuilder();
                if (string.IsNullOrEmpty(_iniSettingsModel.HorseTemplate.Trim()))
                {
                    sb.Append($"我方 {account.Horse} {account.DisplayName} 评分：{account.CurrentModeRecord?.Average(x => x.GetScore()).ToString("0.0")} " +
                        $"单双排：{account.Rank.RANKED_SOLO_5x5.CnTier}{account.Rank.RANKED_SOLO_5x5.Division} {account.Rank.RANKED_SOLO_5x5.ShortDesc}" +
                        $"灵活组排：{account.Rank.RANKED_FLEX_SR.CnTier}{account.Rank.RANKED_FLEX_SR.Division} {account.Rank.RANKED_FLEX_SR.ShortDesc} 最近五场KDA:");

                    int a = 0;
                    foreach (var record in account.Records)
                    {
                        sb.Append($"{record?.Participants?.FirstOrDefault()?.Stats?.KDA}  ");
                        a++;
                        if (a >= 5)
                            break;
                    }
                }
                else
                {
                    var data = _iniSettingsModel.HorseTemplate.Trim();
                    data = data.Replace(Constant.Horse, account.Horse)
                                      .Replace(Constant.Name, account.DisplayName)
                                      .Replace(Constant.Score, account.CurrentModeRecord?.Average(x => x.GetScore()).ToString("0.0"))
                                      .Replace(Constant.Solorank, $"{account.Rank.RANKED_SOLO_5x5.CnTier}{account.Rank.RANKED_SOLO_5x5.Division}")
                                      .Replace(Constant.SolorankDetail, account.Rank.RANKED_SOLO_5x5.ShortDesc)
                                      .Replace(Constant.Flexrank, $"{account.Rank.RANKED_FLEX_SR.CnTier}{account.Rank.RANKED_FLEX_SR.Division}")
                                      .Replace(Constant.FlexrankDetail, account.Rank.RANKED_FLEX_SR.ShortDesc)
                                      .Replace(Constant.WinRate, account.WinRate);

                    var kdadesc = new StringBuilder();
                    if (data.Contains(Constant.Kda))
                    {
                        int a = 0;
                        foreach (var record in account.Records)
                        {
                            kdadesc.Append($"{record?.Participants?.FirstOrDefault()?.Stats?.KDA}  ");
                            a++;
                            if (a >= 5)
                                break;
                        }
                        data = data.Replace(Constant.Kda, kdadesc.ToString());
                    }

                    sb.Append($"我方 {data}");
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return null;
            }
        }

        public string GetGameInHorseInformation(Account account, bool myTeam = true)
        {
            try
            {
                var sb = new StringBuilder();
                var team = myTeam ? "我方" : "敌方";
                if (string.IsNullOrEmpty(_iniSettingsModel.HorseTemplate.Trim()))
                {
                    sb.Append($"{team}{account.Horse} {account.Champion.Label} KDA:");
                    int a = 0;
                    foreach (var record in account.CurrentModeRecord)
                    {
                        sb.Append($"{record?.Participants?.FirstOrDefault()?.Stats?.KDA} ");
                        a++;
                        if (a >= 5)
                            break;
                    }
                }
                else
                {
                    var data = _iniSettingsModel.HorseTemplate.Trim();
                    data = data.Replace(Constant.Horse, account.Horse)
                                      .Replace(Constant.Name, account.DisplayName)
                                      .Replace(Constant.Score, account.CurrentModeRecord?.Average(x => x.GetScore()).ToString("0.0"))
                                      .Replace(Constant.Solorank, $"{account.Rank.RANKED_SOLO_5x5.CnTier}{account.Rank.RANKED_SOLO_5x5.Division}")
                                      .Replace(Constant.SolorankDetail, account.Rank.RANKED_SOLO_5x5.ShortDesc)
                                      .Replace(Constant.Flexrank, $"{account.Rank.RANKED_FLEX_SR.CnTier}{account.Rank.RANKED_FLEX_SR.Division}")
                                      .Replace(Constant.FlexrankDetail, account.Rank.RANKED_FLEX_SR.ShortDesc)
                                      .Replace(Constant.WinRate, account.WinRate);

                    var kdadesc = new StringBuilder();
                    if (data.Contains(Constant.Kda))
                    {
                        int a = 0;
                        foreach (var record in account.CurrentModeRecord)
                        {
                            kdadesc.Append($"{record?.Participants?.FirstOrDefault()?.Stats?.KDA}  ");
                            a++;
                            if (a >= 5)
                                break;
                        }
                        data = data.Replace(Constant.Kda, kdadesc.ToString());
                    }

                    sb.Append($"{team}{data}");
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return null;
            }
        }

        public async Task<Account> GetAccountAsync(long id)
        {
            Account account = null;
            try
            {
                var infromation = await _accountService.GetSummonerInformationAsync(id);
                account = JsonConvert.DeserializeObject<Account>(infromation);
                var recordsData = JToken.Parse(await _accountService.GetRecordInformationAsync(account.SummonerId));
                account.Records = new ObservableCollection<Record>(recordsData["games"]["games"].ToObject<IEnumerable<Record>>().Reverse());
                var rankData = JToken.Parse(await _accountService.GetSummonerRankInformationAsync(account.Puuid));
                account.Rank = rankData["queueMap"].ToObject<Rank>();

                return account;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{account?.DisplayName}{ex}");
                return account;
            }
        }

        private void TeamMateChanged(Account account)
        {
            if (account == null)
                return;

            ShowRecord(account);
        }

        public void ShowRecord(Account account)
        {
            var summonerAnalyse = App.ServiceProvider.GetRequiredService<SummonerAnalyse>();
            var summonerAnalyseViewModel = App.ServiceProvider.GetRequiredService<SummonerAnalyseViewModel>();
            summonerAnalyseViewModel.LoadPageByAccount(account);
            summonerAnalyse.Show();
        }
    }
}
