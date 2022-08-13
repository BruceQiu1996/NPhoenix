using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using HandyControl.Data;
using LeagueOfLegendsBoxer.Application.ApplicationControl;
using LeagueOfLegendsBoxer.Models;
using LeagueOfLegendsBoxer.Resources;
using LeagueOfLegendsBoxer.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace LeagueOfLegendsBoxer.ViewModels.Pages
{
    public class SettingsViewModel : ObservableObject
    {
        private bool _autoAcceptGame;
        public bool AutoAcceptGame
        {
            get => _autoAcceptGame;
            set => SetProperty(ref _autoAcceptGame, value);
        }

        private bool _autoLockHero;
        public bool AutoLockHero
        {
            get => _autoLockHero;
            set => SetProperty(ref _autoLockHero, value);
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

        private Hero _lockHero;
        public Hero LockHero
        {
            get => _lockHero;
            set
            {
                SetProperty(ref _lockHero, value);
                _iniSettingsModel.WriteAutoLockHeroIdAsync(value == null ? 0 : value.ChampId).GetAwaiter().GetResult();
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
                _iniSettingsModel.WriteAutoDisableHeroIdAsync(value == null ? 0 : value.ChampId).GetAwaiter().GetResult();
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

        
        public AsyncRelayCommand CheckedAutoAcceptCommandAsync { get; set; }
        public AsyncRelayCommand UncheckedAutoAcceptCommandAsync { get; set; }
        public AsyncRelayCommand CheckedAutoLockHeroCommandAsync { get; set; }
        public AsyncRelayCommand UncheckedAutoLockHeroCommandAsync { get; set; }
        public AsyncRelayCommand CheckedAutoDisableHeroCommandAsync { get; set; }
        public AsyncRelayCommand UncheckedAutoDisableHeroCommandAsync { get; set; }
        public AsyncRelayCommand CheckedAutoLockHeroInAramCommandAsync { get; set; }
        public AsyncRelayCommand UncheckedAutoLockHeroInAramCommandAsync { get; set; }
        public AsyncRelayCommand GetGameFolderCommandAsync { get; set; }
        public AsyncRelayCommand ExitGameCommandAsync { get; set; }
        public AsyncRelayCommand LoadCommandAsync { get; set; }
        public RelayCommand SearchLockHeroCommand { get; set; }
        public RelayCommand SearchDisableHeroCommand { get; set; }
        public RelayCommand SearchHeroForSkinCommand { get; set; }
        public AsyncRelayCommand ModifyRankLevelCommandAsync { get; set; }
        public AsyncRelayCommand SearchSkinsForHeroCommandAsync { get; set; }
        public AsyncRelayCommand FetchRunesAndItemsCommandAsync { get; set; }
        public RelayCommand OpenAramChooseCommand { get; set; }
        public RelayCommand StartGameCommand { get; set; }
        public AsyncRelayCommand CheckedCloseRecommmandCommandAsync { get; set; }
        public AsyncRelayCommand UnCheckedCloseRecommmandCommandAsync { get; set; }
        public AsyncRelayCommand CheckedCloseSendOtherWhenBeginCommandAsync { get; set; }
        public AsyncRelayCommand UnCheckedCloseSendOtherWhenBeginCommandAsync { get; set; }
        public AsyncRelayCommand SaveHorseTemplateCommandAsync { get; set; }
        public RelayCommand OpenBlackRecordCommand { get; set; }
        public AsyncRelayCommand AutoAcceptGameDelayChangedCommandAsync { get; set; }
        public AsyncRelayCommand CheckUseAltQOpenVsDetailCommandAsync { get; set; }
        public AsyncRelayCommand UnCheckUseAltQOpenVsDetailCommandAsync { get; set; }

        private readonly IniSettingsModel _iniSettingsModel;
        private readonly IApplicationService _applicationService;
        private readonly IConfiguration _iconfiguration;
        private readonly ILogger<SettingsViewModel> _logger;
        public SettingsViewModel(IniSettingsModel iniSettingsModel, IApplicationService applicationService, IConfiguration iconfiguration, ILogger<SettingsViewModel> logger)
        {
            _iniSettingsModel = iniSettingsModel;
            _applicationService = applicationService;
            _iconfiguration = iconfiguration;
            _logger = logger;
            CheckedAutoAcceptCommandAsync = new AsyncRelayCommand(CheckedAutoAcceptAsync);
            UncheckedAutoAcceptCommandAsync = new AsyncRelayCommand(UncheckedAutoAcceptAsync);
            CheckedAutoLockHeroCommandAsync = new AsyncRelayCommand(CheckedAutoLockHeroAsync);
            UncheckedAutoLockHeroCommandAsync = new AsyncRelayCommand(UncheckedAutoLockHeroAsync);
            CheckedAutoDisableHeroCommandAsync = new AsyncRelayCommand(CheckedAutoDisableHeroAsync);
            UncheckedAutoDisableHeroCommandAsync = new AsyncRelayCommand(UncheckedAutoDisableHeroAsync);
            CheckedAutoLockHeroInAramCommandAsync = new AsyncRelayCommand(CheckedAutoLockHeroInAramAsync);
            UncheckedAutoLockHeroInAramCommandAsync = new AsyncRelayCommand(UncheckedAutoLockHeroInAramAsync);
            LoadCommandAsync = new AsyncRelayCommand(LoadAsync);
            SearchLockHeroCommand = new RelayCommand(SearchLockHero);
            SearchDisableHeroCommand = new RelayCommand(SearchDisableHero);
            GetGameFolderCommandAsync = new AsyncRelayCommand(GetGameFolderAsync);
            ExitGameCommandAsync = new AsyncRelayCommand(ExitGameAsync);
            ModifyRankLevelCommandAsync = new AsyncRelayCommand(ModifyRankLevelAsync);
            SearchHeroForSkinCommand = new RelayCommand(SearchHeroForSkin);
            FetchRunesAndItemsCommandAsync = new AsyncRelayCommand(FetchRunesAndItemsAsync);
            SearchSkinsForHeroCommandAsync = new AsyncRelayCommand(SearchSkinsForHeroAsync);
            OpenAramChooseCommand = new RelayCommand(OpenAramChoose);
            StartGameCommand = new RelayCommand(StartGame);
            CheckedCloseRecommmandCommandAsync = new AsyncRelayCommand(CheckedCloseRecommmandAsync);
            UnCheckedCloseRecommmandCommandAsync = new AsyncRelayCommand(UnCheckedCloseRecommmandAsync);
            CheckedCloseSendOtherWhenBeginCommandAsync = new AsyncRelayCommand(CheckedCloseSendOtherWhenBeginAsync);
            UnCheckedCloseSendOtherWhenBeginCommandAsync = new AsyncRelayCommand(UnCheckedCloseSendOtherWhenBeginAsync);
            SaveHorseTemplateCommandAsync = new AsyncRelayCommand(SaveHorseTemplateAsync);
            OpenBlackRecordCommand = new RelayCommand(OpenBlackRecord);
            AutoAcceptGameDelayChangedCommandAsync = new AsyncRelayCommand(AutoAcceptGameDelayChangedAsync);
            CheckUseAltQOpenVsDetailCommandAsync = new AsyncRelayCommand(CheckUseAltQOpenVsDetailAsync);
            UnCheckUseAltQOpenVsDetailCommandAsync = new AsyncRelayCommand(UnCheckUseAltQOpenVsDetailAsync);
        }

        private async Task LoadAsync()
        {
            LockHeros = new ObservableCollection<Hero>(Constant.Heroes);
            DisableHeros = new ObservableCollection<Hero>(Constant.Heroes);
            ChooseHeroForSkins = new ObservableCollection<Hero>(Constant.Heroes);
            AutoAcceptGame = _iniSettingsModel.AutoAcceptGame;
            AutoDisableHero = _iniSettingsModel.AutoDisableHero;
            AutoLockHero = _iniSettingsModel.AutoLockHero;
            AutoLockHeroInAram = _iniSettingsModel.AutoLockHeroInAram;
            DisableHero = DisableHeros.FirstOrDefault(x => x.ChampId == _iniSettingsModel.AutoDisableChampId);
            LockHero = LockHeros.FirstOrDefault(x => x.ChampId == _iniSettingsModel.AutoLockHeroChampId);
            GameStartupLocation = _iniSettingsModel.GameExeLocation;
            IsCloseRecommmand = _iniSettingsModel.IsCloseRecommand;
            CloseSendOtherWhenBegin = _iniSettingsModel.CloseSendOtherWhenBegin;
            HorseTemplate = _iniSettingsModel.HorseTemplate;
            Version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            AutoAcceptGameDelay = _iniSettingsModel.AutoAcceptGameDelay;
            Above120ScoreTxt = _iniSettingsModel.Above120ScoreTxt;
            Above110ScoreTxt = _iniSettingsModel.Above110ScoreTxt;
            Above100ScoreTxt = _iniSettingsModel.Above100ScoreTxt;
            Below100ScoreTxt = _iniSettingsModel.Below100ScoreTxt;
            IsAltQOpenVsDetail = _iniSettingsModel.IsAltQOpenVsDetail;
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
        private async Task CheckedCloseRecommmandAsync()
        {
            await _iniSettingsModel.WriteIsCloseRecommandAsync(true);
            IsCloseRecommmand = true;
        }
        private async Task UnCheckedCloseRecommmandAsync()
        {
            await _iniSettingsModel.WriteIsCloseRecommandAsync(false);
            IsCloseRecommmand = false;
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

        private void StartGame()
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
    }
}
