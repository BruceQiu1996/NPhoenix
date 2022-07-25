using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LeagueOfLegendsBoxer.Application.Account;
using LeagueOfLegendsBoxer.Application.Game;
using LeagueOfLegendsBoxer.Models;
using LeagueOfLegendsBoxer.Windows;
using Microsoft.Extensions.DependencyInjection;
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

        private string _currentChatID = string.Empty;
        public TeammateViewModel(IGameService gameService, IAccountService accountService)
        {
            _gameService = gameService;
            _accountService = accountService;
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
                var messages = await _gameService.GetChatConversationMessages(_currentChatID);
                var ids = JArray.Parse(messages).Select(x => x.Value<long>("fromSummonerId")).ToHashSet();
                if (ids != null && ids.Count > 0) 
                {
                    foreach (var id in ids) 
                    {
                        try
                        {
                            var infromation = await _accountService.GetSummonerInformationAsync(id);
                            var account = JsonConvert.DeserializeObject<Account>(infromation);
                            var rankData = JToken.Parse(await _accountService.GetSummonerRankInformationAsync(account.Puuid));
                            account.Rank = rankData["queueMap"].ToObject<Rank>();
                            var recordsData = JToken.Parse(await _accountService.GetRecordInformationAsync(account.SummonerId));
                            account.Records = new ObservableCollection<Record>(recordsData["games"]["games"].ToObject<IEnumerable<Record>>().Reverse());
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
            var sb = new StringBuilder("来自NPhoenix助手 \n");
            foreach (var account in Accounts)
            {
                sb.Append($"我方 {account.Horse} [{account.DisplayName}] \n 评分：{account.Records?.Average(x => x.GetScore()).ToString("0.0")} " +
                    $"单双排：{account.Rank.RANKED_SOLO_5x5.CnTier}{account.Rank.RANKED_SOLO_5x5.Division} {account.Rank.RANKED_SOLO_5x5.Desc}" +
                    $"灵活组排：{account.Rank.RANKED_FLEX_SR.CnTier}{account.Rank.RANKED_FLEX_SR.Division} {account.Rank.RANKED_FLEX_SR.Desc} 最近五场KDA:");

                int a = 0;
                foreach (var record in account.Records) 
                {
                    sb.Append($"{record?.Participants?.FirstOrDefault()?.Stats?.KDA}  ");
                    a++;
                    if (a >= 5)
                        break;
                }
                sb.Append("\n\n");
            }

            await _gameService.SendMessageAsync(_currentChatID, sb.ToString());
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
