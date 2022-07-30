using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LeagueOfLegendsBoxer.Application.ApplicationControl;
using LeagueOfLegendsBoxer.Application.Client;
using LeagueOfLegendsBoxer.Application.Event;
using LeagueOfLegendsBoxer.Application.Game;
using LeagueOfLegendsBoxer.Application.Request;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using NPhoenixSPA.Models;
using NPhoenixSPA.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Controls;
using Account = NPhoenixSPA.Pages.Account;
using Record = NPhoenixSPA.Pages.Record;
using Menu = NPhoenixSPA.Models.Menu;
using Newtonsoft.Json;
using NPhoenixSPA.Pages;

namespace NPhoenixSPA.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        private readonly string _cmdPath = @"C:\Windows\System32\cmd.exe";
        private readonly string _excuteShell = "wmic PROCESS WHERE name='LeagueClientUx.exe' GET commandline";

        private ObservableCollection<Menu> _menus;
        public ObservableCollection<Menu> Menus
        {
            get { return _menus; }
            set { SetProperty(ref _menus, value); }
        }

        private Menu _menu;
        public Menu Menu
        {
            get { return _menu; }
            set
            {
                if (value != _menu)
                    value.Action?.Invoke();
                SetProperty(ref _menu, value);
            }
        }

        private Page _currentPage;
        public Page CurrentPage
        {
            get { return _currentPage; }
            set
            {
                SetProperty(ref _currentPage, value);
            }
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
        public AsyncRelayCommand LoadCommandAsync { get; set; }

        private readonly IApplicationService _applicationService;
        private readonly IRequestService _requestService;
        private readonly IGameService _gameService;
        private readonly IClientService _clientService;
        private readonly IEventService _eventService;
        private readonly ILogger<MainWindowViewModel> _logger;
        private readonly IniSettingsModel _iniSettingsModel;
        private readonly IConfiguration _configuration;
        private readonly Account _account;
        private readonly Record _record;

        public MainWindowViewModel(IApplicationService applicationService,
                                   IClientService clientService,
                                   IRequestService requestService,
                                   IEventService eventService,
                                   IGameService gameService,
                                   IConfiguration configuration,
                                   IniSettingsModel iniSettingsModel,
                                   Account account,
                                   Record record,
                                   Setting setting,
                                   ILogger<MainWindowViewModel> logger)
        {
            Menus = new ObservableCollection<Menu>()
            {
                new Menu()
                {
                    Name = "账号信息",
                    Icon = System.Windows.Application.Current.FindResource("account"),
                    Action=()=>CurrentPage = _account
                },
                new Menu()
                {
                    Name = "战绩查询",
                    Icon = System.Windows.Application.Current.FindResource("record"),
                    Action=()=>CurrentPage = _record
                },
                new Menu()
                {
                    Name = "国服数据",
                    Icon = System.Windows.Application.Current.FindResource("data")

                },
                new Menu()
                {
                    Name = "符文数据",
                    Icon = System.Windows.Application.Current.FindResource("book")
                },
                new Menu()
                {
                    Name = "设置",
                    Icon = System.Windows.Application.Current.FindResource("setting"),
                    Action=()=>CurrentPage = setting
                },
            };

            _applicationService = applicationService;
            _requestService = requestService;
            _clientService = clientService;
            _eventService = eventService;
            _gameService = gameService;
            _configuration = configuration;
            _logger = logger;
            _iniSettingsModel = iniSettingsModel;
            _account = account;
            _record = record;
            LoadCommandAsync = new AsyncRelayCommand(LoadAsync);
            GameStatus = "获取客户端状态中";
        }

        private async Task LoadAsync()
        {
            await ConnnectAsync();
            await LoadConfig();
            _eventService.Subscribe(Constant.GameFlow, new EventHandler<EventArgument>(GameFlow));
            Connected = true;
            if (CurrentPage == _account)
            {
                await (_account.DataContext as AccountViewModel).LoadAsync();
            }
            Menu = Menus.FirstOrDefault();
            Menu.Action?.Invoke();
            GameStatus = "获取客户端状态中";
            await LoopforClientStatus();
        }

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

        private async Task ConnnectAsync()
        {
            while (true)
            {
                var authenticate = string.Empty;
                try
                {
                    authenticate = await GetAuthenticate();
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
                    _logger.LogError($"auth profile:{authenticate}");
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
            Constant.Items = JsonConvert.DeserializeObject<IEnumerable<Item>>(await _gameService.GetItems());
            Constant.Icons = JsonConvert.DeserializeObject<IEnumerable<Icon>>(await _gameService.GetIcons()).Reverse();

            await _iniSettingsModel.Initialize();
        }

        #region 事件
        private async void GameFlow(object obj, EventArgument @event)
        {
            var data = $"{@event.Data}";
            if (string.IsNullOrEmpty(data))
                return;

            switch (data)
            {
                case "ReadyCheck":
                    GameStatus = "找到对局";
                    break;
                case "ChampSelect":
                    GameStatus = "英雄选择中";
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
        #endregion
    }
}
