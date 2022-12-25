using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using HandyControl.Data;
using LeagueOfLegendsBoxer.Application.ApplicationControl;
using LeagueOfLegendsBoxer.Helpers;
using LeagueOfLegendsBoxer.Models;
using LeagueOfLegendsBoxer.Resources;
using LeagueOfLegendsBoxer.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using ServerArea = LeagueOfLegendsBoxer.Windows.ServerArea;

namespace LeagueOfLegendsBoxer.ViewModels.Pages
{
    public class SettingsViewModel : ObservableObject
    {
        private bool _isDarkTheme;
        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set => SetProperty(ref _isDarkTheme, value);
        }

        private bool _autoAcceptGame;
        public bool AutoAcceptGame
        {
            get => _autoAcceptGame;
            set => SetProperty(ref _autoAcceptGame, value);
        }

        private bool _autoEndGame;
        public bool AutoEndGame
        {
            get => _autoEndGame;
            set => SetProperty(ref _autoEndGame, value);
        }

        private bool _autoStartGame;
        public bool AutoStartGame
        {
            get => _autoStartGame;
            set => SetProperty(ref _autoStartGame, value);
        }

        private bool _autoStartWhenComputerRun;
        public bool AutoStartWhenComputerRun
        {
            get => _autoStartWhenComputerRun;
            set => SetProperty(ref _autoStartWhenComputerRun, value);
        }

        private bool _autoLockHero;
        public bool AutoLockHero
        {
            get => _autoLockHero;
            set => SetProperty(ref _autoLockHero, value);
        }

        private bool _rankAutoLockHero;
        public bool RankAutoLockHero
        {
            get => _rankAutoLockHero;
            set => SetProperty(ref _rankAutoLockHero, value);
        }

        private bool _autoDisableHero;
        public bool AutoDisableHero
        {
            get => _autoDisableHero;
            set => SetProperty(ref _autoDisableHero, value);
        }

        private bool _disableHerosOpen;
        public bool DisableHerosOpen
        {
            get => _disableHerosOpen;
            set => SetProperty(ref _disableHerosOpen, value);
        }

        private bool _lockHerosOpen;
        public bool LockHerosOpen
        {
            get => _lockHerosOpen;
            set => SetProperty(ref _lockHerosOpen, value);
        }

        private bool _topLockHerosOpen1;
        public bool TopLockHerosOpen1
        {
            get => _topLockHerosOpen1;
            set => SetProperty(ref _topLockHerosOpen1, value);

        }

        private bool _topLockHerosOpen2;
        public bool TopLockHerosOpen2
        {
            get => _topLockHerosOpen2;
            set => SetProperty(ref _topLockHerosOpen2, value);

        }

        private bool _jungleLockHerosOpen1;
        public bool JungleLockHerosOpen1
        {
            get => _jungleLockHerosOpen1;
            set => SetProperty(ref _jungleLockHerosOpen1, value);

        }

        private bool _jungleLockHerosOpen2;
        public bool JungleLockHerosOpen2
        {
            get => _jungleLockHerosOpen2;
            set => SetProperty(ref _jungleLockHerosOpen2, value);

        }

        private bool _middleLockHerosOpen1;
        public bool MiddleLockHerosOpen1
        {
            get => _middleLockHerosOpen1;
            set => SetProperty(ref _middleLockHerosOpen1, value);
        }

        private bool _middleLockHerosOpen2;
        public bool MiddleLockHerosOpen2
        {
            get => _middleLockHerosOpen2;
            set => SetProperty(ref _middleLockHerosOpen2, value);
        }

        private bool _bottomLockHerosOpen1;
        public bool BottomLockHerosOpen1
        {
            get => _bottomLockHerosOpen1;
            set => SetProperty(ref _bottomLockHerosOpen1, value);
        }

        private bool _bottomLockHerosOpen2;
        public bool BottomLockHerosOpen2
        {
            get => _bottomLockHerosOpen2;
            set => SetProperty(ref _bottomLockHerosOpen2, value);
        }

        private bool _utilityLockHerosOpen1;
        public bool UtilityLockHerosOpen1
        {
            get => _utilityLockHerosOpen1;
            set => SetProperty(ref _utilityLockHerosOpen1, value);
        }

        private bool _utilityLockHerosOpen2;
        public bool UtilityLockHerosOpen2
        {
            get => _utilityLockHerosOpen2;
            set => SetProperty(ref _utilityLockHerosOpen2, value);

        }
        private bool _autoLockHeroInAram;
        public bool AutoLockHeroInAram
        {
            get => _autoLockHeroInAram;
            set => SetProperty(ref _autoLockHeroInAram, value);
        }

        private bool _isCloseRecommmand;
        public bool IsCloseRecommmand
        {
            get => _isCloseRecommmand;
            set => SetProperty(ref _isCloseRecommmand, value);
        }

        private bool _closeSendOtherWhenBegin;
        public bool CloseSendOtherWhenBegin
        {
            get => _closeSendOtherWhenBegin;
            set => SetProperty(ref _closeSendOtherWhenBegin, value);
        }


        private ObservableCollection<Hero> _lockHeros;
        public ObservableCollection<Hero> LockHeros
        {
            get => _lockHeros;
            set => SetProperty(ref _lockHeros, value);
        }

        private ObservableCollection<Hero> _topLockHeros1;
        public ObservableCollection<Hero> TopLockHeros1
        {
            get => _topLockHeros1;
            set => SetProperty(ref _topLockHeros1, value);
        }

        private ObservableCollection<Hero> _topLockHeros2;
        public ObservableCollection<Hero> TopLockHeros2
        {
            get => _topLockHeros2;
            set => SetProperty(ref _topLockHeros2, value);
        }

        private ObservableCollection<Hero> _JungleLockHeros1;
        public ObservableCollection<Hero> JungleLockHeros1
        {
            get => _JungleLockHeros1;
            set => SetProperty(ref _JungleLockHeros1, value);
        }

        private ObservableCollection<Hero> _JungleLockHeros2;
        public ObservableCollection<Hero> JungleLockHeros2
        {
            get => _JungleLockHeros2;
            set => SetProperty(ref _JungleLockHeros2, value);
        }

        private ObservableCollection<Hero> _MiddleLockHeros1;
        public ObservableCollection<Hero> MiddleLockHeros1
        {
            get => _MiddleLockHeros1;
            set => SetProperty(ref _MiddleLockHeros1, value);
        }

        private ObservableCollection<Hero> _MiddleLockHeros2;
        public ObservableCollection<Hero> MiddleLockHeros2
        {
            get => _MiddleLockHeros2;
            set => SetProperty(ref _MiddleLockHeros2, value);
        }

        private ObservableCollection<Hero> _BottomLockHeros1;
        public ObservableCollection<Hero> BottomLockHeros1
        {
            get => _BottomLockHeros1;
            set => SetProperty(ref _BottomLockHeros1, value);
        }

        private ObservableCollection<Hero> _BottomLockHeros2;
        public ObservableCollection<Hero> BottomLockHeros2
        {
            get => _BottomLockHeros2;
            set => SetProperty(ref _BottomLockHeros2, value);
        }

