using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using HandyControl.Data;
using LeagueOfLegendsBoxer.Application.ApplicationControl;
using LeagueOfLegendsBoxer.Application.Game;
using LeagueOfLegendsBoxer.Helpers;
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
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LeagueOfLegendsBoxer.ViewModels.Pages
{
    public class RuneAndItemViewModel : ObservableObject
    {
        private readonly RuneHelper _runeHelper;
        private readonly IGameService _gameService;
        private readonly IniSettingsModel _iniSettingsModel;
        private int priviouschampId;
        private int currentchampId;
        private HeroRecommandModule _recommandModule;
        //private IEnumerable<RuneModule> _customerRuneModules;

        private Hero _hero;
        public Hero Hero
        {
            get { return _hero; }
            set { SetProperty(ref _hero, value); }

        }

        /// <summary>
        /// 炫彩皮肤
        /// </summary>
        private ObservableCollection<Skin> _colorSkins;
        public ObservableCollection<Skin> ColorSkins
        {
            get { return _colorSkins; }
            set { SetProperty(ref _colorSkins, value); }

        }

        private ObservableCollection<RuneModule> _aramRunes;
        public ObservableCollection<RuneModule> AramRunes
        {
            get { return _aramRunes; }
            set { SetProperty(ref _aramRunes, value); }

        }

        private ObservableCollection<RuneModule> _commonRunes;
        public ObservableCollection<RuneModule> CommonRunes
        {
            get { return _commonRunes; }
            set { SetProperty(ref _commonRunes, value); }

        }

        private ObservableCollection<RuneModule> _displayRunes;
        public ObservableCollection<RuneModule> DisplayRunes
        {
            get { return _displayRunes; }
            set { SetProperty(ref _displayRunes, value); }
        }

        private ObservableCollection<ItemModule> _aramItems;
        public ObservableCollection<ItemModule> AramItems
        {
            get { return _aramItems; }
            set { SetProperty(ref _aramItems, value); }

        }

        private ObservableCollection<ItemModule> _commonItems;
        public ObservableCollection<ItemModule> CommonItems
        {
            get { return _commonItems; }
            set { SetProperty(ref _commonItems, value); }

        }

        private ObservableCollection<ItemModule> _displayItems;
        public ObservableCollection<ItemModule> DisplayItems
        {
            get { return _displayItems; }
            set { SetProperty(ref _displayItems, value); }
        }

        private bool _autoLockHeroInAram;
        public bool AutoLockHeroInAram
        {
            get => _autoLockHeroInAram;
            set => SetProperty(ref _autoLockHeroInAram, value);
        }

        public AsyncRelayCommand LoadCommandAsync { get; set; }
        public RelayCommand SwitchRuneToCommonCommand { get; set; }
        public RelayCommand SwitchRuneToAramCommand { get; set; }
        public RelayCommand SwitchItemPageCommand { get; set; }
        public RelayCommand SwitchRunePageCommand { get; set; }
        public AsyncRelayCommand<RuneModule> SetRuneCommandAsync { get; set; }
        public AsyncRelayCommand<ItemModule> ApplyItemCommandAsync { get; set; }
        public AsyncRelayCommand<RuneModule> AutoApplyRuneCommandAsync { get; set; }
        public AsyncRelayCommand<RuneModule> UnAutoApplyRuneCommandAsync { get; set; }
        public AsyncRelayCommand CheckedAutoLockHeroInAramCommandAsync { get; set; }
        public AsyncRelayCommand UncheckedAutoLockHeroInAramCommandAsync { get; set; }

        private readonly IApplicationService _applicationService;
        private readonly ILogger<RuneAndItemViewModel> _logger;
        public RuneAndItemViewModel(RuneHelper runeHelper,
                                    IGameService gameService,
                                    IApplicationService applicationService,
                                    ILogger<RuneAndItemViewModel> logger,
                                    IniSettingsModel iniSettingsModel)
        {
            _runeHelper = runeHelper;
            _gameService = gameService;
            SwitchRuneToCommonCommand = new RelayCommand(SwitchRuneToCommon);
            SwitchRuneToAramCommand = new RelayCommand(SwitchRuneToAram);
            SetRuneCommandAsync = new AsyncRelayCommand<RuneModule>(SetRuneAsync);
            AutoApplyRuneCommandAsync = new AsyncRelayCommand<RuneModule>(AutoApplyRuneAsync);
            UnAutoApplyRuneCommandAsync = new AsyncRelayCommand<RuneModule>(UnAutoApplyRuneAsync);
            CheckedAutoLockHeroInAramCommandAsync = new AsyncRelayCommand(CheckedAutoLockHeroInAramAsync);
            UncheckedAutoLockHeroInAramCommandAsync = new AsyncRelayCommand(UncheckedAutoLockHeroInAramAsync);
            SwitchItemPageCommand = new RelayCommand(() => IsRunePage = false);
            SwitchRunePageCommand = new RelayCommand(() => IsRunePage = true);
            _iniSettingsModel = iniSettingsModel;
            _logger = logger;
            _applicationService = applicationService;
            ColorSkins = new ObservableCollection<Skin>();
            ApplyItemCommandAsync = new AsyncRelayCommand<ItemModule>(ApplyItemAsync);
        }

        private bool _isAramPage;
        public bool IsAramPage
        {
            get { return _isAramPage; }
            set { SetProperty(ref _isAramPage, value); }

        }

        private bool _isRunePage;
        public bool IsRunePage
        {
            get { return _isRunePage; }
            set { SetProperty(ref _isRunePage, value); }

        }

        public async Task GetCurrentChampionColorSkins()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                ColorSkins.Clear();
            });
            string resp = await _gameService.GetCurrentChampionColorSkinsAsync();
            if (string.IsNullOrEmpty(resp))
            {
                return;
            }

            JArray jsonArray = JArray.Parse(resp);
            if (jsonArray == null || jsonArray.Count <= 0)
            {
                return;
            }

            var championId = jsonArray[0].Value<int>("championId");
            var skins = await LoadSkinsAsync(championId);
            if (skins == null)
            {
                return;
            }
            foreach (var item in jsonArray)
            {
                var array = item["childSkins"].ToArray();
                foreach (var item2 in array)
                {
                    var unlocked = item2["unlocked"].Value<bool>();
                    if (unlocked)
                    {
                        foreach (var skin in skins)
                        {
                            if (skin.Chromas != null)
                            {
                                var innerSkin = skin.Chromas.FirstOrDefault(x => x.Id == item2["id"]?.Value<int>());
                                if (innerSkin != null)
                                {
                                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        ColorSkins.Add(innerSkin);
                                    });
                                }
                            }
                        }
                    }
                }
            }
        }

        public async Task<IEnumerable<Skin>> LoadSkinsAsync(int champId)
        {
            try
            {
                var result = await _gameService.GetSkinsByHeroId(champId);
                if (!string.IsNullOrEmpty(result))
                {
                    var skins = JToken.Parse(result)["skins"].ToObject<IEnumerable<Skin>>();

                    return skins;
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task LoadChampInfoAsync(int champId, bool isAram, bool needFilterSame = true)
        {
            AutoLockHeroInAram = _iniSettingsModel.AutoLockHeroInAram;
            priviouschampId = currentchampId;
            currentchampId = champId;
            if (priviouschampId == currentchampId && needFilterSame)
                return;

            var data = await _gameService.GetRuneItemsFromOnlineAsync(champId);
            if (data == null) return;
            var module = JsonConvert.DeserializeObject<HeroRecommandModule>(data);
            var customerRunes = await _runeHelper.ReadCustomerRuneAsync(champId);
            _recommandModule = module;
            //_customerRuneModules = customerRunes;
            //if (_customerRuneModules == null)
            //{
            //    _customerRuneModules = new List<RuneModule>();
            //}
            Hero = Constant.Heroes?.FirstOrDefault(x => x.ChampId == champId);
            AramRunes = new ObservableCollection<RuneModule>(module.Perk.Where(x => x.GameType == 2).OrderByDescending(x => x.Showrate));
            CommonRunes = new ObservableCollection<RuneModule>(module.Perk.Where(x => x.GameType == 1)
                .OrderBy(x => x.Lane).ThenByDescending(x => x.Showrate));
            //foreach (var rune in _customerRuneModules.Where(x => x.GameType == 2).Reverse())
            //{
            //    rune.IsCustomer = true;
            //    AramRunes.Insert(0, rune);
            //}

            //foreach (var rune in _customerRuneModules.Where(x => x.GameType == 1).Reverse())
            //{
            //    rune.IsCustomer = true;
            //    CommonRunes.Insert(0, rune);
            //}

            var aramAutoRuneId = await _iniSettingsModel.ReadAutoAramRuneItem(champId);
            RuneModule tempAram = null;
            if (aramAutoRuneId != null && (tempAram = AramRunes.FirstOrDefault(x => x.Id == aramAutoRuneId)) != null)
            {
                tempAram.IsAutoApply = true;
            }

            var commonAutoRuneId = await _iniSettingsModel.ReadAutoCommonRuneItem(champId);
            RuneModule tempCommon = null;
            if (commonAutoRuneId != null && (tempCommon = CommonRunes.FirstOrDefault(x => x.Id == commonAutoRuneId)) != null)
            {
                tempCommon.IsAutoApply = true;
            }

            //判断是否存在自动应用符文
            if (isAram && AramRunes.Any(x => x.IsAutoApply))
            {
                var rune = AramRunes.FirstOrDefault(x => x.IsAutoApply);
                await SetRuneAsyncMethod(rune, true);
            }
            else if (!isAram && CommonRunes.Any(x => x.IsAutoApply))
            {
                var rune = CommonRunes.FirstOrDefault(x => x.IsAutoApply);
                await SetRuneAsyncMethod(rune, true);
            }
            else if (_iniSettingsModel.AutoUseRune && _iniSettingsModel.AutoUseRuneByUseCount)
            {
                if (isAram)
                {
                    var rune = AramRunes.OrderByDescending(x => x.Showrate).FirstOrDefault();
                    await SetRuneAsyncMethod(rune, true);
                }
                else
                {
                    var rune = CommonRunes.OrderByDescending(x => x.Showrate).FirstOrDefault();
                    await SetRuneAsyncMethod(rune, true);
                }
            }
            else if (_iniSettingsModel.AutoUseRune && _iniSettingsModel.AutoUseRuneByWinRate)
            {
                if (isAram)
                {
                    var rune = AramRunes.OrderByDescending(x => x.Winrate).FirstOrDefault();
                    await SetRuneAsyncMethod(rune, true);
                }
                else
                {
                    var rune = CommonRunes.OrderByDescending(x => x.Winrate).FirstOrDefault();
                    await SetRuneAsyncMethod(rune, true);
                }
            }

            DisplayRunes = isAram ? AramRunes : CommonRunes;
            AramItems = new ObservableCollection<ItemModule>(module.Equip.Where(x => x.Map_id == 12).OrderByDescending(x => x.Showrate));
            CommonItems = new ObservableCollection<ItemModule>(module.Equip.Where(x => x.Map_id == 11).OrderByDescending(x => x.Showrate));
            DisplayItems = isAram ? AramItems : CommonItems;
            IsAramPage = isAram;
            IsRunePage = true;
            App.ServiceProvider.GetRequiredService<ChampionSelectTool>().Show();
            (App.ServiceProvider.GetRequiredService<ChampionSelectTool>().DataContext as ChampionSelectToolViewModel).ShowRunePage();
        }

        private async Task SetRuneAsync(RuneModule RuneModule)
        {
            await SetRuneAsyncMethod(RuneModule);
        }

        private async Task SetRuneAsyncMethod(RuneModule RuneModule, bool isAuto = false)
        {
            var result = await _gameService.GetAllRunePages();
            var array = JsonConvert.DeserializeObject<IEnumerable<RuneModel>>(result);
            var current = array.FirstOrDefault(x => x.IsDeletable);
            if (current != null) await _gameService.DeleteRunePage(current.Id);
            var mode = IsAramPage ? "大乱斗" : "峡谷5v5";
            var creation = new CreateRuneModel()
            {
                primaryStyleId = RuneModule.PrimaryStyleId,
                subStyleId = RuneModule.SubStyleId,
                name = $"{Hero.Name} - {mode}符文",
                current = true,
                selectedPerkIds = RuneModule.SelectedPerkIds
            };
            await _gameService.AddRunePage(creation);
            if (isAuto)
            {
                Growl.SuccessGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "自动配置符文成功",
                    ShowDateTime = false
                });
            }
            else
            {
                Growl.SuccessGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "符文设置成功",
                    ShowDateTime = false
                });
            }
        }

        private async Task AutoApplyRuneAsync(RuneModule runeModule)
        {
            if (IsAramPage)
            {
                await _iniSettingsModel.WriteAutoAramRuneItem(runeModule.Champion_id, runeModule.Id.ToString());
            }
            else
            {
                await _iniSettingsModel.WriteAutoCommonRuneItem(runeModule.Champion_id, runeModule.Id.ToString());
            }

            await LoadChampInfoAsync(currentchampId, IsAramPage, false);
        }

        private async Task UnAutoApplyRuneAsync(RuneModule runeModule)
        {
            if (IsAramPage)
            {
                await _iniSettingsModel.WriteAutoAramRuneItem(runeModule.Champion_id, null);
            }
            else
            {
                await _iniSettingsModel.WriteAutoCommonRuneItem(runeModule.Champion_id, null);
            }

            await LoadChampInfoAsync(currentchampId, IsAramPage, false);
        }

        public void Clear()
        {
            priviouschampId = 0;
            currentchampId = 0;
        }

        private async Task ApplyItemAsync(ItemModule itemModule)
        {
            var location = (await _applicationService.GetInstallLocation())?.Replace("\"", string.Empty);
            if (string.IsNullOrEmpty(location))
            {
                Growl.WarningGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "获取不到客户端所在位置",
                    ShowDateTime = false
                });

                return;
            }

            var parent = Directory.GetParent(location);
            var path = Path.Combine(parent.FullName, "Game\\Config\\Global\\Recommended");
            if (!Directory.Exists(path))
            {
                Growl.WarningGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "无法获取装备推荐设置目录",
                    ShowDateTime = false
                });

                return;
            }

            try
            {
                var dirinfo = new DirectoryInfo(path);
                RecommandItem recommandItem = new RecommandItem();
                recommandItem.champion = Hero.Alias;
                var map = IsAramPage ? 12 : 11;
                recommandItem.associatedMaps = new int[] { map };
                if (itemModule.Item1s != null && itemModule.Item1s.Count() > 0)
                {
                    var block = new Block();
                    block.type = "起始装备";
                    foreach (var item in itemModule.Item1s)
                    {
                        block.items.Add(new RItem()
                        {
                            id = item.Id.ToString()
                        });
                    }

                    recommandItem.blocks.Add(block);
                }
                if (itemModule.Item2s != null && itemModule.Item2s.Count() > 0)
                {
                    var block = new Block();
                    block.type = "核心装备";
                    foreach (var item in itemModule.Item2s)
                    {
                        block.items.Add(new RItem()
                        {
                            id = item.Id.ToString()
                        });
                    }

                    recommandItem.blocks.Add(block);
                }
                if (itemModule.Item3s != null && itemModule.Item3s.Count() > 0)
                {
                    var block = new Block();
                    block.type = "神装";
                    foreach (var item in itemModule.Item3s)
                    {
                        block.items.Add(new RItem()
                        {
                            id = item.Id.ToString()
                        });
                    }

                    recommandItem.blocks.Add(block);
                }
                var mode = IsAramPage ? "大乱斗" : "峡谷5v5";
                recommandItem.title = $"{Hero.Name}{mode}装备推荐——NPhoenix";
                var data = JsonConvert.SerializeObject(recommandItem);
                foreach (var file in dirinfo.GetFiles())
                {
                    try
                    {
                        File.Delete(file.FullName);
                    }
                    catch
                    {
                        continue;
                    }
                }

                DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                long timeStamp = (long)(DateTime.Now - startTime).TotalSeconds;
                await File.WriteAllTextAsync(Path.Combine(path, $"{Hero.Alias}{map}-{timeStamp}.json"), data);

                Growl.SuccessGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "推荐装备设置成功",
                    ShowDateTime = false
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Growl.WarningGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "发生错误",
                    ShowDateTime = false
                });
            }
        }

        private void SwitchRuneToCommon()
        {
            if (!IsAramPage)
                return;

            IsAramPage = false;
            DisplayRunes = CommonRunes;
            DisplayItems = CommonItems;
        }

        private void SwitchRuneToAram()
        {
            if (IsAramPage)
                return;

            IsAramPage = true;
            DisplayRunes = AramRunes;
            DisplayItems = AramItems;
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
    }
}
