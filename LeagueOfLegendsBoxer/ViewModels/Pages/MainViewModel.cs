using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using HandyControl.Data;
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
using System.Threading.Tasks;

namespace LeagueOfLegendsBoxer.ViewModels.Pages
{
    public class MainViewModel : ObservableObject
    {
        private Account _account;
        public Account Account
        {
            get => _account;
            set => SetProperty(ref _account, value);
        }

        private ObservableCollection<Record> _records;
        public ObservableCollection<Record> Records
        {
            get => _records;
            set => SetProperty(ref _records, value);
        }
        public AsyncRelayCommand LoadCommandAsync { get; set; }
        public RelayCommand CurrentUserInfoCommand { get; set; }
        private readonly IAccountService _accountService;
        private readonly IGameService _gameService;
        private readonly ILogger<MainViewModel> _logger;

        public MainViewModel(IAccountService accountService, ILogger<MainViewModel> logger, IGameService gameService)
        {
            _accountService = accountService;
            _gameService = gameService;
            _logger = logger;
            CurrentUserInfoCommand = new RelayCommand(CurrentUserInfo);
            LoadCommandAsync = new AsyncRelayCommand(LoadAsync);
        }

        public async Task LoadAsync()
        {

            try
            {
                var profile = await _accountService.GetUserAccountInformationAsync();
                Account = JsonConvert.DeserializeObject<Account>(profile);
                Constant.Account = Account;
                var records = await _accountService.GetRecordInformationAsync(Account.SummonerId);
                var recordsData = JToken.Parse(records);
                Account.Records = new ObservableCollection<Record>(recordsData["games"]["games"].ToObject<IEnumerable<Record>>().Reverse());
                var rankDataStr = await _accountService.GetUserRankInformationAsync();
                var rankData = JToken.Parse(rankDataStr);
                Account.Rank = rankData["queueMap"].ToObject<Rank>();
            }
            catch (Exception ex)
            {
                Growl.WarningGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "拉取登录信息失败,可以点击刷新重试",
                    ShowDateTime = false
                });
                _logger.LogError(ex.ToString());
            }
        }

        private void CurrentUserInfo()
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