        private ObservableCollection<Hero> _UtilityLockHeros1;
        public ObservableCollection<Hero> UtilityLockHeros1
        {
            get => _UtilityLockHeros1;
            set => SetProperty(ref _UtilityLockHeros1, value);
        }

        private ObservableCollection<Hero> _UtilityLockHeros2;
        public ObservableCollection<Hero> UtilityLockHeros2
        {
            get => _UtilityLockHeros2;
            set => SetProperty(ref _UtilityLockHeros2, value);
        }

        private Hero _lockHero;
        public Hero LockHero
        {
            get => _lockHero;
            set
            {
                SetProperty(ref _lockHero, value);
                if (value != null)
                    _iniSettingsModel.WriteAutoLockHeroIdAsync(value.ChampId).GetAwaiter().GetResult();
            }
        }

        private Hero _topLockHero1;
        public Hero TopLockHero1
        {
            get => _topLockHero1;
            set
            {
                SetProperty(ref _topLockHero1, value);
                if (value != null)
                    _iniSettingsModel.WriteTopAutoLockHeroChampId1Async(value.ChampId).GetAwaiter().GetResult();
            }
        }

        private Hero _topLockHero2;
        public Hero TopLockHero2
        {
            get => _topLockHero2;
            set
            {
                SetProperty(ref _topLockHero2, value);
                if (value != null)
                    _iniSettingsModel.WriteTopAutoLockHeroChampId2Async(value.ChampId).GetAwaiter().GetResult();
            }
        }

        private Hero _jungleLockHero1;
        public Hero JungleLockHero1
        {
            get => _jungleLockHero1;
            set
            {
                SetProperty(ref _jungleLockHero1, value);
                if (value != null)
                    _iniSettingsModel.WriteJungleAutoLockHeroChampId1Async(value.ChampId).GetAwaiter().GetResult();
            }
        }

        private Hero _jungleLockHero2;
        public Hero JungleLockHero2
        {
            get => _jungleLockHero2;
            set
            {
                SetProperty(ref _jungleLockHero2, value);
                if (value != null)
                    _iniSettingsModel.WriteJungleAutoLockHeroChampId2Async(value.ChampId).GetAwaiter().GetResult();
            }
        }

        private Hero _middleLockHero1;
        public Hero MiddleLockHero1
        {
            get => _middleLockHero1;
            set
            {
                SetProperty(ref _middleLockHero1, value);
                if (value != null)
                    _iniSettingsModel.WriteMiddleAutoLockHeroChampId1Async(value.ChampId).GetAwaiter().GetResult();
            }
        }

        private Hero _middleLockHero2;
        public Hero MiddleLockHero2
        {
            get => _middleLockHero2;
            set
            {
                SetProperty(ref _middleLockHero2, value);
                if (value != null)
                    _iniSettingsModel.WriteMiddleAutoLockHeroChampId2Async(value.ChampId).GetAwaiter().GetResult();
            }
        }

        private Hero _bottomLockHero1;
        public Hero BottomLockHero1
        {
            get => _bottomLockHero1;
            set
            {
                SetProperty(ref _bottomLockHero1, value);
                if (value != null)
                    _iniSettingsModel.WriteBottomAutoLockHeroChampId1Async(value.ChampId).GetAwaiter().GetResult();
            }
        }

        private Hero _bottomLockHero2;
        public Hero BottomLockHero2
        {
            get => _bottomLockHero2;
            set
            {
                SetProperty(ref _bottomLockHero2, value);
                if (value != null)
                    _iniSettingsModel.WriteBottomAutoLockHeroChampId2Async(value.ChampId).GetAwaiter().GetResult();
            }
        }

        private Hero _utilityLockHero1;
        public Hero UtilityLockHero1
        {
            get => _utilityLockHero1;
            set
            {
                SetProperty(ref _utilityLockHero1, value);
                if (value != null)
                    _iniSettingsModel.WriteUtilityAutoLockHeroChampId1Async(value.ChampId).GetAwaiter().GetResult();
            }
        }

        private Hero _utilityLockHero2;
        public Hero UtilityLockHero2
        {
            get => _utilityLockHero2;
            set
            {
                SetProperty(ref _utilityLockHero2, value);
                if (value != null)
                    _iniSettingsModel.WriteUtilityAutoLockHeroChampId2Async(value.ChampId).GetAwaiter().GetResult();
            }
        }

        private ObservableCollection<Hero> _disableHeros;
        public ObservableCollection<Hero> DisableHeros
        {
            get => _disableHeros;
            set => SetProperty(ref _disableHeros, value);
        }

        private Hero _disableHero;
        public Hero DisableHero
        {
            get => _disableHero;
            set
            {
                SetProperty(ref _disableHero, value);
                if (value != null)
                    _iniSettingsModel.WriteAutoDisableHeroIdAsync(value.ChampId).GetAwaiter().GetResult();
            }
        }

        private string _preSearchDisableText;
        private string _searchDisableText;
        public string SearchDisableText
        {
            get => _searchDisableText;
            set => SetProperty(ref _searchDisableText, value);
        }

        private string _preSearchLockText;
        private string _searchLockText;
        public string SearchLockText
        {
            get => _searchLockText;
            set => SetProperty(ref _searchLockText, value);
        }

        private string _preTopSearchLockText1;
        private string _topSearchLockText1;
        public string TopSearchLockText1
        {
            get => _topSearchLockText1;
            set => SetProperty(ref _topSearchLockText1, value);
        }

        private string _preTopSearchLockText2;
        private string _topSearchLockText2;
        public string TopSearchLockText2
        {
            get => _topSearchLockText2;
            set => SetProperty(ref _topSearchLockText2, value);
        }

        private string _preJungleSearchLockText1;
        private string _jungleSearchLockText1;
        public string JungleSearchLockText1
        {
            get => _jungleSearchLockText1;
            set => SetProperty(ref _jungleSearchLockText1, value);
        }

        private string _preJungleSearchLockText2;
        private string _jungleSearchLockText2;
        public string JungleSearchLockText2
        {
            get => _jungleSearchLockText2;
            set => SetProperty(ref _jungleSearchLockText2, value);
        }

        private string _preMiddleSearchLockText1;
        private string _middleSearchLockText1;
        public string MiddleSearchLockText1
        {
            get => _middleSearchLockText1;
            set => SetProperty(ref _middleSearchLockText1, value);
        }

        private string _preMiddleSearchLockText2;
        private string _middleSearchLockText2;
        public string MiddleSearchLockText2
        {
            get => _middleSearchLockText2;
            set => SetProperty(ref _middleSearchLockText2, value);
        }

        private string _preBottomSearchLockText1;
        private string _bottomSearchLockText1;
        public string BottomSearchLockText1
        {
            get => _bottomSearchLockText1;
            set => SetProperty(ref _bottomSearchLockText1, value);
        }

        private string _preBottomSearchLockText2;
        private string _bottomSearchLockText2;
        public string BottomSearchLockText2
        {
            get => _bottomSearchLockText2;
            set => SetProperty(ref _bottomSearchLockText2, value);
        }

        private string _preUtilitySearchLockText1;
        private string _utilitySearchLockText1;
        public string UtilitySearchLockText1
        {
            get => _utilitySearchLockText1;
            set => SetProperty(ref _utilitySearchLockText1, value);
        }

