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

        private Account _account;
        public Account Account
        {
            get => _account;
            set => SetProperty(ref _account, value);
        }

        public AsyncRelayCommand LoadCommandAsync { get; set; }
        public AsyncRelayCommand SendTeamMateDataCommandAsync { get; set; }
        public RelayCommand TeamMateChangedCommand { get; set; }

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
            TeamMateChangedCommand = new RelayCommand(TeamMateChanged);
        }

        private async Task LoadAsync()
        {
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
                    foreach (var id in ids)
                    {
                        try
                        {
                            var account = await GetAccountAsync(id);
                            Accounts.Add(account);
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
            }
        }

        public string GetHorseInformation(Account account)
        {
            try
            {
                var sb = new StringBuilder();
                if (string.IsNullOrEmpty(_iniSettingsModel.HorseTemplate.Trim()))
                {
                    sb.Append($"我方 {account.Horse} {account.DisplayName} \n 评分：{account.Records?.Average(x => x.GetScore()).ToString("0.0")} " +
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
                                      .Replace(Constant.Score, account.Records?.Average(x => x.GetScore()).ToString("0.0"))
                                      .Replace(Constant.Solorank, $"{account.Rank.RANKED_SOLO_5x5.CnTier}{account.Rank.RANKED_SOLO_5x5.Division}")
                                      .Replace(Constant.SolorankDetail, account.Rank.RANKED_SOLO_5x5.ShortDesc)
                                      .Replace(Constant.Flexrank, $"{account.Rank.RANKED_FLEX_SR.CnTier}{account.Rank.RANKED_FLEX_SR.Division}")
                                      .Replace(Constant.FlexrankDetail, account.Rank.RANKED_FLEX_SR.ShortDesc);

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

                    sb.Append(data);
                }

                sb.Append(_iniSettingsModel.IsCloseRecommand ? string.Empty : "-来自NPhoenix助手");

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

        private void TeamMateChanged()
        {
            if (Account == null)
                return;

            var summonerAnalyse = App.ServiceProvider.GetRequiredService<SummonerAnalyse>();
            var summonerAnalyseViewModel = App.ServiceProvider.GetRequiredService<SummonerAnalyseViewModel>();
            summonerAnalyseViewModel.Account = Account;
            summonerAnalyse.DataContext = summonerAnalyseViewModel;
            summonerAnalyse.Show();
        }
    }
}
