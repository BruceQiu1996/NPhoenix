using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using HandyControl.Data;
using LeagueOfLegendsBoxer.Application.Account;
using LeagueOfLegendsBoxer.Application.Game;
using LeagueOfLegendsBoxer.Application.Teamup;
using LeagueOfLegendsBoxer.Application.Teamup.Dtos;
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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Principal;
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
        private readonly ITeamupService _teamupService;

        public MainViewModel(IAccountService accountService, ILogger<MainViewModel> logger, IGameService gameService, ITeamupService teamupService)
        {
            _accountService = accountService;
            _gameService = gameService;
            _teamupService = teamupService;
            _logger = logger;
            CurrentUserInfoCommand = new RelayCommand(CurrentUserInfo);
            LoadCommandAsync = new AsyncRelayCommand(LoadAsync);
        }

        public async Task LoadAsync()
        {
            try
            {
                await Task.Delay(2000);
                var profile = await _accountService.GetUserAccountInformationAsync();
                Account = JsonConvert.DeserializeObject<Account>(profile);
                Constant.Account = Account;
                var records = await _accountService.GetRecordInformationAsync(Account.SummonerId);
                var recordsData = JToken.Parse(records);
                Account.Records = new ObservableCollection<Record>(recordsData["games"]["games"].ToObject<IEnumerable<Record>>().Reverse());
                var rankDataStr = await _accountService.GetUserRankInformationAsync();
                var rankData = JToken.Parse(rankDataStr);
                Account.Rank = rankData["queueMap"].ToObject<Rank>();
                var resp = await _teamupService.LoginAsync(new UserCreateOrUpdateByClientDto()
                {
                    Id = Account.SummonerId,
                    UserName = Account.DisplayName,
                    Rank_FLEX_SR = $"{Account.Rank.RANKED_FLEX_SR.CnTier}{Account.Rank.RANKED_FLEX_SR.Division}",
                    Rank_SOLO_5x5 = $"{Account.Rank.RANKED_SOLO_5x5.CnTier}{Account.Rank.RANKED_SOLO_5x5.Division}",
                    Version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion
                });

                if (resp == null)
                {
                    Growl.WarningGlobal(new GrowlInfo()
                    {
                        WaitTime = 2,
                        Message = "连接服务器出现问题,部分功能不能使用",
                        ShowDateTime = false
                    });
                    Constant.ConnectTeamupSuccessful = false;
                }
                else if (resp.IsDeleted)
                {
                    Growl.WarningGlobal(new GrowlInfo()
                    {
                        WaitTime = 2,
                        Message = "该账号已被封禁，请联系管理员",
                        ShowDateTime = false
                    });

                    await Task.Delay(2000);
                    Environment.Exit(0);
                } 
                else
                {
                    _teamupService.SetToken(resp.Token);
                    Constant.ConnectTeamupSuccessful = true;
                }
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