        private string _preUtilitySearchLockText2;
        private string _utilitySearchLockText2;
        public string UtilitySearchLockText2
        {
            get => _utilitySearchLockText2;
            set => SetProperty(ref _utilitySearchLockText2, value);

        }


        private ObservableCollection<Hero> _chooseHeroForSkins;
        public ObservableCollection<Hero> ChooseHeroForSkins
        {
            get => _chooseHeroForSkins;
            set => SetProperty(ref _chooseHeroForSkins, value);
        }

        private Hero _chooseHeroForSkin;
        public Hero ChooseHeroForSkin
        {
            get => _chooseHeroForSkin;
            set
            {
                SetProperty(ref _chooseHeroForSkin, value);
            }
        }

        private string _preSearchChooseHeroForSkinText;
        private string _searchChooseHeroForSkinText;
        public string SearchChooseHeroForSkinText
        {
            get => _searchChooseHeroForSkinText;
            set => SetProperty(ref _searchChooseHeroForSkinText, value);
        }

        private bool _chooseHeroForSkinsOpen;
        public bool ChooseHeroForSkinsOpen
        {
            get => _chooseHeroForSkinsOpen;
            set => SetProperty(ref _chooseHeroForSkinsOpen, value);
        }

        private string _gameStartupLocation;
        public string GameStartupLocation
        {
            get => _gameStartupLocation;
            set => SetProperty(ref _gameStartupLocation, value);
        }

        private string _rankSetting;
        public string RankSetting
        {
            get => _rankSetting;
            set => SetProperty(ref _rankSetting, value);
        }

        private string _horseTemplate;
        public string HorseTemplate
        {
            get => _horseTemplate;
            set => SetProperty(ref _horseTemplate, value);
        }

        private string _above120ScoreTxt;
        public string Above120ScoreTxt
        {
            get => _above120ScoreTxt;
            set => SetProperty(ref _above120ScoreTxt, value);
        }

        private string _above110ScoreTxt;
        public string Above110ScoreTxt
        {
            get => _above110ScoreTxt;
            set => SetProperty(ref _above110ScoreTxt, value);
        }

        private string _above100ScoreTxt;
        public string Above100ScoreTxt
        {
            get => _above100ScoreTxt;
            set => SetProperty(ref _above100ScoreTxt, value);
        }

        private string _below100ScoreTxt;
        public string Below100ScoreTxt
        {
            get => _below100ScoreTxt;
            set => SetProperty(ref _below100ScoreTxt, value);
        }

        private string _version;
        public string Version
        {
            get => _version;
            set => SetProperty(ref _version, value);
        }

        /// <summary>
        /// 自动接收游戏延迟
        /// </summary>
        private int _autoAcceptGameDelay;
        public int AutoAcceptGameDelay
        {
            get => _autoAcceptGameDelay;
            set => SetProperty(ref _autoAcceptGameDelay, value);
        }

        private bool _isAltQOpenVsDetail;
        public bool IsAltQOpenVsDetail
        {
            get => _isAltQOpenVsDetail;
            set => SetProperty(ref _isAltQOpenVsDetail, value);
        }

        private string _fuckWords;
        public string FuckWords
        {
            get => _fuckWords;
            set => SetProperty(ref _fuckWords, value);
        }

        private string _goodWords;
        public string GoodWords
        {
            get => _goodWords;
            set => SetProperty(ref _goodWords, value);
        }


        private string _teamDetailKeys;

        public string TeamDetailKeys
        {
            get => _teamDetailKeys;
            set => SetProperty(ref _teamDetailKeys, value);
        }

        public bool IsOpenModifyHotkeys { get; set; }


        private string _signature;
        public string Signature
        {
            get => _signature;
            set => SetProperty(ref _signature, value);
        }
        
        public AsyncRelayCommand CheckedAutoAcceptCommandAsync { get; set; }
        public AsyncRelayCommand UncheckedAutoAcceptCommandAsync { get; set; }
        public AsyncRelayCommand CheckedAutoStartWhenComputerRunCommandAsync { get; set; }
        public AsyncRelayCommand UncheckedAutoStartWhenComputerRunCommandAsync { get; set; }
        public AsyncRelayCommand CheckedAutoStartGameCommandAsync { get; set; }
        public AsyncRelayCommand UncheckedAutoStartGameCommandAsync { get; set; }
        public AsyncRelayCommand CheckedAutoLockHeroCommandAsync { get; set; }
        public AsyncRelayCommand UncheckedAutoLockHeroCommandAsync { get; set; }
        public AsyncRelayCommand CheckedRankAutoLockHeroCommandAsync { get; set; }
        public AsyncRelayCommand UncheckedRankAutoLockHeroCommandAsync { get; set; }
        public AsyncRelayCommand CheckedAutoDisableHeroCommandAsync { get; set; }
        public AsyncRelayCommand UncheckedAutoDisableHeroCommandAsync { get; set; }
        public AsyncRelayCommand CheckedAutoLockHeroInAramCommandAsync { get; set; }
        public AsyncRelayCommand UncheckedAutoLockHeroInAramCommandAsync { get; set; }
        public AsyncRelayCommand CheckedAutoEndGameCommandAsync { get; set; }
        public AsyncRelayCommand UncheckedAutoEndGameCommandAsync { get; set; }
        public AsyncRelayCommand GetGameFolderCommandAsync { get; set; }
        public AsyncRelayCommand ManualGetGameFolderCommandAsync { get; set; }
        public AsyncRelayCommand ExitGameCommandAsync { get; set; }
        public AsyncRelayCommand LoadCommandAsync { get; set; }
        public RelayCommand SearchLockHeroCommand { get; set; }
        public RelayCommand TopSearchLockHeroCommand1 { get; set; }
        public RelayCommand TopSearchLockHeroCommand2 { get; set; }
        public RelayCommand JungleSearchLockHeroCommand1 { get; set; }
        public RelayCommand JungleSearchLockHeroCommand2 { get; set; }
        public RelayCommand MiddleSearchLockHeroCommand1 { get; set; }
        public RelayCommand MiddleSearchLockHeroCommand2 { get; set; }
        public RelayCommand BottomSearchLockHeroCommand1 { get; set; }
        public RelayCommand BottomSearchLockHeroCommand2 { get; set; }
        public RelayCommand UtilitySearchLockHeroCommand1 { get; set; }
        public RelayCommand UtilitySearchLockHeroCommand2 { get; set; }
        public RelayCommand SearchDisableHeroCommand { get; set; }
        public RelayCommand SearchHeroForSkinCommand { get; set; }
        public AsyncRelayCommand ModifyRankLevelCommandAsync { get; set; }
        public AsyncRelayCommand SearchSkinsForHeroCommandAsync { get; set; }
        public AsyncRelayCommand FetchRunesAndItemsCommandAsync { get; set; }
        public RelayCommand OpenAramChooseCommand { get; set; }
        public RelayCommand StartGameCommand { get; set; }
        public AsyncRelayCommand CheckedCloseSendOtherWhenBeginCommandAsync { get; set; }
        public AsyncRelayCommand UnCheckedCloseSendOtherWhenBeginCommandAsync { get; set; }
        public AsyncRelayCommand SaveHorseTemplateCommandAsync { get; set; }
        public RelayCommand OpenBlackRecordCommand { get; set; }
        public AsyncRelayCommand AutoAcceptGameDelayChangedCommandAsync { get; set; }
        public AsyncRelayCommand CheckUseAltQOpenVsDetailCommandAsync { get; set; }
        public AsyncRelayCommand UnCheckUseAltQOpenVsDetailCommandAsync { get; set; }
        public AsyncRelayCommand ChooseLightThemeCommandAsync { get; set; }
        public AsyncRelayCommand ChooseDarkThemeCommandAsync { get; set; }
        public AsyncRelayCommand SaveFuckWordsCommandAsync { get; set; }
        public AsyncRelayCommand SaveGoodWordsCommandAsync { get; set; }
        public AsyncRelayCommand ManualUpdateCommandAsync { get; set; }
        public RelayCommand PayCommand { get; set; }
        public RelayCommand ChooseServerAreaForCurrentAccountCommand { get; set; }
        public RelayCommand OpenFireModeCommand { get; set; }
        public RelayCommand ManageRuneCommand { get; set; }
        public AsyncRelayCommand SettingSignatureCommand { get; set; }

