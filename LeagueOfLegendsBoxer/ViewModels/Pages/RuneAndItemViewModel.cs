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
        private RuneModule _customerRuneModule;

        private Hero _hero;
        public Hero Hero
        {
            get { return _hero; }
            set { SetProperty(ref _hero, value); }

        }

        private ObservableCollection<RuneDetail> _aramRunes;
        public ObservableCollection<RuneDetail> AramRunes
        {
            get { return _aramRunes; }
            set { SetProperty(ref _aramRunes, value); }

        }

        private ObservableCollection<RuneDetail> _commonRunes;
        public ObservableCollection<RuneDetail> CommonRunes
        {
            get { return _commonRunes; }
            set { SetProperty(ref _commonRunes, value); }

        }

        private ObservableCollection<RuneDetail> _displayRunes;
        public ObservableCollection<RuneDetail> DisplayRunes
        {
            get { return _displayRunes; }
            set { SetProperty(ref _displayRunes, value); }
        }

        private ItemsDetail _aramItems;
        public ItemsDetail AramItems
        {
            get { return _aramItems; }
            set { SetProperty(ref _aramItems, value); }

        }

        private ItemsDetail _commonItems;
        public ItemsDetail CommonItems
        {
            get { return _commonItems; }
            set { SetProperty(ref _commonItems, value); }

        }

        private ItemsDetail _displayItems;
        public ItemsDetail DisplayItems
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
        public AsyncRelayCommand<RuneDetail> SetRuneCommandAsync { get; set; }
        public AsyncRelayCommand ApplyItemCommandAsync { get; set; }
        public AsyncRelayCommand<RuneDetail> AutoApplyRuneCommandAsync { get; set; }
        public AsyncRelayCommand UnAutoApplyRuneCommandAsync { get; set; }
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
            SetRuneCommandAsync = new AsyncRelayCommand<RuneDetail>(SetRuneAsync);
            AutoApplyRuneCommandAsync = new AsyncRelayCommand<RuneDetail>(AutoApplyRuneAsync);
            UnAutoApplyRuneCommandAsync = new AsyncRelayCommand(UnAutoApplyRuneAsync);
            CheckedAutoLockHeroInAramCommandAsync = new AsyncRelayCommand(CheckedAutoLockHeroInAramAsync);
            UncheckedAutoLockHeroInAramCommandAsync = new AsyncRelayCommand(UncheckedAutoLockHeroInAramAsync);
            SwitchItemPageCommand = new RelayCommand(() => IsRunePage = false);
            SwitchRunePageCommand = new RelayCommand(() => IsRunePage = true);
            _iniSettingsModel = iniSettingsModel;
            _logger = logger;
            _applicationService = applicationService;
            ApplyItemCommandAsync = new AsyncRelayCommand(ApplyItemAsync);
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

        public async Task LoadChampInfoAsync(int champId, bool isAram, bool needFilterSame = true)
        {
            AutoLockHeroInAram = _iniSettingsModel.AutoLockHeroInAram;
            priviouschampId = currentchampId;
            currentchampId = champId;
            if (priviouschampId == currentchampId && needFilterSame)
                return;

            var data = await _gameService.GetRuneItemsFromOnlineAsync(champId);
            var module = await _runeHelper.GetRuneAsync(champId);
            var customerRunes = await _runeHelper.ReadCustomerRuneAsync(champId);
            _recommandModule = module;
            _customerRuneModule = customerRunes;
            if (_customerRuneModule == null)
            {
                _customerRuneModule = new RuneModule()
                {
                    ChampId = champId
                };
            }
            Hero = Constant.Heroes?.FirstOrDefault(x => x.ChampId == champId);
            AramRunes = new ObservableCollection<RuneDetail>(module.Rune.Aram);
            CommonRunes = new ObservableCollection<RuneDetail>(module.Rune.Common);
            foreach (var rune in _customerRuneModule.Aram.Reverse())
            {
                rune.IsCustomer = true;
                AramRunes.Insert(0, rune);
            }

            foreach (var rune in _customerRuneModule.Common.Reverse())
            {
                rune.IsCustomer = true;
                CommonRunes.Insert(0, rune);
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
                    var rune = AramRunes.OrderByDescending(x => x.Popular).FirstOrDefault();
                    await SetRuneAsyncMethod(rune, true);
                }
                else 
                {
                    var rune = CommonRunes.OrderByDescending(x => x.Popular).FirstOrDefault();
                    await SetRuneAsyncMethod(rune, true);
                }
            }
            else if (_iniSettingsModel.AutoUseRune && _iniSettingsModel.AutoUseRuneByWinRate)
            {
                if (isAram)
                {
                    var rune = AramRunes.OrderByDescending(x => x.WinRate).FirstOrDefault();
                    await SetRuneAsyncMethod(rune, true);
                }
                else
                {
                    var rune = CommonRunes.OrderByDescending(x => x.WinRate).FirstOrDefault();
                    await SetRuneAsyncMethod(rune, true);
                }
            }
            DisplayRunes = isAram ? AramRunes : CommonRunes;
            AramItems = module.Item.Aram;
            CommonItems = module.Item.Common;
            DisplayItems = isAram ? AramItems : CommonItems;
            IsAramPage = isAram;
            IsRunePage = true;
            App.ServiceProvider.GetRequiredService<ChampionSelectTool>().Show();
            (App.ServiceProvider.GetRequiredService<ChampionSelectTool>().DataContext as ChampionSelectToolViewModel).ShowRunePage();
        }

        private async Task SetRuneAsync(RuneDetail runeDetail)
        {
            await SetRuneAsyncMethod(runeDetail);
        }

        private async Task SetRuneAsyncMethod(RuneDetail runeDetail,bool isAuto = false) 
        {
            var result = await _gameService.GetAllRunePages();
            var array = JsonConvert.DeserializeObject<IEnumerable<RuneModel>>(result);
            var current = array.FirstOrDefault(x => x.IsDeletable);
            if (current != null) await _gameService.DeleteRunePage(current.Id);
            var mode = IsAramPage ? "大乱斗" : "峡谷5v5";
            var creation = new CreateRuneModel()
            {
                primaryStyleId = runeDetail.Main,
                subStyleId = runeDetail.Dputy,
                name = $"{Hero.Name} - {mode}符文",
                current = true,
                selectedPerkIds = new[] { runeDetail.Main1, runeDetail.Main2, runeDetail.Main3, runeDetail.Main4, runeDetail.Dputy1, runeDetail.Dputy2, runeDetail.Extra1, runeDetail.Extra2, runeDetail.Extra3 }
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

        private async Task AutoApplyRuneAsync(RuneDetail runeDetail)
        {
            if (IsAramPage)
            {
                _customerRuneModule.Aram.Where(x => x != runeDetail).ToList().ForEach(x => x.IsAutoApply = false);
                _recommandModule.Rune.Aram.Where(x => x != runeDetail).ToList().ForEach(x => x.IsAutoApply = false);
            }
            else
            {
                _customerRuneModule.Common.Where(x => x != runeDetail).ToList().ForEach(x => x.IsAutoApply = false);
                _recommandModule.Rune.Common.Where(x => x != runeDetail).ToList().ForEach(x => x.IsAutoApply = false);
            }

            await _runeHelper.WriteCustomerRuneAsync(currentchampId, _customerRuneModule);
            await _runeHelper.WriteSystemRuneAsync(currentchampId, _recommandModule);

            await LoadChampInfoAsync(currentchampId, IsAramPage, false);
        }

        private async Task UnAutoApplyRuneAsync()
        {
            if (IsAramPage)
            {
                _recommandModule.Rune.Aram.ToList().ForEach(x => x.IsAutoApply = false);
                _customerRuneModule.Aram.ToList().ForEach(x => x.IsAutoApply = false);
            }
            else
            {
                _customerRuneModule.Common.ToList().ForEach(x => x.IsAutoApply = false);
                _recommandModule.Rune.Common.ToList().ForEach(x => x.IsAutoApply = false);
            }

            await _runeHelper.WriteCustomerRuneAsync(currentchampId, _customerRuneModule);
            await _runeHelper.WriteSystemRuneAsync(currentchampId, _recommandModule);

            await LoadChampInfoAsync(currentchampId, IsAramPage, false);
        }

        public void Clear() 
        {
            priviouschampId = 0;
            currentchampId = 0;
        }

        private async Task ApplyItemAsync()
        {
            if (DisplayItems.CoreItem == null && DisplayItems.StartItem == null && DisplayItems.ShoeItem == null)
            {
                Growl.WarningGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "没有设置任何装备",
                    ShowDateTime = false
                });

                return;
            }

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
                if (DisplayItems.StartItem != null)
                {
                    var block = new Block();
                    block.type = "起始装备";
                    foreach (var item in DisplayItems.StartItem.ItemIds)
                    {
                        block.items.Add(new RItem()
                        {
                            id = item.ToString()
                        });
                    }

                    recommandItem.blocks.Add(block);
                }
                if (DisplayItems.CoreItem != null)
                {
                    var block = new Block();
                    block.type = "核心装备";
                    foreach (var item in DisplayItems.CoreItem.ItemIds)
                    {
                        block.items.Add(new RItem()
                        {
                            id = item.ToString()
                        });
                    }

                    recommandItem.blocks.Add(block);
                }
                if (DisplayItems.ShoeItem != null)
                {
                    var block = new Block();
                    block.type = "鞋子";
                    foreach (var item in DisplayItems.ShoeItem.ItemIds)
                    {
                        block.items.Add(new RItem()
                        {
                            id = item.ToString()
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

                DisplayItems.CoreItem = null; DisplayItems.StartItem = null; DisplayItems.ShoeItem = null;
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
