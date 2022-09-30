using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Gma.System.MouseKeyHook;
using HandyControl.Controls;
using HandyControl.Data;
using LeagueOfLegendsBoxer.Application.ApplicationControl;
using LeagueOfLegendsBoxer.Application.Client;
using LeagueOfLegendsBoxer.Application.Event;
using LeagueOfLegendsBoxer.Application.Game;
using LeagueOfLegendsBoxer.Application.LiveGame;
using LeagueOfLegendsBoxer.Application.Request;
using LeagueOfLegendsBoxer.Application.Teamup;
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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

using WindowsInput;
using WindowsInput.Events;
using WindowsInput.EventSources;
using Notice = LeagueOfLegendsBoxer.Models.Notice;
using Rune = LeagueOfLegendsBoxer.Models.Rune;
using Teammate = LeagueOfLegendsBoxer.Models.Teammate;

namespace LeagueOfLegendsBoxer.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        private readonly string _cmdPath = @"C:\Windows\System32\cmd.exe";
        private readonly string _excuteShell = "wmic PROCESS WHERE name='LeagueClientUx.exe' GET commandline";
        public AsyncRelayCommand LoadCommandAsync { get; set; }
        public RelayCommand ShiftSettingsPageCommand { get; set; }
        public RelayCommand ShiftMainPageCommand { get; set; }
        public RelayCommand ShiftNoticePageCommand { get; set; }
        public RelayCommand OpenChampionSelectToolCommand { get; set; }
        public RelayCommand OpenTeamDetailCommand { get; set; }
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

        private string unReadNotices;
        public string UnReadNotices
        {
            get => unReadNotices;
            set => SetProperty(ref unReadNotices, value);
        }

        private bool _isLoop = false;
        private bool _isLoopChampionSelect = false;
        private bool _isLoopLive = false;
        private IKeyboardMouseEvents _keyboardMouseEvent;
        public List<Account> Team1Accounts { get; set; } = new List<Account>();
        public List<Account> Team2Accounts { get; set; } = new List<Account>();
        private readonly IApplicationService _applicationService;
        private readonly IRequestService _requestService;
        private readonly IGameService _gameService;
        private readonly ITeamupService _teamupService;
        private readonly IClientService _clientService;
        private readonly IEventService _eventService;
        private readonly IniSettingsModel _iniSettingsModel;
        private readonly IConfiguration _configuration;
        private readonly Settings _settingsPage;
        private readonly MainPage _mainPage;
        private readonly LeagueOfLegendsBoxer.Pages.Notice _notice;
        private readonly ChampionSelectTool _championSelectTool;
        private readonly ILogger<MainWindowViewModel> _logger;
        private readonly ImageManager _imageManager;
        private readonly RuneAndItemViewModel _runeViewModel;
        private readonly ILiveGameService _livegameservice;
        private readonly TeammateViewModel _teammateViewModel;
        private readonly SettingsViewModel _settingsViewModel;
        private readonly Team1V2Window _team1V2Window;
        private readonly BlackList _blackList;
        private readonly HtmlHelper _htmlHelper;

        public MainWindowViewModel(IApplicationService applicationService,
                                   IClientService clientService,
                                   IRequestService requestService,
                                   IEventService eventService,
                                   ITeamupService teamupService,
                                   IGameService gameService,
                                   IniSettingsModel iniSettingsModel,
                                   IConfiguration configuration,
                                   Settings settingsPage,
                                   MainPage mainPage,
                                   ImageManager imageManager,
                                   RuneAndItemViewModel runeViewModel,
                                   SettingsViewModel settingsViewModel,
                                   ChampionSelectTool championSelectTool,
                                   ILogger<MainWindowViewModel> logger,
                                   ILiveGameService livegameservice,
                                   TeammateViewModel teammateViewModel,
                                   BlackList blackList,
                                   HtmlHelper htmlHelper,
                                   LeagueOfLegendsBoxer.Pages.Notice notice,
                                   Team1V2Window team1V2Window)
        {
            LoadCommandAsync = new AsyncRelayCommand(LoadAsync);
            ShiftSettingsPageCommand = new RelayCommand(OpenSettingsPage);
            ShiftMainPageCommand = new RelayCommand(OpenMainPage);
            OpenChampionSelectToolCommand = new RelayCommand(OpenChampionSelectTool);
            OpenTeamDetailCommand = new RelayCommand(OpenTeamDetail);
            ResetCommandAsync = new AsyncRelayCommand(ResetAsync);
            ShiftNoticePageCommand = new RelayCommand(OpenNoticePage);
            ExitCommand = new RelayCommand(() => { Environment.Exit(0); });
            _applicationService = applicationService;
            _requestService = requestService;
            _clientService = clientService;
            _teamupService = teamupService;
            _iniSettingsModel = iniSettingsModel;
            _configuration = configuration;
            _settingsPage = settingsPage;
            _notice = notice;
            _championSelectTool = championSelectTool;
            _mainPage = mainPage;
            _eventService = eventService;
            _settingsViewModel = settingsViewModel;
            _gameService = gameService;
            _logger = logger;
            _htmlHelper = htmlHelper;
            _blackList = blackList;
            _runeViewModel = runeViewModel;
            _imageManager = imageManager;
            GameStatus = "获取状态中";
            _livegameservice = livegameservice;
            _teammateViewModel = teammateViewModel;
            _team1V2Window = team1V2Window;
            _keyboardMouseEvent = Hook.GlobalEvents();
            _keyboardMouseEvent.KeyDown += OnKeyDown;
            _keyboardMouseEvent.KeyUp += OnKeyUp;
            WeakReferenceMessenger.Default.Register<MainWindowViewModel, IEnumerable<Notice>>(this, (x, y) =>
            {
                UnReadNotices = y.FirstOrDefault(x => x.IsMust) != null ? "必读" + y.Where(x => x.IsMust).Count() : y.Count().ToString();
            });
        }

        /// <summary>
        /// send my team record
        /// </summary>
        /// <param name="Keyboard"></param>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ListenerSendMyTeamInfoInnerGame_Triggered(IKeyboardEventSource Keyboard, object sender, KeyChordEventArgs e)
        {
            var myTeam = Team1Accounts.FirstOrDefault(x => x.SummonerId == Constant.Account.SummonerId) == null ? Team2Accounts : Team1Accounts;

            foreach (var item in myTeam)
            {
                await Task.Delay(100);
                var message = _teammateViewModel.GetGameInHorseInformation(item);
                if (string.IsNullOrWhiteSpace(message))
                {
                    continue;
                }
                _logger.LogInformation("发送我方消息:" + message);
                await InGameSendMessage(message);
            }

        }

        /// <summary>
        /// send other team record
        /// </summary>
        /// <param name="Keyboard"></param>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ListenerSendOtherTeamInfoInnerGame_Triggered(IKeyboardEventSource Keyboard, object sender, KeyChordEventArgs e)
        {
            var otherTeam = Team1Accounts.FirstOrDefault(x => x.SummonerId == Constant.Account.SummonerId) == null ? Team1Accounts : Team2Accounts;
            foreach (var item in otherTeam)
            {
                await Task.Delay(100);
                var message = _teammateViewModel.GetGameInHorseInformation(item, false);
                if (string.IsNullOrWhiteSpace(message))
                {
                    continue;
                }
                _logger.LogInformation("发送敌方消息:" + message);
                await InGameSendMessage(message);
            }
        }

        /// <summary>
        /// send 开黑信息
        /// </summary>
        /// <param name="Keyboard"></param>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ListenerTeamBuildInfo_Triggered(IKeyboardEventSource Keyboard, object sender, KeyChordEventArgs e)
        {
            var myTeam = Team1Accounts.FirstOrDefault(x => x.SummonerId == Constant.Account.SummonerId) == null ? Team2Accounts : Team1Accounts;
            var otherTeam = Team1Accounts.FirstOrDefault(x => x.SummonerId == Constant.Account.SummonerId) == null ? Team1Accounts : Team2Accounts;
            foreach (var item in myTeam.GroupBy(x => x.TeamID))
            {
                if (item.Count() >= 2)
                {
                    var sb = new StringBuilder();
                    sb.Append("我方开黑:[");
                    sb.Append(string.Join(",", item.Select(x => x.Champion?.Label)));
                    sb.Append("]");
                    await Task.Delay(300);
                    await InGameSendMessage(sb.ToString());
                }
            }

            foreach (var item in otherTeam.GroupBy(x => x.TeamID))
            {
                if (item.Count() >= 2)
                {
                    var sb = new StringBuilder();
                    sb.Append("敌方开黑:[");
                    sb.Append(string.Join(",", item.Select(x => x.Champion?.Label)));
                    sb.Append("]");
                    await Task.Delay(300);
                    await InGameSendMessage(sb.ToString());
                }
            }
        }

        private async void SendFuckWords()
        {
            if (_iniSettingsModel.FuckWordCollection != null && _iniSettingsModel.FuckWordCollection.Count <= 5)
            {
                foreach (var str in _iniSettingsModel.FuckWordCollection)
                {
                    await Task.Delay(300);
                    await InGameSendMessage(str);
                }
            }
            else if (_iniSettingsModel.FuckWordCollection != null && _iniSettingsModel.FuckWordCollection.Count > 5)
            {
                foreach (var str in _iniSettingsModel.FuckWordCollection.OrderBy(x => Guid.NewGuid()).Take(5))
                {
                    await Task.Delay(300);
                    await InGameSendMessage(str);
                }
            }
        }

        private async void SendGoodWords()
        {
            if (_iniSettingsModel.GoodWordCollection != null && _iniSettingsModel.GoodWordCollection.Count <= 5)
            {
                foreach (var str in _iniSettingsModel.GoodWordCollection)
                {
                    await Task.Delay(300);
                    await InGameSendMessage(str);
                }
            }
            else if (_iniSettingsModel.GoodWordCollection != null && _iniSettingsModel.GoodWordCollection.Count > 5)
            {
                foreach (var str in _iniSettingsModel.GoodWordCollection.OrderBy(x => Guid.NewGuid()).Take(5))
                {
                    await Task.Delay(300);
                    await InGameSendMessage(str);
                }
            }
        }

        public static Keys StringToKeys(string keyStr)
        {
            if (string.IsNullOrWhiteSpace(keyStr))
                throw new ArgumentException("Cannot be null or whitespaces.", nameof(keyStr));

            Combination combination = Combination.FromString(keyStr);
            Keys result = combination.TriggerKey;
            foreach (var chord in combination.Chord)
            {
                result |= chord;
            }
            return result;
        }

        #region 热键
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (_iniSettingsModel.IsAltQOpenVsDetail && e.KeyData == StringToKeys("Alt+Q") && (Team1Accounts.Count > 0 || Team2Accounts.Count > 0))
            {
                _team1V2Window.Opacity = 1;
                _team1V2Window.Topmost = true;
                e.Handled = true;
            }
            else if (!_iniSettingsModel.IsAltQOpenVsDetail && e.KeyData == StringToKeys("Control+Q") && (Team1Accounts.Count > 0 || Team2Accounts.Count > 0))
            {
                _team1V2Window.Opacity = 1;
                _team1V2Window.Topmost = true;
                e.Handled = true;
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (_iniSettingsModel.IsAltQOpenVsDetail && (e.KeyData == StringToKeys("Q+Alt") || e.KeyData == StringToKeys("Alt+Q")))
            {
                _team1V2Window.Opacity = 0;
                e.Handled = true;
            }
            else if (!_iniSettingsModel.IsAltQOpenVsDetail && (e.KeyData == StringToKeys("Q+Control") || e.KeyData == StringToKeys("Control+Q")))
            {
                _team1V2Window.Opacity = 0;
                e.Handled = true;
            }
            else if (e.KeyData == Keys.F7)
            {
                ListenerSendMyTeamInfoInnerGame_Triggered(null, null, null);
            }
            else if (e.KeyData == Keys.F8)
            {
                ListenerSendOtherTeamInfoInnerGame_Triggered(null, null, null);
            }
            else if (e.KeyData == Keys.F11)
            {
                ListenerTeamBuildInfo_Triggered(null, null, null);
            }
            else if (e.KeyData == StringToKeys("Control+F7"))
            {
                SendFuckWords();
            }
            else if (e.KeyData == StringToKeys("Control+F8"))
            {
                SendGoodWords();
            }
        }

        #endregion 
        private async Task LoadAsync()
        {
            CheckUpdate();
            await CheckGameNotExistWhenStartAsync();
            await LoadConfig();
            await (_notice.DataContext as NoticeViewModel).LoadAsync();
            await ConnnectAsync();
            Constant.Items = JsonConvert.DeserializeObject<IEnumerable<Item>>(await _gameService.GetItems());
            if (Constant.Items != null)
            {
                foreach (var item in Constant.Items)
                {
                    item.Description = _htmlHelper.ReplaceHtmlTag(item.Description);
                }
            }
            Constant.Spells = JsonConvert.DeserializeObject<IEnumerable<SpellModel>>(await _gameService.GetSpells());
            //等websocket恢复后在使用
            //_eventService.Subscribe(Constant.ChampSelect, new EventHandler<EventArgument>(ChampSelect));
            //_eventService.Subscribe(Constant.GameFlow, new EventHandler<EventArgument>(GameFlow));
            Connected = true;
            if (CurrentPage == _mainPage)
            {
                await (_mainPage.DataContext as MainViewModel).LoadAsync();
            }
            CurrentPage = _mainPage;
            GameStatus = "获取状态中";
            LoopLiveGameEventAsync();
            LoopGameStatus();
            LoopChampSelect();
            await LoopforClientStatus();
        }

        private void CheckUpdate()
        {
            var version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.exe");
            var updateFile = files.FirstOrDefault(f => f.Split('\\').LastOrDefault() == "NPhoenixAutoUpdateTool.exe");
            if (updateFile != null)
            {
                Process.Start(updateFile, version);
            }
            else
            {
                Task.Run(async () =>
                {
                    try
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            var responseMessage = await client.GetAsync("http://www.dotlemon.top:5200/upload/NPhoenix/NPhoenixAutoUpdateTool.exe", HttpCompletionOption.ResponseHeadersRead, CancellationToken.None).ConfigureAwait(false);
                            responseMessage.EnsureSuccessStatusCode();
                            if (responseMessage.StatusCode == HttpStatusCode.OK)
                            {
                                var filePath = Directory.GetCurrentDirectory() + "/NPhoenixAutoUpdateTool.exe";
                                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                                {
                                    await responseMessage.Content.CopyToAsync(fs);
                                }
                                if (File.Exists(filePath))
                                {
                                    Process.Start(filePath, version);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "下载自动更新程序失败:");
                    }
                });
            }
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
                    _team1V2Window.Topmost = true;
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
        private void OpenNoticePage()
        {
            CurrentPage = _notice;
        }
        #endregion

        #region 各种事件

        #region websocket恢复后再恢复监听游戏流程代码
        //private async void GameFlow(object obj, EventArgument @event)
        //{
        //    var data = $"{@event.Data}";
        //    if (string.IsNullOrEmpty(data))
        //        return;

        //    if (data == "ReadyCheck" ||
        //        data == "ChampSelect" ||
        //        data == "Lobby" ||
        //        data == "Matchmaking" ||
        //        data == "None")
        //    {
        //        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        //        {
        //            _blackList.Hide();
        //            Team1Accounts.Clear();
        //            Team2Accounts.Clear();
        //            _team1V2Window.Hide();
        //            _team1V2Window.Topmost = false;
        //        });
        //    }
        //    switch (data)
        //    {
        //        case "ReadyCheck":
        //            GameStatus = "找到对局";
        //            if (_iniSettingsModel.AutoAcceptGame)
        //            {
        //                await AutoAcceptAsync();
        //            }
        //            break;
        //        case "ChampSelect":
        //            GameStatus = "英雄选择中";
        //            await ChampSelectAsync();
        //            await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
        //            {
        //                _championSelectTool.Show();
        //                _championSelectTool.WindowStartupLocation = WindowStartupLocation.Manual;
        //                _championSelectTool.Top = (SystemParameters.PrimaryScreenHeight - _championSelectTool.ActualHeight) / 2;
        //                _championSelectTool.Left = SystemParameters.PrimaryScreenWidth - _championSelectTool.ActualWidth - 10;
        //            });
        //            break;
        //        case "None":
        //            GameStatus = "大厅中或正在创建对局";
        //            break;
        //        case "Reconnect":
        //            GameStatus = "游戏中,等待重新连接";
        //            break;
        //        case "Lobby":
        //            GameStatus = "房间中";
        //            break;
        //        case "Matchmaking":
        //            GameStatus = "匹配中";
        //            break;
        //        case "InProgress":
        //            GameStatus = "游戏中";
        //            break;
        //        case "GameStart":
        //            GameStatus = "游戏开始了";
        //            Team1Accounts = new List<Account>();
        //            Team2Accounts = new List<Account>();
        //            await ActionWhenGameBegin();
        //            break;
        //        case "WaitingForStats":
        //            GameStatus = "等待结算界面";
        //            break;
        //        case "PreEndOfGame":
        //        case "EndOfGame":
        //            GameStatus = "对局结束";
        //            ActionWhenGameEnd();
        //            break;
        //        default:
        //            GameStatus = "未知状态" + data;
        //            break;
        //    }
        //}
        #endregion

        private string _preStatus = string.Empty;

        #region websocket恢复后再删除loop游戏流程代码
        private async Task LoopGameFlow(string phase)
        {
            if (string.IsNullOrEmpty(phase) || _preStatus == phase)
                return;

            _preStatus = phase;
            if (phase == "ReadyCheck" ||
                phase == "ChampSelect" ||
                phase == "Lobby" ||
                phase == "Matchmaking" ||
                phase == "None")
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    _blackList.Hide();
                    Team1Accounts.Clear();
                    Team2Accounts.Clear();
                    _team1V2Window.Hide();
                    _team1V2Window.Topmost = false;
                });
            }
            switch (phase)
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
                case "GameStart":
                    GameStatus = "游戏开始了";
                    Team1Accounts = new List<Account>();
                    Team2Accounts = new List<Account>();
                    await ActionWhenGameBegin();
                    break;
                case "WaitingForStats":
                    GameStatus = "等待结算界面";
                    break;
                case "PreEndOfGame":
                    await EndofGameAutoExit();
                    break;
                case "EndOfGame":
                    GameStatus = "对局结束";
                    await ActionWhenGameEnd();
                    break;
                default:
                    GameStatus = "未知状态" + phase;
                    break;
            }
        }

        private void LoopGameStatus()
        {
            var _ = Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        var status = await _gameService.GetCurrentGameInfoAsync();
                        if (status == null)
                        {
                            System.Windows.Application.Current.Dispatcher.Invoke(() =>
                            {
                                _championSelectTool.Hide();
                                _blackList.Hide();
                                Team1Accounts.Clear();
                                Team2Accounts.Clear();
                                _team1V2Window.Hide();
                                _team1V2Window.Topmost = false;
                            });
                            GameStatus = "大厅或者游戏主界面";
                            await Task.Delay(500);
                            continue;
                        }

                        var phase = JObject.Parse(status)["phase"]?.ToString();
                        if (!string.IsNullOrEmpty(phase))
                        {
                            await LoopGameFlow(phase);
                        }

                        await Task.Delay(500);
                    }
                    catch
                    {
                        await Task.Delay(500);
                        continue;
                    }
                }
            });
        }
        #endregion websocket恢复后再删除
        #region websocket恢复后再删除loop选英雄流程代码
        private void LoopChampSelect()
        {
            var _ = Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        var session = await _gameService.GetGameSessionAsync();
                        if (string.IsNullOrEmpty(session))
                        {
                            await Task.Delay(500);
                            continue;
                        }

                        var token = JToken.Parse(session);
                        if (token.Value<int>("httpStatus") == 404)
                        {
                            await Task.Delay(500);
                            continue;
                        }

                        await LoopChampSelect(token);
                        await Task.Delay(500);
                    }
                    catch
                    {
                        await Task.Delay(500);
                        continue;
                    }
                }
            });
        }

        private async Task LoopChampSelect(JToken token)
        {
            try
            {
                var gInfo = await _gameService.GetCurrentGameInfoAsync();
                var mode = JToken.Parse(gInfo)["gameData"]["queue"]["gameMode"].ToString();
                int playerCellId = token.Value<int>("localPlayerCellId");
                IEnumerable<Team> teams = token["myTeam"].ToObject<IEnumerable<Team>>();
                var me = teams.FirstOrDefault(x => x.CellId == playerCellId);
                if (me == null)
                    return;

                if (mode == "ARAM")
                {
                    await System.Windows.Application.Current.Dispatcher.Invoke(async () =>
                    {
                        if (me.ChampionId != default)
                            await _runeViewModel.LoadChampInfoAsync(me.ChampionId, true);
                    });

                    if (_iniSettingsModel.AutoLockHeroInAram)
                    {
                        BenchChampion[] champs = token["benchChampions"]?.ToObject<BenchChampion[]>();
                        var loc = _iniSettingsModel.LockHerosInAram.IndexOf(me.ChampionId);
                        loc = loc == -1 ? _iniSettingsModel.LockHerosInAram.Count : loc;
                        if (loc != 0)
                        {
                            var heros = _iniSettingsModel.LockHerosInAram.Take(loc);
                            var swapHeros = new List<int>();
                            foreach (var item in heros)
                            {
                                if (champs.Select(x => x.ChampionId).ToList().Contains(item))
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
                    foreach (var action in token["actions"])
                    {
                        foreach (var actionItem in action)
                        {
                            if (int.Parse(actionItem["actorCellId"].ToString()) == playerCellId)
                            {
                                if (actionItem["type"].ToString() == "pick")
                                {
                                    foreach (var teamPlayer in token["myTeam"])
                                    {
                                        if (teamPlayer["cellId"].ToObject<int>() == playerCellId)
                                        {
                                            int champ = teamPlayer["championId"].ToObject<int>();
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
                _logger.LogError(ex.ToString());
            }
        }
        #endregion
        #region websocket恢复后再恢复监听选英雄流程代码
        //private async void ChampSelect(object obj, EventArgument @event)
        //{
        //    try
        //    {
        //        var gInfo = await _gameService.GetCurrentGameInfoAsync();
        //        var mode = JToken.Parse(gInfo)["gameData"]["queue"]["gameMode"].ToString();
        //        var myData = JObject.Parse(@event.Data.ToString());
        //        int playerCellId = int.Parse(@event.Data["localPlayerCellId"].ToString());
        //        IEnumerable<Team> teams = JsonConvert.DeserializeObject<IEnumerable<Team>>(@event.Data["myTeam"].ToString());
        //        var me = teams.FirstOrDefault(x => x.CellId == playerCellId);
        //        if (me == null)
        //            return;

        //        if (mode == "ARAM")
        //        {
        //            await System.Windows.Application.Current.Dispatcher.Invoke(async () =>
        //            {
        //                if (me.ChampionId != default)
        //                    await _runeViewModel.LoadChampInfoAsync(me.ChampionId, true);
        //            });

        //            if (_iniSettingsModel.AutoLockHeroInAram)
        //            {
        //                int[] champs = JsonConvert.DeserializeObject<int[]>(@event.Data["benchChampionIds"].ToString());
        //                var loc = _iniSettingsModel.LockHerosInAram.IndexOf(me.ChampionId);
        //                loc = loc == -1 ? _iniSettingsModel.LockHerosInAram.Count : loc;
        //                if (loc != 0)
        //                {
        //                    var heros = _iniSettingsModel.LockHerosInAram.Take(loc);
        //                    var swapHeros = new List<int>();
        //                    foreach (var item in heros)
        //                    {
        //                        if (champs.Contains(item))
        //                        {
        //                            swapHeros.Add(item);
        //                        }
        //                    }

        //                    for (var index = swapHeros.Count - 1; index >= 0; index--)
        //                    {
        //                        await _gameService.BenchSwapChampionsAsync(swapHeros[index]);
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            foreach (var action in @event.Data["actions"])
        //            {
        //                foreach (var actionItem in action)
        //                {
        //                    if (int.Parse(actionItem["actorCellId"].ToString()) == playerCellId)
        //                    {
        //                        if (actionItem["type"] == "pick")
        //                        {
        //                            foreach (var teamPlayer in myData["myTeam"])
        //                            {
        //                                if (teamPlayer["cellId"] == playerCellId)
        //                                {
        //                                    int champ = teamPlayer["championId"];
        //                                    if (int.Parse((string)actionItem["championId"]) != 0 && champ != 0)
        //                                    {
        //                                        await System.Windows.Application.Current.Dispatcher.Invoke(async () =>
        //                                        {
        //                                            await _runeViewModel.LoadChampInfoAsync(champ, false);
        //                                        });
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.ToString());
        //    }
        //}
        #endregion

        //获取游戏内实时的一些数据，目前只是获取选择的英雄和召唤师技能
        private void LoopLiveGameEventAsync()
        {
            if (_isLoopLive)
                return;

            _isLoopLive = true;
            var _ = Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        var gInfo = await _gameService.GetCurrentGameInfoAsync();
                        if (JToken.Parse(gInfo)["phase"].ToString() == "InProgress")
                        {
                            if (Team1Accounts.Count <= 0 && Team2Accounts.Count <= 0)
                            {
                                await ActionWhenGameBegin();
                            }
                            else if (Team1Accounts.All(x => x?.Champion == null) && Team2Accounts.All(x => x?.Champion == null))
                            {
                                var teams1 = await _livegameservice.GetPlayersAsync(100);
                                var teams2 = await _livegameservice.GetPlayersAsync(200);
                                if (!string.IsNullOrEmpty(teams1) && !string.IsNullOrEmpty(teams2))
                                {
                                    var token1 = JArray.Parse(teams1);
                                    var token2 = JArray.Parse(teams2);

                                    foreach (var item in token1)
                                    {
                                        var name = item["summonerName"].ToObject<string>();
                                        var account = (Team1Accounts.Concat(Team2Accounts)).FirstOrDefault(x => x?.DisplayName == name);
                                        if (account == null)
                                            continue;
                                        var championName = item["championName"].ToObject<string>();
                                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                                        {
                                            account.Champion = Constant.Heroes.FirstOrDefault(x => x.Label == championName);
                                        });
                                    }

                                    foreach (var item in token2)
                                    {
                                        var name = item["summonerName"].ToObject<string>();
                                        var account = (Team1Accounts.Concat(Team2Accounts)).FirstOrDefault(x => x?.DisplayName == name);
                                        if (account == null)
                                            continue;
                                        var championName = item["championName"].ToObject<string>();
                                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                                        {
                                            account.Champion = Constant.Heroes.FirstOrDefault(x => x.Label == championName);
                                        });
                                    }
                                }

                                await Task.Delay(5000);
                            }
                            else if (Team1Accounts.All(x => x.Spell1Id == default) && Team2Accounts.All(x => x.Spell1Id == default))
                            {
                                Team1Accounts.Concat(Team2Accounts).ToList().ForEach(async x =>
                                {
                                    if (x != null)
                                    {
                                        var spells = await _livegameservice.GetSpellByNameAsync(x.SummonerInternalName);
                                        if (!string.IsNullOrEmpty(spells))
                                        {
                                            var spell = JObject.Parse(spells).ToObject<InternalSpell>();
                                            x.Spell1Id = Constant.Spells.FirstOrDefault(x => x.Name == spell.SummonerSpellOne.DisplayName).Id;
                                            x.Spell2Id = Constant.Spells.FirstOrDefault(x => x.Name == spell.SummonerSpellTwo.DisplayName).Id;
                                        }
                                    }
                                });

                                await Task.Delay(5000);
                            }
                            else
                            {
                                await Task.Delay(30000);
                            }
                        }
                        else
                        {
                            await Task.Delay(5000);
                        }
                    }
                    catch (Exception ex)
                    {
                        await Task.Delay(5000);
                    }
                }
            });
        }
        private async Task AutoAcceptAsync()
        {
            if (_iniSettingsModel.AutoAcceptGameDelay > 0)
            {
                await Task.Delay(_iniSettingsModel.AutoAcceptGameDelay * 1000);
            }

            await _gameService.AutoAcceptGameAsync();
        }
        private async Task ChampSelectAsync()
        {
            if (_isLoopChampionSelect)
                return;

            await Task.Yield();
            _isLoopChampionSelect = true;
            var _ = Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        var session = await _gameService.GetGameSessionAsync();
                        var token = JToken.Parse(session);
                        if (token.Value<int>("httpStatus") != 404)
                        {
                            var localPlayerCellId = token.Value<int>("localPlayerCellId");

                            var me = token["myTeam"].ToObject<IEnumerable<Team>>()?.FirstOrDefault(x => x.CellId == localPlayerCellId);
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
                                            && (_iniSettingsModel.AutoLockHero || _iniSettingsModel.RankAutoLockHero))
                                        {
                                            var gInfo = await _gameService.GetCurrentGameInfoAsync();
                                            var mode = JToken.Parse(gInfo)["gameData"]["queue"]["type"].ToString();
                                            int champId = 0;
                                            switch (mode)
                                            {
                                                case "RANKED_SOLO_5x5":
                                                case "RANKED_FLEX_SR":
                                                    {
                                                        if (_iniSettingsModel.RankAutoLockHero)
                                                        {
                                                            var lockheros = me?.AssignedPosition.ToLower() switch
                                                            {
                                                                "top" => new List<int>() { _iniSettingsModel.TopAutoLockHeroChampId1, _iniSettingsModel.TopAutoLockHeroChampId2 },
                                                                "jungle" => new List<int>() { _iniSettingsModel.JungleAutoLockHeroChampId1, _iniSettingsModel.JungleAutoLockHeroChampId2 },
                                                                "middle" => new List<int>() { _iniSettingsModel.MiddleAutoLockHeroChampId1, _iniSettingsModel.MiddleAutoLockHeroChampId2 },
                                                                "bottom" => new List<int>() { _iniSettingsModel.BottomAutoLockHeroChampId1, _iniSettingsModel.BottomAutoLockHeroChampId2 },
                                                                "utility" => new List<int>() { _iniSettingsModel.UtilityAutoLockHeroChampId1, _iniSettingsModel.UtilityAutoLockHeroChampId2 },
                                                                _ => new List<int>()
                                                            };

                                                            var ids = actions?.SelectMany(x => x).ToList()?.Where(x => !x.Value<bool>("isInProgress")
                                                                                                                                && x.Value<bool>("completed")
                                                                                                                                && (x.Value<string>("type") == "pick" || x.Value<string>("type") == "ban"))?.Select(x => x.Value<int>("championId"))?.ToList();

                                                            if (ids != null)
                                                            {
                                                                champId = lockheros.FirstOrDefault(x => !ids.Contains(x) && x != 0);
                                                            }
                                                            else
                                                            {
                                                                champId = lockheros.FirstOrDefault(x => x != 0);
                                                            }
                                                        }
                                                    }
                                                    break;
                                                default:
                                                    if (_iniSettingsModel.AutoLockHero)
                                                        champId = _iniSettingsModel.AutoLockHeroChampId;
                                                    break;
                                            }

                                            if (champId != 0)
                                                await _gameService.AutoLockHeroAsync(userActionID, champId, "pick");
                                        }
                                        else if (actionElement.Value<string>("type") == "ban"
                                         && !actionElement.Value<bool>("completed")
                                         && _iniSettingsModel.AutoDisableHero
                                         && _iniSettingsModel.AutoDisableChampId != default)
                                        {
                                            await _gameService.AutoLockHeroAsync(userActionID, _iniSettingsModel.AutoDisableChampId, "ban");
                                        }
                                    }
                                }
                            }
                        }

                        await Task.Delay(500);
                    }
                    catch (Exception ex)
                    {
                        await Task.Delay(1500);
                    }
                }
            });
        }
        private async Task ActionWhenGameBegin()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() => _championSelectTool?.Hide());
            try
            {
                var gameInformation = await _gameService.GetCurrentGameInfoAsync();
                var token = JToken.Parse(gameInformation)["gameData"];
                var t1 = token["teamOne"].ToObject<IEnumerable<Teammate>>();
                var t2 = token["teamTwo"].ToObject<IEnumerable<Teammate>>();

                if (t1.All(x => x.SummonerId == default) && t2.All(x => x.SummonerId == default))
                {
                    return;
                }

                if (!t1.All(x => string.IsNullOrEmpty(x.Puuid?.Trim())))
                {
                    Team1Accounts = await TeamToAccountsAsync(t1);
                }

                if (!t2.All(x => string.IsNullOrEmpty(x.Puuid?.Trim())))
                {
                    Team2Accounts = await TeamToAccountsAsync(t2);
                }

                await System.Windows.Application.Current.Dispatcher.Invoke(async () =>
                {
                    await (_team1V2Window.DataContext as Team1V2WindowViewModel).LoadDataAsync(Team1Accounts, Team2Accounts);
                    _team1V2Window.Topmost = true;
                    _team1V2Window.Opacity = 0;
                    _team1V2Window.Show();
                    _team1V2Window.Activate();
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }
        private async Task ActionWhenGameEnd()
        {
            var game = await _gameService.GetCurrentGameInfoAsync();
            if (Team1Accounts.Count <= 0 && Team2Accounts.Count <= 0)
                return;
            var gameId = JToken.Parse(game)["gameData"].Value<long>("gameId");
            var details = await _gameService.QueryGameDetailAsync(gameId);
            var detailRecordsData = JToken.Parse(details);
            var DetailRecord = detailRecordsData.ToObject<Record>();
            var myTeam = Team1Accounts.FirstOrDefault(x => x?.SummonerId == Constant.Account?.SummonerId) == null ? Team2Accounts : Team1Accounts;
            await System.Windows.Application.Current.Dispatcher.Invoke(async () =>
            {
                await (_blackList.DataContext as BlackListViewModel).LoadAccount(DetailRecord);
                _blackList.Show();
                _blackList.WindowStartupLocation = WindowStartupLocation.Manual;
                _blackList.Top = (SystemParameters.PrimaryScreenHeight - _blackList.ActualHeight) - 50;
                _blackList.Left = SystemParameters.PrimaryScreenWidth - _blackList.ActualWidth - 10;
                _blackList.Topmost = true;
            });
        }
        private async Task<List<Account>> TeamToAccountsAsync(IEnumerable<Teammate> teammates)
        {
            var accounts = new List<Account>();
            var teamvm = App.ServiceProvider.GetRequiredService<TeammateViewModel>();
            int teamId = 1;
            List<(int, int)> teams = new List<(int, int)>();
            foreach (var id in teammates)
            {
                var account = await teamvm.GetAccountAsync(id.SummonerId);
                account.SummonerInternalName = id.SummonerInternalName;
                if (id.TeamParticipantId == null)
                {
                    account.TeamID = teamId++;
                }
                else
                {
                    var team = teams.FirstOrDefault(x => x.Item2 == id.TeamParticipantId);
                    if (team == default)
                    {
                        account.TeamID = teamId++;
                        teams.Add((account.TeamID, id.TeamParticipantId.Value));
                    }
                    else
                    {
                        account.TeamID = team.Item1;
                    }
                }

                if (!string.IsNullOrEmpty(id.GameCustomization?.Perks))
                {
                    account.Runes = new ObservableCollection<Rune>(JToken.Parse(id.GameCustomization?.Perks)["perkIds"].ToObject<IEnumerable<int>>()?.Select(x => Constant.Runes.FirstOrDefault(y => y.Id == x))?.ToList());
                }
                if (account != null)
                {
                    accounts.Add(account);
                }
            }

            return accounts;
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

        /// <summary>
        /// 首次打开不能存在lol进程
        /// </summary>
        /// <returns></returns>
        private async Task CheckGameNotExistWhenStartAsync()
        {
            var authenticate = await GetAuthenticate();
            if (!string.IsNullOrEmpty(authenticate) && authenticate.Contains("--remoting-auth-token="))
            {
                Growl.WarningGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "存在lol进程，请等待lol进程完全退出后再打开",
                    ShowDateTime = false
                });

                await Task.Delay(1000);
                Environment.Exit(0);
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
                                           _teamupService.Initialize(_configuration.GetSection("TeamupApi").Value),
                                           _eventService.Initialize(Constant.Port, Constant.Token));

                        //await _eventService.ConnectAsync();
                        break;
                    }
                    else
                        throw new InvalidOperationException("can't read right token and port");
                }
                catch (Exception ex)
                {
                    await Task.Delay(2000);
                }
            }
        }

        private async Task LoadConfig()
        {
            using (var client = new HttpClient())
            {
                //获取所有天赋列表
                var runes = await client.GetStringAsync("https://game.gtimg.cn/images/lol/act/img/js/runeList/rune_list.js");
                var runeDic = JToken.Parse(runes)["rune"].ToObject<IDictionary<int, Rune>>();
                foreach (var runed in runeDic)
                {
                    runed.Value.Id = runed.Key;
                    runed.Value.Shortdesc = _htmlHelper.ReplaceHtmlTag(runed.Value.Shortdesc);
                }
                Constant.Runes = runeDic.Select(x => x.Value).ToList();
                var heros = await client.GetStringAsync("https://game.gtimg.cn/images/lol/act/img/js/heroList/hero_list.js");
                Constant.Heroes = JToken.Parse(heros)["hero"].ToObject<IEnumerable<Hero>>();
            }
        }

        private async Task<bool> InGameSendMessage(string message)
        {
            return await Simulate.Events()
                .Click(KeyCode.Enter).Wait(75)
                .Click(message).Wait(75)
                .Click(KeyCode.Enter)
                .Invoke();
        }

        private async Task<bool> EndofGameAutoExit()
        {
            if (_iniSettingsModel.AutoEndGame)
            {
                return await Simulate.Events()
                    .Hold(KeyCode.Alt).Wait(75)
                    .Click(KeyCode.F4).Wait(75)
                    .Click(KeyCode.F4).Wait(75).Release(KeyCode.Alt).Invoke();
            }

            return false;
        }

        private void OpenChampionSelectTool()
        {
            _championSelectTool.Show();
            _championSelectTool.WindowStartupLocation = WindowStartupLocation.Manual;
            _championSelectTool.Top = (SystemParameters.PrimaryScreenHeight - _championSelectTool.ActualHeight) / 2;
            _championSelectTool.Left = SystemParameters.PrimaryScreenWidth - _championSelectTool.ActualWidth - 10;
            _championSelectTool.Topmost = true;
        }

        private void OpenTeamDetail()
        {
            _team1V2Window.Opacity = 1;
            _team1V2Window.Show();
        }
    }
}