        public AsyncRelayCommand SaveTeamDetailKeyCommand { get; set; }

        private readonly IniSettingsModel _iniSettingsModel;
        private readonly IApplicationService _applicationService;
        private readonly IConfiguration _iconfiguration;
        private readonly ILogger<SettingsViewModel> _logger;
        private readonly IServiceProvider serviceProvider;
        private readonly Pay _pay;
        private readonly SoftwareHelper _softwareHelper;
        private readonly ManageRune _manageRune;

        public SettingsViewModel(IniSettingsModel iniSettingsModel,
                                 IApplicationService applicationService,
                                 Pay pay,
                                 SoftwareHelper softwareHelper,
                                 ManageRune manageRune,
                                 IConfiguration iconfiguration,
                                 ILogger<SettingsViewModel> logger,
                                 IServiceProvider serviceProvider)
        {
            _pay = pay;
            _iniSettingsModel = iniSettingsModel;
            _applicationService = applicationService;
            _iconfiguration = iconfiguration;
            _logger = logger;
            this.serviceProvider = serviceProvider;
            _manageRune = manageRune;
            _softwareHelper = softwareHelper;
            PayCommand = new RelayCommand(PayMethod);
            CheckedAutoAcceptCommandAsync = new AsyncRelayCommand(CheckedAutoAcceptAsync);
            UncheckedAutoAcceptCommandAsync = new AsyncRelayCommand(UncheckedAutoAcceptAsync);
            CheckedAutoStartWhenComputerRunCommandAsync = new AsyncRelayCommand(CheckedAutoStartWhenComputerRunAsync);
            UncheckedAutoStartWhenComputerRunCommandAsync = new AsyncRelayCommand(UncheckedAutoStartWhenComputerRunAsync);
            CheckedAutoStartGameCommandAsync = new AsyncRelayCommand(CheckedAutoStartGameAsync);
            UncheckedAutoStartGameCommandAsync = new AsyncRelayCommand(UncheckedAutoStartGameAsync);
            CheckedAutoLockHeroCommandAsync = new AsyncRelayCommand(CheckedAutoLockHeroAsync);
            UncheckedAutoLockHeroCommandAsync = new AsyncRelayCommand(UncheckedAutoLockHeroAsync);
            CheckedRankAutoLockHeroCommandAsync = new AsyncRelayCommand(CheckedRankAutoLockHeroAsync);
            UncheckedRankAutoLockHeroCommandAsync = new AsyncRelayCommand(UncheckedRankAutoLockHeroAsync);
            CheckedAutoDisableHeroCommandAsync = new AsyncRelayCommand(CheckedAutoDisableHeroAsync);
            UncheckedAutoDisableHeroCommandAsync = new AsyncRelayCommand(UncheckedAutoDisableHeroAsync);
            CheckedAutoLockHeroInAramCommandAsync = new AsyncRelayCommand(CheckedAutoLockHeroInAramAsync);
            UncheckedAutoLockHeroInAramCommandAsync = new AsyncRelayCommand(UncheckedAutoLockHeroInAramAsync);
            CheckedAutoEndGameCommandAsync = new AsyncRelayCommand(CheckedAutoEndGameAsync);
            UncheckedAutoEndGameCommandAsync = new AsyncRelayCommand(UncheckedAutoEndGameAsync);
            LoadCommandAsync = new AsyncRelayCommand(LoadAsync);
            SearchLockHeroCommand = new RelayCommand(SearchLockHero);
            OpenFireModeCommand = new RelayCommand(OpenFireMode);

            TopSearchLockHeroCommand1 = new RelayCommand(TopSearchLockHero1);
            TopSearchLockHeroCommand2 = new RelayCommand(TopSearchLockHero2);
            JungleSearchLockHeroCommand1 = new RelayCommand(JungleSearchLockHero1);
            JungleSearchLockHeroCommand2 = new RelayCommand(JungleSearchLockHero2);
            MiddleSearchLockHeroCommand1 = new RelayCommand(MiddleSearchLockHero1);
            MiddleSearchLockHeroCommand2 = new RelayCommand(MiddleSearchLockHero2);
            BottomSearchLockHeroCommand1 = new RelayCommand(BottomSearchLockHero1);
            BottomSearchLockHeroCommand2 = new RelayCommand(BottomSearchLockHero2);
            UtilitySearchLockHeroCommand1 = new RelayCommand(UtilitySearchLockHero1);
            UtilitySearchLockHeroCommand2 = new RelayCommand(UtilitySearchLockHero2);

            SearchDisableHeroCommand = new RelayCommand(SearchDisableHero);//设置禁用英雄
            GetGameFolderCommandAsync = new AsyncRelayCommand(GetGameFolderAsync);//获取游戏所在目录
            ManualGetGameFolderCommandAsync = new AsyncRelayCommand(ManualGetGameFolderAsync);//手动获取游戏所在目录
            ExitGameCommandAsync = new AsyncRelayCommand(ExitGameAsync);//游戏进程是否存在
            ModifyRankLevelCommandAsync = new AsyncRelayCommand(ModifyRankLevelAsync);//修改段位
            SearchHeroForSkinCommand = new RelayCommand(SearchHeroForSkin);//获取英雄的皮肤
            FetchRunesAndItemsCommandAsync = new AsyncRelayCommand(FetchRunesAndItemsAsync);//获取符文和装备推荐
            SearchSkinsForHeroCommandAsync = new AsyncRelayCommand(SearchSkinsForHeroAsync);//获取英雄的皮肤
            OpenAramChooseCommand = new RelayCommand(OpenAramChoose);//打开大乱斗秒选设置界面
            StartGameCommand = new RelayCommand(StartGame);//启动游戏
            CheckedCloseSendOtherWhenBeginCommandAsync = new AsyncRelayCommand(CheckedCloseSendOtherWhenBeginAsync);
            UnCheckedCloseSendOtherWhenBeginCommandAsync = new AsyncRelayCommand(UnCheckedCloseSendOtherWhenBeginAsync);
            SaveHorseTemplateCommandAsync = new AsyncRelayCommand(SaveHorseTemplateAsync);
            OpenBlackRecordCommand = new RelayCommand(OpenBlackRecord);
            AutoAcceptGameDelayChangedCommandAsync = new AsyncRelayCommand(AutoAcceptGameDelayChangedAsync);
            CheckUseAltQOpenVsDetailCommandAsync = new AsyncRelayCommand(CheckUseAltQOpenVsDetailAsync);
            UnCheckUseAltQOpenVsDetailCommandAsync = new AsyncRelayCommand(UnCheckUseAltQOpenVsDetailAsync);
            ChooseLightThemeCommandAsync = new AsyncRelayCommand(ChooseLightThemeAsync);
            ChooseDarkThemeCommandAsync = new AsyncRelayCommand(ChooseDarkThemeAsync);
            SaveFuckWordsCommandAsync = new AsyncRelayCommand(SaveFuckWordsAsync);
            SaveGoodWordsCommandAsync = new AsyncRelayCommand(SaveGoodWordsAsync);
            ManualUpdateCommandAsync = new AsyncRelayCommand(ManualUpdateAsync);
            ChooseServerAreaForCurrentAccountCommand = new RelayCommand(ChooseServerAreaForCurrentAccount);
            ManageRuneCommand = new RelayCommand(ManageRune);//打开符文管理界面

            SaveTeamDetailKeyCommand = new AsyncRelayCommand(SaveTeamDetailKey);
            SettingSignatureCommand = new AsyncRelayCommand(SettingSignature);//设置个人签名
        }

