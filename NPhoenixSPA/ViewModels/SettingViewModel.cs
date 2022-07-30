using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using HandyControl.Data;
using LeagueOfLegendsBoxer.Application.ApplicationControl;
using LeagueOfLegendsBoxer.Application.Game;
using Microsoft.Extensions.DependencyInjection;
using NPhoenixSPA.Models;
using NPhoenixSPA.Resources;
using NPhoenixSPA.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace NPhoenixSPA.ViewModels
{
    public class SettingViewModel : ObservableObject
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

        private ObservableCollection<Hero> _quickChooseHeros;
        public ObservableCollection<Hero> QuickChooseHeros
        {
            get => _quickChooseHeros;
            set => SetProperty(ref _quickChooseHeros, value);
        }

        private ObservableCollection<Hero> _subQuickChooseHeros;
        public ObservableCollection<Hero> SubQuickChooseHeros
        {
            get => _subQuickChooseHeros;
            set => SetProperty(ref _subQuickChooseHeros, value);
        }

        private ObservableCollection<Hero> _selectedQuickChooseHeros;
        public ObservableCollection<Hero> SelectedQuickChooseHeros
        {
            get => _selectedQuickChooseHeros;
            set => SetProperty(ref _selectedQuickChooseHeros, value);
        }

        private ObservableCollection<Hero> _subSelectedQuickChooseHeros;
        public ObservableCollection<Hero> SubSelectedQuickChooseHeros
        {
            get => _subSelectedQuickChooseHeros;
            set => SetProperty(ref _subSelectedQuickChooseHeros, value);
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
        public AsyncRelayCommand CheckedAutoAcceptCommandAsync { get; set; }
        public AsyncRelayCommand UncheckedAutoAcceptCommandAsync { get; set; }
        public AsyncRelayCommand CheckedAutoLockHeroCommandAsync { get; set; }
        public AsyncRelayCommand UncheckedAutoLockHeroCommandAsync { get; set; }
        public AsyncRelayCommand CheckedAutoDisableHeroCommandAsync { get; set; }
        public AsyncRelayCommand UncheckedAutoDisableHeroCommandAsync { get; set; }
        public AsyncRelayCommand CheckedAutoLockHeroInAramCommandAsync { get; set; }
        public AsyncRelayCommand UncheckedAutoLockHeroInAramCommandAsync { get; set; }
        public AsyncRelayCommand SelectHerosLockCommandAsync { get; set; }
        public AsyncRelayCommand UnSelectHerosLockCommandAsync { get; set; }
        public AsyncRelayCommand GetGameFolderCommandAsync { get; set; }
        public AsyncRelayCommand ExitGameCommandAsync { get; set; }
        public AsyncRelayCommand LoadCommandAsync { get; set; }
        public RelayCommand SearchLockHeroCommand { get; set; }
        public RelayCommand SearchDisableHeroCommand { get; set; }
        public RelayCommand SearchHeroForSkinCommand { get; set; }
        public AsyncRelayCommand ModifyRankLevelCommandAsync { get; set; }
        public AsyncRelayCommand SearchSkinsForHeroCommandAsync { get; set; }
        public AsyncRelayCommand FetchRunesCommandAsync { get; set; }

        private readonly IniSettingsModel _iniSettingsModel;
        private readonly IGameService _gameService;
        private readonly IApplicationService _applicationService;
        public SettingViewModel(IniSettingsModel iniSettingsModel, IApplicationService applicationService, IGameService gameService)
        {
            _iniSettingsModel = iniSettingsModel;
            _applicationService = applicationService;
            _gameService = gameService;
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
            SelectHerosLockCommandAsync = new AsyncRelayCommand(SelectHerosLockAsync);
            UnSelectHerosLockCommandAsync = new AsyncRelayCommand(UnSelectHerosLockAsync);
            GetGameFolderCommandAsync = new AsyncRelayCommand(GetGameFolderAsync);
            ExitGameCommandAsync = new AsyncRelayCommand(ExitGameAsync);
            ModifyRankLevelCommandAsync = new AsyncRelayCommand(ModifyRankLevelAsync);
            SearchHeroForSkinCommand = new RelayCommand(SearchHeroForSkin);
            FetchRunesCommandAsync = new AsyncRelayCommand(FetchRunesAsync);
            SearchSkinsForHeroCommandAsync = new AsyncRelayCommand(SearchSkinsForHeroAsync);
        }

        private async Task LoadAsync()
        {
            LockHeros = new ObservableCollection<Hero>(Constant.Heroes);
            DisableHeros = new ObservableCollection<Hero>(Constant.Heroes);
            ChooseHeroForSkins = new ObservableCollection<Hero>(Constant.Heroes);
            QuickChooseHeros = new ObservableCollection<Hero>(Constant.Heroes.Where(x => !_iniSettingsModel.LockHerosInAram.Contains(x.ChampId)).OrderBy(x => x.Name));
            SubQuickChooseHeros = new ObservableCollection<Hero>();
            SubSelectedQuickChooseHeros = new ObservableCollection<Hero>();
            var list = new List<Hero>();
            foreach (var item in _iniSettingsModel.LockHerosInAram)
            {
                var h = Constant.Heroes.FirstOrDefault(x => x.ChampId == item);
                if (h != null)
                {
                    list.Add(h);
                }
            }
            SelectedQuickChooseHeros = new ObservableCollection<Hero>(list);
            AutoAcceptGame = _iniSettingsModel.AutoAcceptGame;
            AutoDisableHero = _iniSettingsModel.AutoDisableHero;
            AutoLockHero = _iniSettingsModel.AutoLockHero;
            AutoLockHeroInAram = _iniSettingsModel.AutoLockHeroInAram;
            DisableHero = DisableHeros.FirstOrDefault(x => x.ChampId == _iniSettingsModel.AutoDisableChampId);
            LockHero = LockHeros.FirstOrDefault(x => x.ChampId == _iniSettingsModel.AutoLockHeroChampId);
            GameStartupLocation = _iniSettingsModel.GameExeLocation;
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
        #endregion

        private async Task SelectHerosLockAsync()
        {
            if (SubQuickChooseHeros.Count <= 0)
                return;

            if (SubQuickChooseHeros.Count + SelectedQuickChooseHeros.Count > 20)
            {
                Growl.InfoGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "乱斗模式秒选最多设置20位英雄",
                    ShowDateTime = false
                });

                return;
            }
            var temp = new List<Hero>();
            foreach (var item in SubQuickChooseHeros)
            {
                temp.Add(item);
            }

            foreach (var item in temp)
            {
                QuickChooseHeros.Remove(item);
                SelectedQuickChooseHeros.Add(item);
            }

            await _iniSettingsModel.WriteLockHerosInAramAsync(SelectedQuickChooseHeros.Select(x => x.ChampId).ToList());
            SubQuickChooseHeros.Clear();
        }

        private async Task UnSelectHerosLockAsync()
        {
            if (SubSelectedQuickChooseHeros.Count <= 0)
                return;

            var temp = new List<Hero>();
            foreach (var item in SubSelectedQuickChooseHeros)
            {
                temp.Add(item);
            }

            foreach (var item in temp)
            {
                QuickChooseHeros.Add(item);
                SelectedQuickChooseHeros.Remove(item);
            }
            QuickChooseHeros = new ObservableCollection<Hero>(QuickChooseHeros.OrderBy(x => x.Name));
            await _iniSettingsModel.WriteLockHerosInAramAsync(SelectedQuickChooseHeros.Select(x => x.ChampId).ToList());
            SubSelectedQuickChooseHeros.Clear();
        }

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

            var location = (await _applicationService.GetInstallLocation()).Replace("\"", string.Empty);
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

        private async Task FetchRunesAsync()
        {
            using (var client = new HttpClient())
            {
                string extension = "zip";
                var temp = Path.GetTempPath();
                var fileLoc = Path.Combine(temp, $"{Guid.NewGuid()}.{extension}");
                using (var stream = await client.GetStreamAsync("https://files.cnblogs.com/files/qwqwQAQ/heroDatas.zip"))
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
                            Message = "符文更新成功!",
                            ShowDateTime = false
                        });
                    }
                    catch (Exception ex)
                    {
                        Growl.InfoGlobal(new GrowlInfo()
                        {
                            WaitTime = 2,
                            Message = "符文更新失败!" + ex.Message,
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
        public BitmapImage ByteArrayToBitmapImage(byte[] array)
        {
            using (var ms = new MemoryStream(array))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad; // here
                image.StreamSource = ms;
                image.EndInit();
                image.Freeze();
                return image;
            }
        }
    }
}
