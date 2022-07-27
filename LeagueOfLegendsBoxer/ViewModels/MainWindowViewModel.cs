using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using HandyControl.Data;
using LeagueOfLegendsBoxer.Application.ApplicationControl;
using LeagueOfLegendsBoxer.Application.Client;
using LeagueOfLegendsBoxer.Application.Event;
using LeagueOfLegendsBoxer.Application.Game;
using LeagueOfLegendsBoxer.Application.Request;
using LeagueOfLegendsBoxer.Helpers;
using LeagueOfLegendsBoxer.Models;
using LeagueOfLegendsBoxer.Pages;
using LeagueOfLegendsBoxer.Resources;
using LeagueOfLegendsBoxer.ViewModels.Pages;
using LeagueOfLegendsBoxer.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace LeagueOfLegendsBoxer.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        private readonly string _cmdPath = @"C:\Windows\System32\cmd.exe";
        private readonly string _excuteShell = "wmic PROCESS WHERE name='LeagueClientUx.exe' GET commandline";
        public AsyncRelayCommand LoadCommandAsync { get; set; }
        public RelayCommand ShiftSettingsPageCommand { get; set; }
        public RelayCommand ShiftMainPageCommand { get; set; }
        public RelayCommand CurrentUserInfoCommand { get; set; }
        public RelayCommand OpenChampionSelectToolCommand { get; set; }
        public AsyncRelayCommand ResetCommandAsync { get; set; }
        public RelayCommand ExitCommand { get; set; }

        private Page _currentPage;
        public Page CurrentPage
        {
            get => _currentPage;
            set => SetProperty(ref _currentPage, value);
        }

        private bool _connected;
        public bool Connected
        {
            get => _connected;
            set => SetProperty(ref _connected, value);
        }

        private string gameStatus;
        public string GameStatus
        {
            get => gameStatus;
            set => SetProperty(ref gameStatus, value);
        }

        private bool _isLoop = false;
        private readonly IApplicationService _applicationService;
        private readonly IRequestService _requestService;
        private readonly IGameService _gameService;
        private readonly IClientService _clientService;
        private readonly IEventService _eventService;
        private readonly IniSettingsModel _iniSettingsModel;
        private readonly IConfiguration _configuration;
        private readonly Settings _settingsPage;
        private readonly MainPage _mainPage;
        private readonly ChampionSelectTool _championSelectTool;
        private readonly ILogger<MainWindowViewModel> _logger;
        private readonly ImageManager _imageManager;
        private readonly RuneViewModel _runeViewModel;
        public MainWindowViewModel(IApplicationService applicationService,
                                   IClientService clientService,
                                   IRequestService requestService,
                                   IEventService eventService,
                                   IGameService gameService,
                                   IniSettingsModel iniSettingsModel,
                                   IConfiguration configuration,
                                   Settings settingsPage,
                                   MainPage mainPage,
                                   ImageManager imageManager,
                                   RuneViewModel runeViewModel,
                                   ChampionSelectTool championSelectTool,
                                   ILogger<MainWindowViewModel> logger)
        {
            LoadCommandAsync = new AsyncRelayCommand(LoadAsync);
            ShiftSettingsPageCommand = new RelayCommand(OpenSettingsPage);
            ShiftMainPageCommand = new RelayCommand(OpenMainPage);
            CurrentUserInfoCommand = new RelayCommand(CurrentUserInfo);
            OpenChampionSelectToolCommand = new RelayCommand(OpenChampionSelectTool);
            ResetCommandAsync = new AsyncRelayCommand(ResetAsync);
            ExitCommand = new RelayCommand(() => { Environment.Exit(0); });
            _applicationService = applicationService;
            _requestService = requestService;
            _clientService = clientService;
            _iniSettingsModel = iniSettingsModel;
            _configuration = configuration;
            _settingsPage = settingsPage;
            _championSelectTool = championSelectTool;
            _mainPage = mainPage;
            _eventService = eventService;
            _gameService = gameService;
            _logger = logger;
            _runeViewModel = runeViewModel;
            _imageManager = imageManager;
            GameStatus = "获取状态中";
        }

        private async Task LoadAsync()
        {
            await ConnnectAsync();
            await LoadConfig();
            _eventService.Subscribe(Constant.GameFlow, new EventHandler<EventArgument>(GameFlow));
            _eventService.Subscribe(Constant.ChampSelect, new EventHandler<EventArgument>(ChampSelect));
            Connected = true;
            if (CurrentPage == _mainPage) 
            {
                await (_mainPage.DataContext as MainViewModel).LoadAsync();
            }
            CurrentPage = _mainPage;
            GameStatus = "获取状态中";
            await LoopforClientStatus();
        }

        private async Task ResetAsync() 
        {
            await LoadAsync();
        }

        private async Task LoopforClientStatus()
        {
            if (_isLoop)
                return;

            _isLoop = true;
            await Task.Yield();
            while (true)
            {
                try
                {
                    var data = await _clientService.GetZoomScaleAsync();
                    await Task.Delay(1500);
                }
                catch
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        Connected = false;
                        GameStatus = "断线中...";
                    });

                    await LoadAsync();
                    await Task.Delay(1500);
                }
            }
        }

        #region 打开各页面
        private void OpenSettingsPage()
        {
            CurrentPage = _settingsPage;
        }
        private void OpenMainPage()
        {
            CurrentPage = _mainPage;
        }
        #endregion

        #region 各种事件
        private async void GameFlow(object obj, EventArgument @event)
        {
            var data = $"{@event.Data}";
            if (string.IsNullOrEmpty(data))
                return;

            switch (data)
            {
                case "ReadyCheck":
                    GameStatus = "找到对局";
                    if (_iniSettingsModel.AutoAcceptGame)
                    {
                        await AutoAcceptAsync();
                    }
                    break;
                case "ChampSelect":
                    GameStatus = "英雄选择中";
                    await ChampSelectAsync();
                    await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        _championSelectTool.Show();
                        _championSelectTool.WindowStartupLocation = WindowStartupLocation.Manual;
                        _championSelectTool.Top = (SystemParameters.PrimaryScreenHeight - _championSelectTool.ActualHeight) / 2;
                        _championSelectTool.Left = SystemParameters.PrimaryScreenWidth - _championSelectTool.ActualWidth - 10;
                    });
                    break;
                case "None":
                    GameStatus = "大厅中或正在创建对局";
                    break;
                case "Reconnect":
                    GameStatus = "游戏中,等待重新连接";
                    break;
                case "Lobby":
                    GameStatus = "房间中";
                    break;
                case "Matchmaking":
                    GameStatus = "匹配中";
                    break;
                case "InProgress":
                    GameStatus = "游戏中";
                    break;
                case "WaitingForStats":
                    GameStatus = "等待结算界面";
                    break;
                case "PreEndOfGame":
                case "EndOfGame":
                    GameStatus = "对局结束";
                    break;
                default:
                    GameStatus = "未知状态" + data;
                    break;
            }
        }

        private async void ChampSelect(object obj, EventArgument @event)
        {
            try
            {
                var gInfo = await _gameService.GetCurrentGameInfoAsync();
                var mode = JToken.Parse(gInfo)["gameData"]["queue"]["gameMode"].ToString();
                var myData = JObject.Parse(@event.Data.ToString());
                int playerCellId = int.Parse(@event.Data["localPlayerCellId"].ToString());
                IEnumerable<Team> teams = JsonConvert.DeserializeObject<IEnumerable<Team>>(@event.Data["myTeam"].ToString());
                var me = teams.FirstOrDefault(x => x.CellId == playerCellId);
                if (me == null)
                    return;

                if (mode == "ARAM")
                {
                    await System.Windows.Application.Current.Dispatcher.Invoke(async () =>
                    {
                        await _runeViewModel.LoadChampInfoAsync(me.ChampionId, true);
                    });

                    if (_iniSettingsModel.AutoLockHeroInAram)
                    {
                        int[] champs = JsonConvert.DeserializeObject<int[]>(@event.Data["benchChampionIds"].ToString());
                        var loc = _iniSettingsModel.LockHerosInAram.IndexOf(me.ChampionId);
                        loc = loc == -1 ? _iniSettingsModel.LockHerosInAram.Count : loc;
                        if (loc != 0)
                        {
                            var heros = _iniSettingsModel.LockHerosInAram.Take(loc);
                            var swapHeros = new List<int>();
                            foreach (var item in heros)
                            {
                                if (champs.Contains(item))
                                {
                                    swapHeros.Add(item);
                                }
                            }

                            for (var index = swapHeros.Count - 1; index >= 0; index--)
                            {
                                await _gameService.BenchSwapChampionsAsync(swapHeros[index]);
                            }
                        }
                    }
                }
                else
                {
                    foreach (var action in @event.Data["actions"])
                    {
                        foreach (var actionItem in action)
                        {
                            if (int.Parse(actionItem["actorCellId"].ToString()) == playerCellId)
                            {
                                if (actionItem["type"] == "pick")
                                {
                                    foreach (var teamPlayer in myData["myTeam"])
                                    {
                                        if (teamPlayer["cellId"] == playerCellId)
                                        {
                                            int champ = teamPlayer["championId"];
                                            if (int.Parse((string)actionItem["championId"]) != 0 && champ != 0)
                                            {
                                                await System.Windows.Application.Current.Dispatcher.Invoke(async () =>
                                                {
                                                    await _runeViewModel.LoadChampInfoAsync(champ, false);
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex) 
            {
            }
        }

        private async Task AutoAcceptAsync()
        {
            await _gameService.AutoAcceptGameAsync();
        }

        private void CurrentUserInfo()
        {
            if ((_mainPage?.DataContext as MainViewModel)?.Account == null)
                return;

            var summonerAnalyse = App.ServiceProvider.GetRequiredService<SummonerAnalyse>();
            var summonerAnalyseViewModel = App.ServiceProvider.GetRequiredService<SummonerAnalyseViewModel>();
            summonerAnalyseViewModel.Account = (_mainPage?.DataContext as MainViewModel)?.Account;
            summonerAnalyse.DataContext = summonerAnalyseViewModel;
            summonerAnalyse.Show();
        }

        private async Task ChampSelectAsync()
        {
            await Task.Yield();
            var _ = Task.Run(async () =>
            {
                while (true)
                {
                    var session = await _gameService.GetGameSessionAsync();
                    var token = JToken.Parse(session);
                    if (token.Value<int>("httpStatus") != 404)
                    {
                        var localPlayerCellId = token.Value<int>("localPlayerCellId");
                        var actions = token.Value<IEnumerable<IEnumerable<JToken>>>("actions");
                        int userActionID;
                        foreach (var action in actions)
                        {
                            foreach (var actionElement in action)
                            {
                                if (actionElement.Value<int>("actorCellId") == localPlayerCellId && actionElement.Value<bool>("isInProgress"))
                                {
                                    userActionID = actionElement.Value<int>("id");
                                    if (actionElement.Value<string>("type") == "pick"
                                        && !actionElement.Value<bool>("completed")
                                        && _iniSettingsModel.AutoLockHero
                                        && _iniSettingsModel.AutoLockHeroChampId != default)
                                    {
                                        await _gameService.AutoLockHeroAsync(userActionID, _iniSettingsModel.AutoLockHeroChampId);
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    await Task.Delay(500);
                }
            });
        }
        #endregion

        private async Task<string> GetAuthenticate()
        {
            using (Process p = new Process())
            {
                p.StartInfo.FileName = _cmdPath;
                p.StartInfo.UseShellExecute = false; //是否使用操作系统shell启动
                p.StartInfo.RedirectStandardInput = true; //接受来自调用程序的输入信息
                p.StartInfo.RedirectStandardOutput = true; //由调用程序获取输出信息
                p.StartInfo.RedirectStandardError = true; //重定向标准错误输出
                p.StartInfo.CreateNoWindow = true; //不显示程序窗口
                p.Start();
                p.StandardInput.WriteLine(_excuteShell.TrimEnd('&') + "&exit");
                p.StandardInput.AutoFlush = true;
                string output = await p.StandardOutput.ReadToEndAsync();
                p.WaitForExit();
                p.Close();

                return output;
            }
        }

        private async Task ConnnectAsync()
        {
            while (true)
            {
                try
                {
                    var authenticate = await GetAuthenticate();
                    if (!string.IsNullOrEmpty(authenticate) && authenticate.Contains("--remoting-auth-token="))
                    {
                        var tokenResults = authenticate.Split("--remoting-auth-token=");
                        var portResults = authenticate.Split("--app-port=");
                        var PidResults = authenticate.Split("--app-pid=");
                        var installLocations = authenticate.Split("--install-directory=");
                        Constant.Token = tokenResults[1].Substring(0, tokenResults[1].IndexOf("\""));
                        Constant.Port = int.TryParse(portResults[1].Substring(0, portResults[1].IndexOf("\"")), out var temp) ? temp : 0;
                        Constant.Pid = int.TryParse(PidResults[1].Substring(0, PidResults[1].IndexOf("\"")), out var temp1) ? temp1 : 0;
                        if (string.IsNullOrEmpty(Constant.Token) || Constant.Port == 0)
                            throw new InvalidOperationException("invalid data when try to crack.");

                        var settingFileLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _configuration.GetSection("SettingsFileLocation").Value);
                        await Task.WhenAll(_requestService.Initialize(Constant.Port, Constant.Token),
                                           _eventService.Initialize(Constant.Port, Constant.Token));

                        await _eventService.ConnectAsync();
                        break;
                    }
                    else
                        throw new InvalidOperationException("can't read right token and port");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    await Task.Delay(5000);
                }
            }
        }

        private async Task LoadConfig()
        {
            //获取所有天赋列表
            var runes = await _requestService.GetJsonResponseAsync(HttpMethod.Get, "https://game.gtimg.cn/images/lol/act/img/js/runeList/rune_list.js");
            var runeDic = JToken.Parse(runes)["rune"].ToObject<IDictionary<int, Rune>>();
            foreach (var runed in runeDic)
            {
                runed.Value.Id = runed.Key;
            }
            Constant.Runes = runeDic.Select(x => x.Value).ToList();
            var heros = await _requestService.GetJsonResponseAsync(HttpMethod.Get, "https://game.gtimg.cn/images/lol/act/img/js/heroList/hero_list.js");
            Constant.Heroes = JToken.Parse(heros)["hero"].ToObject<IEnumerable<Hero>>();
            Constant.ServerAreas = _configuration.GetSection("ServerAreas").Get<IEnumerable<ServerArea>>();
            await _iniSettingsModel.Initialize();
        }

        private void OpenChampionSelectTool()
        {
            _championSelectTool.Show();
            _championSelectTool.WindowStartupLocation = WindowStartupLocation.Manual;
            _championSelectTool.Top = (SystemParameters.PrimaryScreenHeight - _championSelectTool.ActualHeight) / 2;
            _championSelectTool.Left = SystemParameters.PrimaryScreenWidth - _championSelectTool.ActualWidth - 10;
            _championSelectTool.Topmost = true;
            var _ = Task.Run(async () =>
            {
                await Task.Delay(1000);
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    _championSelectTool.Topmost = false;
                });
            });
        }
    }
}