        private void PayMethod()
        {
            _pay.ShowDialog();
            _pay.Topmost = true;
        }

        private void ManageRune()
        {
            _manageRune.ShowDialog();
        }

        private void ChooseServerAreaForCurrentAccount()
        {
            if (Constant.Account == null)
            {
                Growl.WarningGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "成功登录后才可以设置所在服务大区",
                    ShowDateTime = false
                });

                return;
            }

            var window = App.ServiceProvider.GetRequiredService<ServerArea>();
            window.Show();
        }

        private async Task LoadAsync()
        {
            LockHeros = new ObservableCollection<Hero>(Constant.Heroes);
            LockHeros.Insert(0, new Hero()
            {
                ChampId = 0
            });

            TopLockHeros1 = new ObservableCollection<Hero>(Constant.Heroes);
            TopLockHeros1.Insert(0, new Hero()
            {
                ChampId = 0
            });
            TopLockHeros2 = new ObservableCollection<Hero>(Constant.Heroes);
            TopLockHeros2.Insert(0, new Hero()
            {
                ChampId = 0
            });

            JungleLockHeros1 = new ObservableCollection<Hero>(Constant.Heroes);
            JungleLockHeros1.Insert(0, new Hero()
            {
                ChampId = 0
            });
            JungleLockHeros2 = new ObservableCollection<Hero>(Constant.Heroes);
            JungleLockHeros2.Insert(0, new Hero()
            {
                ChampId = 0
            });

            MiddleLockHeros1 = new ObservableCollection<Hero>(Constant.Heroes);
            MiddleLockHeros1.Insert(0, new Hero()
            {
                ChampId = 0
            });
            MiddleLockHeros2 = new ObservableCollection<Hero>(Constant.Heroes);
            MiddleLockHeros2.Insert(0, new Hero()
            {
                ChampId = 0
            });

            BottomLockHeros1 = new ObservableCollection<Hero>(Constant.Heroes);
            BottomLockHeros1.Insert(0, new Hero()
            {
                ChampId = 0
            });
            BottomLockHeros2 = new ObservableCollection<Hero>(Constant.Heroes);
            BottomLockHeros2.Insert(0, new Hero()
            {
                ChampId = 0
            });

            UtilityLockHeros1 = new ObservableCollection<Hero>(Constant.Heroes);
            UtilityLockHeros1.Insert(0, new Hero()
            {
                ChampId = 0
            });
            UtilityLockHeros2 = new ObservableCollection<Hero>(Constant.Heroes);
            UtilityLockHeros2.Insert(0, new Hero()
            {
                ChampId = 0
            });

            DisableHeros = new ObservableCollection<Hero>(Constant.Heroes);
            ChooseHeroForSkins = new ObservableCollection<Hero>(Constant.Heroes);
            AutoAcceptGame = _iniSettingsModel.AutoAcceptGame;
            AutoStartWhenComputerRun = _iniSettingsModel.AutoStartWhenComputerRun;
            AutoDisableHero = _iniSettingsModel.AutoDisableHero;
            RankAutoLockHero = _iniSettingsModel.RankAutoLockHero;
            AutoLockHero = _iniSettingsModel.AutoLockHero;
            AutoLockHeroInAram = _iniSettingsModel.AutoLockHeroInAram;
            DisableHero = DisableHeros.FirstOrDefault(x => x?.ChampId == _iniSettingsModel.AutoDisableChampId);
            LockHero = LockHeros.FirstOrDefault(x => x?.ChampId == _iniSettingsModel.AutoLockHeroChampId);

            TopLockHero1 = TopLockHeros1.FirstOrDefault(x => x?.ChampId == _iniSettingsModel.TopAutoLockHeroChampId1);
            TopLockHero2 = TopLockHeros2.FirstOrDefault(x => x?.ChampId == _iniSettingsModel.TopAutoLockHeroChampId2);
            MiddleLockHero1 = MiddleLockHeros1.FirstOrDefault(x => x?.ChampId == _iniSettingsModel.MiddleAutoLockHeroChampId1);
            MiddleLockHero2 = MiddleLockHeros2.FirstOrDefault(x => x?.ChampId == _iniSettingsModel.MiddleAutoLockHeroChampId2);
            JungleLockHero1 = JungleLockHeros1.FirstOrDefault(x => x?.ChampId == _iniSettingsModel.JungleAutoLockHeroChampId1);
            JungleLockHero2 = JungleLockHeros2.FirstOrDefault(x => x?.ChampId == _iniSettingsModel.JungleAutoLockHeroChampId2);
            BottomLockHero1 = BottomLockHeros1.FirstOrDefault(x => x?.ChampId == _iniSettingsModel.BottomAutoLockHeroChampId1);
            BottomLockHero2 = BottomLockHeros2.FirstOrDefault(x => x?.ChampId == _iniSettingsModel.BottomAutoLockHeroChampId2);
            UtilityLockHero1 = UtilityLockHeros1.FirstOrDefault(x => x?.ChampId == _iniSettingsModel.UtilityAutoLockHeroChampId1);
            UtilityLockHero2 = UtilityLockHeros2.FirstOrDefault(x => x?.ChampId == _iniSettingsModel.UtilityAutoLockHeroChampId2);

            GameStartupLocation = _iniSettingsModel.GameExeLocation;
            CloseSendOtherWhenBegin = _iniSettingsModel.CloseSendOtherWhenBegin;
            HorseTemplate = _iniSettingsModel.HorseTemplate;
            Version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            AutoAcceptGameDelay = _iniSettingsModel.AutoAcceptGameDelay;
            AutoStartGame = _iniSettingsModel.AutoStartGame;
            AutoEndGame = _iniSettingsModel.AutoEndGame;
            Above120ScoreTxt = _iniSettingsModel.Above120ScoreTxt;
            Above110ScoreTxt = _iniSettingsModel.Above110ScoreTxt;
            Above100ScoreTxt = _iniSettingsModel.Above100ScoreTxt;
            Below100ScoreTxt = _iniSettingsModel.Below100ScoreTxt;
            IsAltQOpenVsDetail = _iniSettingsModel.IsAltQOpenVsDetail;
            IsDarkTheme = _iniSettingsModel.IsDarkTheme;
            FuckWords = _iniSettingsModel.FuckWords;
            GoodWords = _iniSettingsModel.GoodWords;

            TeamDetailKeys = _iniSettingsModel.TeamDetailKeys;
        }

        #region checkbox
        private async Task CheckedAutoAcceptAsync()
        {
            await _iniSettingsModel.WriteAutoAcceptAsync(true);
            AutoAcceptGame = true;
        }
        private async Task UncheckedAutoAcceptAsync()
        {
            await _iniSettingsModel.WriteAutoAcceptAsync(false);
            AutoAcceptGame = false;
        }

        private async Task CheckedAutoStartWhenComputerRunAsync()
        {
            _softwareHelper.RunAtStart(true);
            await _iniSettingsModel.WriteAutoStartWhenComputerRun(true);
            AutoStartWhenComputerRun = true;
        }

        private async Task UncheckedAutoStartWhenComputerRunAsync()
        {
            _softwareHelper.RunAtStart(false);
            await _iniSettingsModel.WriteAutoStartWhenComputerRun(false);
            AutoStartWhenComputerRun = false;
        }

        private async Task CheckedAutoStartGameAsync()
        {
            await _iniSettingsModel.WriteAutoStartGameAsync(true);
            AutoStartGame = true;
        }
        private async Task UncheckedAutoStartGameAsync()
        {
            await _iniSettingsModel.WriteAutoStartGameAsync(false);
            AutoStartGame = false;
        }
        private async Task CheckedAutoLockHeroAsync()
        {
            await _iniSettingsModel.WriteAutoLockHeroAsync(true);
            AutoLockHero = true;
        }
        private async Task UncheckedAutoLockHeroAsync()
        {
            await _iniSettingsModel.WriteAutoLockHeroAsync(false);
            AutoLockHero = false;
        }

        private async Task CheckedRankAutoLockHeroAsync()
        {
            await _iniSettingsModel.WriteRankAutoLockHeroAsync(true);
            RankAutoLockHero = true;
        }

        private async Task UncheckedRankAutoLockHeroAsync()
        {
            await _iniSettingsModel.WriteRankAutoLockHeroAsync(false);
            RankAutoLockHero = false;
        }

        private async Task CheckedAutoDisableHeroAsync()
        {
            await _iniSettingsModel.WriteAutoDisableHeroAsync(true);
            AutoDisableHero = true;
        }
        private async Task UncheckedAutoDisableHeroAsync()
        {
            await _iniSettingsModel.WriteAutoDisableHeroAsync(false);
            AutoDisableHero = false;
        }
        private async Task CheckedAutoLockHeroInAramAsync()
        {
            await _iniSettingsModel.WriteAutoLockHeroInAramAsync(true);
            AutoLockHeroInAram = true;
        }
        private async Task UncheckedAutoLockHeroInAramAsync()
        {
            await _iniSettingsModel.WriteAutoLockHeroInAramAsync(false);
            AutoLockHeroInAram = false;
        }
        private async Task CheckedCloseSendOtherWhenBeginAsync()
        {
            await _iniSettingsModel.WriteCloseSendOtherWhenBeginAsync(true);
            CloseSendOtherWhenBegin = true;
        }
        private async Task UnCheckedCloseSendOtherWhenBeginAsync()
        {
            await _iniSettingsModel.WriteCloseSendOtherWhenBeginAsync(false);
            CloseSendOtherWhenBegin = false;
        }

        private async Task CheckUseAltQOpenVsDetailAsync()
        {
            await _iniSettingsModel.WriteIsAltQOpenVsDetail(true);
        }

        private async Task UnCheckUseAltQOpenVsDetailAsync()
        {
            await _iniSettingsModel.WriteIsAltQOpenVsDetail(false);
        }

        private async Task SaveHorseTemplateAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(Above120ScoreTxt) || string.IsNullOrEmpty(Above110ScoreTxt) || string.IsNullOrEmpty(Above100ScoreTxt) || string.IsNullOrEmpty(Below100ScoreTxt))
                {
                    Growl.WarningGlobal(new GrowlInfo()
                    {
                        WaitTime = 2,
                        Message = "马匹名不能为空",
                        ShowDateTime = false
                    });

                    return;
                }

                if (Above120ScoreTxt.Length > 5 || Above110ScoreTxt.Length > 5 || Above100ScoreTxt.Length > 5 || Below100ScoreTxt.Length > 5)
                {
                    Growl.WarningGlobal(new GrowlInfo()
                    {
                        WaitTime = 2,
                        Message = "马匹名不能超过5个字符",
                        ShowDateTime = false
                    });

                    return;
                }

                await _iniSettingsModel.WriteHorseTemplateAsync(HorseTemplate);
                await _iniSettingsModel.WriteAbove120ScoreTxtAsync(Above120ScoreTxt);
                await _iniSettingsModel.WriteAbove110ScoreTxtAsync(Above110ScoreTxt);
                await _iniSettingsModel.WriteAbove100ScoreTxtAsync(Above100ScoreTxt);
                await _iniSettingsModel.WriteBelow100ScoreTxtAsync(Below100ScoreTxt);

                Growl.SuccessGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "设置成功",
                    ShowDateTime = false
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Growl.WarningGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "设置异常",
                    ShowDateTime = false
                });
            }
        }

        private async Task AutoAcceptGameDelayChangedAsync()
        {
            await _iniSettingsModel.WriteAutoAcceptGameDelay(AutoAcceptGameDelay);
        }

        private async Task CheckedAutoEndGameAsync()
        {
            await _iniSettingsModel.WriteAutoEndGameAsync(true);
            AutoEndGame = true;
        }

        private async Task UncheckedAutoEndGameAsync()
        {
            await _iniSettingsModel.WriteAutoEndGameAsync(false);
            AutoEndGame = false;
        }
        #endregion

        private void SearchLockHero()
        {
            LockHerosOpen = true;
            if (_preSearchLockText == SearchLockText)
                return;
            _preSearchLockText = SearchLockText;
            LockHero = null;
            if (string.IsNullOrEmpty(SearchLockText))
                LockHeros = new ObservableCollection<Hero>(Constant.Heroes);
            else
                LockHeros = new ObservableCollection<Hero>(Constant.Heroes.Where(x => x.Label.Contains(SearchLockText) || x.Title.Contains(SearchLockText)));
        }

        private void OpenFireMode()
        {
            Process.Start("notepad.exe", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/12.10firemode.txt"));
        }

        private void TopSearchLockHero1()
        {
            TopLockHerosOpen1 = true;
            if (_preTopSearchLockText1 == TopSearchLockText1)
                return;
            _preTopSearchLockText1 = TopSearchLockText1;
            TopLockHero1 = null;
            if (string.IsNullOrEmpty(TopSearchLockText1))
                TopLockHeros1 = new ObservableCollection<Hero>(Constant.Heroes);
            else
                TopLockHeros1 = new ObservableCollection<Hero>(Constant.Heroes.Where(x => x.Label.Contains(TopSearchLockText1) || x.Title.Contains(TopSearchLockText1)));
        }

        private void TopSearchLockHero2()
        {
            TopLockHerosOpen2 = true;
            if (_preTopSearchLockText2 == TopSearchLockText2)
                return;
            _preTopSearchLockText2 = TopSearchLockText2;
            TopLockHero2 = null;
            if (string.IsNullOrEmpty(TopSearchLockText2))
                TopLockHeros2 = new ObservableCollection<Hero>(Constant.Heroes);
            else
                TopLockHeros2 = new ObservableCollection<Hero>(Constant.Heroes.Where(x => x.Label.Contains(TopSearchLockText2) || x.Title.Contains(TopSearchLockText2)));
        }

        private void JungleSearchLockHero1()
        {
            JungleLockHerosOpen1 = true;
            if (_preJungleSearchLockText1 == JungleSearchLockText1)
                return;
            _preJungleSearchLockText1 = JungleSearchLockText1;
            JungleLockHero1 = null;
            if (string.IsNullOrEmpty(JungleSearchLockText1))
                JungleLockHeros1 = new ObservableCollection<Hero>(Constant.Heroes);
            else
                JungleLockHeros1 = new ObservableCollection<Hero>(Constant.Heroes.Where(x => x.Label.Contains(JungleSearchLockText1) || x.Title.Contains(JungleSearchLockText1)));
        }

        private void JungleSearchLockHero2()
        {
            JungleLockHerosOpen2 = true;
            if (_preJungleSearchLockText2 == JungleSearchLockText2)
                return;
            _preJungleSearchLockText2 = JungleSearchLockText2;
            JungleLockHero2 = null;
            if (string.IsNullOrEmpty(JungleSearchLockText2))
                JungleLockHeros2 = new ObservableCollection<Hero>(Constant.Heroes);
            else
                JungleLockHeros2 = new ObservableCollection<Hero>(Constant.Heroes.Where(x => x.Label.Contains(JungleSearchLockText2) || x.Title.Contains(JungleSearchLockText2)));
        }

        private void MiddleSearchLockHero1()
        {
            MiddleLockHerosOpen1 = true;
            if (_preMiddleSearchLockText1 == MiddleSearchLockText1)
                return;
            _preMiddleSearchLockText1 = MiddleSearchLockText1;
            MiddleLockHero1 = null;
            if (string.IsNullOrEmpty(MiddleSearchLockText1))
                MiddleLockHeros1 = new ObservableCollection<Hero>(Constant.Heroes);
            else
                MiddleLockHeros1 = new ObservableCollection<Hero>(Constant.Heroes.Where(x => x.Label.Contains(MiddleSearchLockText1) || x.Title.Contains(MiddleSearchLockText1)));
        }

        private void MiddleSearchLockHero2()
        {
            MiddleLockHerosOpen2 = true;
            if (_preMiddleSearchLockText2 == MiddleSearchLockText2)
                return;
            _preMiddleSearchLockText2 = MiddleSearchLockText2;
            MiddleLockHero2 = null;
            if (string.IsNullOrEmpty(MiddleSearchLockText2))
                MiddleLockHeros2 = new ObservableCollection<Hero>(Constant.Heroes);
            else
                MiddleLockHeros2 = new ObservableCollection<Hero>(Constant.Heroes.Where(x => x.Label.Contains(MiddleSearchLockText2) || x.Title.Contains(MiddleSearchLockText2)));
        }

        private void BottomSearchLockHero1()
        {
            BottomLockHerosOpen1 = true;
            if (_preBottomSearchLockText1 == BottomSearchLockText1)
                return;
            _preBottomSearchLockText1 = BottomSearchLockText1;
            BottomLockHero1 = null;
            if (string.IsNullOrEmpty(BottomSearchLockText1))
                BottomLockHeros1 = new ObservableCollection<Hero>(Constant.Heroes);
            else
                BottomLockHeros1 = new ObservableCollection<Hero>(Constant.Heroes.Where(x => x.Label.Contains(BottomSearchLockText1) || x.Title.Contains(BottomSearchLockText1)));
        }

        private void BottomSearchLockHero2()
        {
            BottomLockHerosOpen2 = true;
            if (_preBottomSearchLockText2 == BottomSearchLockText2)
                return;
            _preBottomSearchLockText2 = BottomSearchLockText2;
            BottomLockHero2 = null;
            if (string.IsNullOrEmpty(BottomSearchLockText2))
                BottomLockHeros2 = new ObservableCollection<Hero>(Constant.Heroes);
            else
                BottomLockHeros2 = new ObservableCollection<Hero>(Constant.Heroes.Where(x => x.Label.Contains(BottomSearchLockText2) || x.Title.Contains(BottomSearchLockText2)));
        }

        private void UtilitySearchLockHero1()
        {
            UtilityLockHerosOpen1 = true;
            if (_preUtilitySearchLockText1 == UtilitySearchLockText1)
                return;
            _preUtilitySearchLockText1 = UtilitySearchLockText1;
            UtilityLockHero1 = null;
            if (string.IsNullOrEmpty(UtilitySearchLockText1))
                UtilityLockHeros1 = new ObservableCollection<Hero>(Constant.Heroes);
            else
                UtilityLockHeros1 = new ObservableCollection<Hero>(Constant.Heroes.Where(x => x.Label.Contains(UtilitySearchLockText1) || x.Title.Contains(UtilitySearchLockText1)));
        }

        private void UtilitySearchLockHero2()
        {
            UtilityLockHerosOpen2 = true;
            if (_preUtilitySearchLockText2 == UtilitySearchLockText2)
                return;
            _preUtilitySearchLockText2 = UtilitySearchLockText2;
            UtilityLockHero2 = null;
            if (string.IsNullOrEmpty(UtilitySearchLockText2))
                UtilityLockHeros2 = new ObservableCollection<Hero>(Constant.Heroes);
            else
                UtilityLockHeros2 = new ObservableCollection<Hero>(Constant.Heroes.Where(x => x.Label.Contains(UtilitySearchLockText2) || x.Title.Contains(UtilitySearchLockText2)));
        }

        public void StartGame()
        {
            try
            {
                var loc = _iniSettingsModel.GameExeLocation;
                if (string.IsNullOrEmpty(loc) || !File.Exists(loc))
                {
                    Growl.InfoGlobal(new GrowlInfo()
                    {
                        WaitTime = 2,
                        Message = "无法获取客户端位置,请确保已经获取游戏路径",
                        ShowDateTime = false
                    });

                    return;
                }

                using (Process p = new Process())
                {
                    p.StartInfo.FileName = loc;
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardInput = true;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.RedirectStandardError = true;
                    p.StartInfo.CreateNoWindow = true;
                    p.Start();
                    p.StandardInput.AutoFlush = true;
                    p.Close();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        private void SearchDisableHero()
        {
            DisableHerosOpen = true;
            if (_preSearchDisableText == SearchDisableText)
                return;
            _preSearchDisableText = SearchDisableText;
            DisableHero = null;
            if (string.IsNullOrEmpty(SearchDisableText))
                DisableHeros = new ObservableCollection<Hero>(Constant.Heroes);
            else
                DisableHeros = new ObservableCollection<Hero>(Constant.Heroes.Where(x => x.Label.Contains(SearchDisableText) || x.Title.Contains(SearchDisableText)));
        }

        private void SearchHeroForSkin()
        {
            ChooseHeroForSkinsOpen = true;
            if (_preSearchChooseHeroForSkinText == SearchChooseHeroForSkinText)
                return;
            _preSearchChooseHeroForSkinText = SearchChooseHeroForSkinText;
            ChooseHeroForSkin = null;
            if (string.IsNullOrEmpty(SearchChooseHeroForSkinText))
                ChooseHeroForSkins = new ObservableCollection<Hero>(Constant.Heroes);
            else
                ChooseHeroForSkins = new ObservableCollection<Hero>(Constant.Heroes.Where(x => x.Label.Contains(SearchChooseHeroForSkinText) || x.Title.Contains(SearchChooseHeroForSkinText)));
        }

        private async Task GetGameFolderAsync()
        {

            var location = (await _applicationService.GetInstallLocation())?.Replace("\"", string.Empty);
            if (string.IsNullOrEmpty(location))
            {
                Growl.InfoGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "无法获取客户端位置,确保游戏已经打开",
                    ShowDateTime = false
                });

                return;
            }
            var parent = Directory.GetParent(location);
            var files = parent.GetFiles("client.exe", SearchOption.AllDirectories);
            FileInfo fileinfo = null;
            if (files.Count() > 1)
            {
                fileinfo = files.FirstOrDefault(x => x.FullName.Contains("TCLS"));
            }
            else if (files.Count() == 1) fileinfo = files.FirstOrDefault();
            if (fileinfo != null)
            {
                GameStartupLocation = fileinfo.FullName;
                await _iniSettingsModel.WriteGameLocationAsync(GameStartupLocation);
            }
        }

        private async Task ManualGetGameFolderAsync()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            var result = openFileDialog.ShowDialog();
            if (result != null && result.Value)
            {
                GameStartupLocation = openFileDialog.FileName;
                await _iniSettingsModel.WriteGameLocationAsync(GameStartupLocation);
            }
        }

        private async Task ExitGameAsync()
        {
            await _applicationService.QuitAsync();
        }
        private async Task ModifyRankLevelAsync()
        {
            var settings = RankSetting.Split(",");
            if (settings.Length != 3 || settings.Any(x => string.IsNullOrEmpty(x)))
            {
                Growl.InfoGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "非法的段位修改字符串",
                    ShowDateTime = false
                });
                return;
            }

            await _applicationService.SetRankAsync(new
            {
                lol = new
                {
                    rankedLeagueQueue = settings[0],
                    rankedLeagueTier = settings[1],
                    rankedLeagueDivision = settings[2]
                }
            });

            await _iniSettingsModel.WriteRankSettingAsync(RankSetting);
        }

        private async Task SearchSkinsForHeroAsync()
        {
            if (ChooseHeroForSkin == null)
                return;

            var _skinsWindow = App.ServiceProvider.GetRequiredService<SkinsWindow>();
            await (_skinsWindow.DataContext as SkinsWindowViewModel).LoadSkinsAsync(ChooseHeroForSkin);
            _skinsWindow.ShowDialog();
        }

        private void OpenAramChoose()
        {
            var _window = App.ServiceProvider.GetRequiredService<AramQuickChoose>();
            _window.ShowDialog();
        }

        private void OpenBlackRecord()
        {
            var _window = App.ServiceProvider.GetRequiredService<BlackRecord>();
            (_window.DataContext as BlackRecordViewModel).Load();
            _window.ShowDialog();
        }

        private async Task FetchRunesAndItemsAsync()
        {
            using (var client = new HttpClient())
            {
                string extension = "zip";
                var temp = Path.GetTempPath();
                var fileLoc = Path.Combine(temp, $"{Guid.NewGuid()}.{extension}");
                var loc = _iconfiguration.GetSection("HeroDatas").Value;
                if (string.IsNullOrEmpty(loc))
                {
                    Growl.InfoGlobal(new GrowlInfo()
                    {
                        WaitTime = 2,
                        Message = "符文与装备拉取地址为空",
                        ShowDateTime = false
                    });

                    return;
                }
                using (var stream = await client.GetStreamAsync(loc))
                {
                    try
                    {
                        using (FileStream fileStream = new FileStream(fileLoc, FileMode.CreateNew))
                        {
                            byte[] buffer = new byte[1024 * 1024 * 2];
                            int readLength = 0;
                            int length;
                            while ((length = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                            {
                                readLength += length;
                                fileStream.Write(buffer, 0, length);
                            }
                        }

                        ZipFile.ExtractToDirectory(fileLoc, AppDomain.CurrentDomain.BaseDirectory, true);
                        Growl.SuccessGlobal(new GrowlInfo()
                        {
                            WaitTime = 2,
                            Message = "符文装备更新成功!",
                            ShowDateTime = false
                        });
                    }
                    catch (Exception ex)
                    {
                        Growl.InfoGlobal(new GrowlInfo()
                        {
                            WaitTime = 2,
                            Message = "符文装备更新失败!" + ex.Message,
                            ShowDateTime = false
                        });
                    }
                    finally
                    {
                        if (File.Exists(fileLoc))
                            File.Delete(fileLoc);
                    }
                }
            }
        }

        private async Task ChooseLightThemeAsync()
        {
            await _iniSettingsModel.WriteIsDarkTheme(false);
            IsDarkTheme = false;
            App.ChangeTheme(App.Theme.Light);
        }

        private async Task ChooseDarkThemeAsync()
        {
            await _iniSettingsModel.WriteIsDarkTheme(true);
            IsDarkTheme = true;
            App.ChangeTheme(App.Theme.Dark);
        }

        private async Task ManualUpdateAsync()
        {
            await _softwareHelper.Update();
        }

        private async Task SaveFuckWordsAsync()
        {
            try
            {
                await _iniSettingsModel.WriteFuckWords(FuckWords);
                Growl.SuccessGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "保存成功",
                    ShowDateTime = false
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Growl.WarningGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "保存失败",
                    ShowDateTime = false
                });
            }
        }

        private async Task SaveGoodWordsAsync()
        {
            try
            {
                await _iniSettingsModel.WriteGoodWords(GoodWords);
                Growl.SuccessGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "保存成功",
                    ShowDateTime = false
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Growl.WarningGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "保存失败",
                    ShowDateTime = false
                });
            }
        }

        private async Task SettingSignature() 
        {
            await _applicationService.SetSignatureAsync(new 
            {
                statusMessage = Signature
            });
        }

        private async Task SaveTeamDetailKey()
        {
            var selectKeyWindows = serviceProvider.GetRequiredService<SelectKey>();
            IsOpenModifyHotkeys = true;
            try
            {
                if (selectKeyWindows.ShowDialog() == true)
                {
                    await _iniSettingsModel.WriteTeamDetailKeys(selectKeyWindows.SelectKeys);
                    TeamDetailKeys = selectKeyWindows.SelectKeys;
                }
            }
            finally
            {
                IsOpenModifyHotkeys = false;
            }
        }
    }
}
