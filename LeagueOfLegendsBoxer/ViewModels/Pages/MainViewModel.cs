using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using HandyControl.Data;
using LeagueOfLegendsBoxer.Application.Account;
using LeagueOfLegendsBoxer.Application.Game;
using LeagueOfLegendsBoxer.Application.Teamup;
using LeagueOfLegendsBoxer.Application.Teamup.Dtos;
using LeagueOfLegendsBoxer.Helpers;
using LeagueOfLegendsBoxer.Models;
using LeagueOfLegendsBoxer.Resources;
using LeagueOfLegendsBoxer.Windows;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

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
        private readonly SoftwareHelper _softwareHelper;
        private readonly ITeamupService _teamupService;
        private readonly ChatWindow _chatWindow;
        private readonly IConfiguration _configuration;

        public MainViewModel(IAccountService accountService,
                             ILogger<MainViewModel> logger,
                             IGameService gameService,
                             ChatWindow chatWindow,
                             IConfiguration configuration,
                             SoftwareHelper softwareHelper,
                             ITeamupService teamupService)
        {
            _accountService = accountService;
            _gameService = gameService;
            _teamupService = teamupService;
            _logger = logger;
            _chatWindow = chatWindow;
            _configuration = configuration;
            _softwareHelper = softwareHelper;
            CurrentUserInfoCommand = new RelayCommand(CurrentUserInfo);
            LoadCommandAsync = new AsyncRelayCommand(LoadAsync);
        }

        public async Task LoadAsync()
        {
            try
            {
                //await Task.Delay(1000);
                var profile = await _accountService.GetUserAccountInformationAsync();
                Account = JsonConvert.DeserializeObject<Account>(profile);
                Account.ServerArea = Constant.Account?.ServerArea;
                Account.MvpRank = Constant.Account?.MvpRank;
                Account.XiaguKill = Constant.Account?.XiaguKill;
                Account.AramKill = Constant.Account?.AramKill;
                Constant.Account = Account;
                var records = await _accountService.GetRecordInformationAsync(Account.SummonerId);
                var recordsData = JToken.Parse(records);
                Account.Records = new ObservableCollection<Record>(recordsData["games"]["games"].ToObject<IEnumerable<Record>>().Reverse());
                var rankDataStr = await _accountService.GetUserRankInformationAsync();
                var rankData = JToken.Parse(rankDataStr);
                Account.Rank = rankData["queueMap"].ToObject<Rank>();
                var champData = await _gameService.QuerySummonerSuperChampDataAsync(Account.SummonerId);
                Account.Champs = JsonConvert.DeserializeObject<ObservableCollection<Champ>>(champData);
                Account.Champs = new ObservableCollection<Champ>(Account.Champs?.Take(20));

                if (!Constant.ConnectTeamupSuccessful)
                {
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
                        Constant.Account.ServerArea = resp.ServerArea;
                        Constant.Account.IsAdministrator = string.IsNullOrEmpty(resp.RoleName) ? false : resp.RoleName.Contains("Administrator");
                        App.HubConnection = new HubConnectionBuilder().WithUrl(_configuration.GetSection("TeamupChatHub").Value, option =>
                        {
                            option.CloseTimeout = TimeSpan.FromSeconds(60);
                            option.AccessTokenProvider = () => Task.FromResult(resp.Token);
                        }).WithAutomaticReconnect().Build();
                        App.HubConnection.On<ChatMessage>("ReceiveMessage", msg =>
                        {
                            System.Windows.Application.Current.Dispatcher.Invoke(() =>
                            {
                                App.ServiceProvider.GetRequiredService<TeamupViewModel>().AddNewMessage(msg);
                            });
                        });
                        App.HubConnection.On<IEnumerable<ChatMessage>>("FetchMessages", msgs =>
                        {
                            System.Windows.Application.Current.Dispatcher.Invoke(() =>
                            {
                                foreach (var msg in msgs)
                                {
                                    App.ServiceProvider.GetRequiredService<TeamupViewModel>().AddNewMessage(msg);
                                }
                            });
                        });
                        App.HubConnection.On<int>("OnlineMembers", count =>
                        {
                            System.Windows.Application.Current.Dispatcher.Invoke(() =>
                            {
                                App.ServiceProvider.GetRequiredService<TeamupViewModel>().SetOnlineCount(count);
                            });
                        });
                        App.HubConnection.On("DenyChat", () =>
                        {
                            Growl.WarningGlobal(new GrowlInfo()
                            {
                                WaitTime = 2,
                                Message = "你已被禁言，联系管理员解禁",
                                ShowDateTime = false
                            });
                        });
                        await App.HubConnection.StartAsync();
                        _chatWindow.Show();
                        var ranks = await _softwareHelper.GetRanksAsync();
                        Account.MvpRank = ranks?.Mvp.FirstOrDefault(x => x.UserId == Account.SummonerId) == null ? "未上榜" : $"{ranks?.Mvp.FirstOrDefault(x => x.UserId == Account.SummonerId).Rank}";
                        Account.XiaguKill = ranks?.Xiagu.FirstOrDefault(x => x.UserId == Account.SummonerId) == null ? "未上榜" : $"{ranks?.Xiagu.FirstOrDefault(x => x.UserId == Account.SummonerId).Rank}";
                        Account.AramKill = ranks?.Aram.FirstOrDefault(x => x.UserId == Account.SummonerId) == null ? "未上榜" : $"{ranks?.Aram.FirstOrDefault(x => x.UserId == Account.SummonerId).Rank}";
                        Constant.ConnectTeamupSuccessful = true;
                        if (string.IsNullOrEmpty(Constant.Account.ServerArea))
                        {
                            HandyControl.Controls.MessageBox.Show("请尽快在设置里配置本账号所属服务大区,否则之后将禁止登录", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
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
            summonerAnalyseViewModel.LoadPageByAccount(Account);
            summonerAnalyse.Show();
        }
    }
}
