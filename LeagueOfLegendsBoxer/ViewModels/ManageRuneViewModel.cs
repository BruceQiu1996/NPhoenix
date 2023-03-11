using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using HandyControl.Data;
using LeagueOfLegendsBoxer.Application.Game;
using LeagueOfLegendsBoxer.Helpers;
using LeagueOfLegendsBoxer.Models;
using LeagueOfLegendsBoxer.Resources;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LeagueOfLegendsBoxer.ViewModels
{
    public class ManageRuneViewModel : ObservableObject
    {
        //private bool _chooseHeroForOpen;
        //public bool ChooseHeroForOpen
        //{
        //    get => _chooseHeroForOpen;
        //    set => SetProperty(ref _chooseHeroForOpen, value);
        //}

        //private string _preSearchChooseHeroText;
        //private string _searchChooseHeroText;
        //public string SearchChooseHeroText
        //{
        //    get => _searchChooseHeroText;
        //    set => SetProperty(ref _searchChooseHeroText, value);
        //}

        //private ObservableCollection<Hero> _chooseHeros;
        //public ObservableCollection<Hero> ChooseHeros
        //{
        //    get => _chooseHeros;
        //    set => SetProperty(ref _chooseHeros, value);
        //}

        //private ObservableCollection<string> _chooseModes;
        //public ObservableCollection<string> ChooseModes
        //{
        //    get => _chooseModes;
        //    set => SetProperty(ref _chooseModes, value);
        //}

        //private Tuple<HeroRecommandModule, ObservableCollection<RuneModule>> _systemRunes;
        //public Tuple<HeroRecommandModule, ObservableCollection<RuneModule>> SystemRunes
        //{
        //    get => _systemRunes;
        //    set => SetProperty(ref _systemRunes, value);
        //}

        //private Tuple<RuneModule, ObservableCollection<RuneModule>> _customerRunes;
        //public Tuple<RuneModule, ObservableCollection<RuneModule>> CustomerRunes
        //{
        //    get => _customerRunes;
        //    set => SetProperty(ref _customerRunes, value);
        //}

        //private string _chooseMode;
        //public string ChooseMode
        //{
        //    get => _chooseMode;
        //    set
        //    {
        //        SetProperty(ref _chooseMode, value);
        //        SwitchHeroOrModeAsync();
        //    }
        //}

        //private Hero _chooseHero;
        //public Hero ChooseHero
        //{
        //    get => _chooseHero;
        //    set
        //    {
        //        SetProperty(ref _chooseHero, value);
        //        SwitchHeroOrModeAsync();
        //    }
        //}

        //public RelayCommand SearchHeroCommand { get; set; }
        //public RelayCommand LoadCommand { get; set; }
        //public AsyncRelayCommand ImportRuneFromClientCommandAsync { get; set; }
        //public AsyncRelayCommand<RuneModule> DeleteCustomerRuneCommandAsync { get; set; }
        //public AsyncRelayCommand<RuneModule> DeleteSystemRuneCommandAsync { get; set; }
        //public AsyncRelayCommand<RuneModule> AutoApplyCustomerCommandAsync { get; set; }
        //public AsyncRelayCommand UnAutoApplyCustomerCommandAsync { get; set; }
        //public AsyncRelayCommand<RuneModule> AutoApplySystemCommandAsync { get; set; }
        //public AsyncRelayCommand UnAutoApplySystemCommandAsync { get; set; }

        //private readonly RuneHelper _runeHelper;
        //private readonly IGameService _gameService;
        //private readonly ILogger<ManageRuneViewModel> _logger;

        //public ManageRuneViewModel(RuneHelper runeHelper, IGameService gameService, ILogger<ManageRuneViewModel> logger)
        //{
        //    SearchHeroCommand = new RelayCommand(SearchHero);
        //    DeleteCustomerRuneCommandAsync = new AsyncRelayCommand<RuneModule>(DeleteCustomerRuneAsync);
        //    DeleteSystemRuneCommandAsync = new AsyncRelayCommand<RuneModule>(DeleteSystemRuneAsync);
        //    ImportRuneFromClientCommandAsync = new AsyncRelayCommand(ImportRuneFromClientAsync);
        //    AutoApplyCustomerCommandAsync = new AsyncRelayCommand<RuneModule>(AutoApplyCustomerAsync);
        //    UnAutoApplyCustomerCommandAsync = new AsyncRelayCommand(UnAutoApplyCustomerAsync);
        //    AutoApplySystemCommandAsync = new AsyncRelayCommand<RuneModule>(AutoApplySystemAsync);
        //    UnAutoApplySystemCommandAsync = new AsyncRelayCommand(UnAutoApplySystemAsync);
        //    LoadCommand = new RelayCommand(Load);
        //    _runeHelper = runeHelper;
        //    _logger = logger;
        //    _gameService = gameService;
        //}

        //private void Load()
        //{
        //    ChooseHeros = new ObservableCollection<Hero>(Constant.Heroes);
        //    ChooseHero = null;
        //    ChooseModes = new ObservableCollection<string>();
        //    ChooseModes.Add("峡谷5v5");
        //    ChooseModes.Add("大乱斗");
        //    ChooseMode = ChooseModes.FirstOrDefault();
        //}

        //private void SearchHero()
        //{
        //    ChooseHeroForOpen = true;
        //    if (_preSearchChooseHeroText == SearchChooseHeroText)
        //        return;

        //    _preSearchChooseHeroText = SearchChooseHeroText;
        //    ChooseHero = null;
        //    if (string.IsNullOrEmpty(SearchChooseHeroText))
        //        ChooseHeros = new ObservableCollection<Hero>(Constant.Heroes);
        //    else
        //        ChooseHeros = new ObservableCollection<Hero>(Constant.Heroes.Where(x => x.Label.Contains(SearchChooseHeroText) || x.Title.Contains(SearchChooseHeroText)));
        //}

        //private async void SwitchHeroOrModeAsync()
        //{
        //    if (ChooseHero == null || ChooseMode == null)
        //        return;

        //    var myRunes = await _runeHelper.ReadCustomerRuneAsync(ChooseHero.ChampId);
        //    var sysRunes = await _runeHelper.GetRuneAsync(ChooseHero.ChampId);
        //    if (ChooseMode == "峡谷5v5")
        //    {
        //        SystemRunes = sysRunes == null ? null : new Tuple<HeroRecommandModule, ObservableCollection<RuneModule>>(sysRunes, new ObservableCollection<RuneModule>(sysRunes.Rune.Common));
        //        CustomerRunes = myRunes == null ? null : new Tuple<RuneModule, ObservableCollection<RuneModule>>(myRunes, new ObservableCollection<RuneModule>(myRunes.Common));
        //    }
        //    else
        //    {
        //        SystemRunes = sysRunes == null ? null : new Tuple<HeroRecommandModule, ObservableCollection<RuneModule>>(sysRunes, new ObservableCollection<RuneModule>(sysRunes.Rune.Aram));
        //        CustomerRunes = myRunes == null ? null : new Tuple<RuneModule, ObservableCollection<RuneModule>>(myRunes, new ObservableCollection<RuneModule>(myRunes.Aram));
        //    }
        //}

        ///// <summary>
        ///// 从客户端导入符文
        ///// </summary>
        ///// <returns></returns>
        //private async Task ImportRuneFromClientAsync()
        //{
        //    if (ChooseHero == null || ChooseMode == null)
        //    {
        //        Growl.WarningGlobal(new GrowlInfo()
        //        {
        //            WaitTime = 2,
        //            Message = "未选择任何英雄或者模式",
        //            ShowDateTime = false
        //        });

        //        return;
        //    }

        //    try
        //    {
        //        var current = await _gameService.GetCurrentRunePage();
        //        var model = JsonConvert.DeserializeObject<GetRuneModel>(current);
        //        if (!model.IsValid || model.selectedPerkIds == null || model.selectedPerkIds.Count() <= 8)
        //        {
        //            Growl.WarningGlobal(new GrowlInfo()
        //            {
        //                WaitTime = 2,
        //                Message = "符文不合法",
        //                ShowDateTime = false
        //            });

        //            return;
        //        }

        //        //保存到本地在读取到内存中
        //        var detail = new RuneModule()
        //        {
        //            IsAutoApply = false,
        //            Name = model.name,
        //            Main = model.primaryStyleId,
        //            Dputy = model.subStyleId,
        //            Main1 = model.selectedPerkIds[0],
        //            Main2 = model.selectedPerkIds[1],
        //            Main3 = model.selectedPerkIds[2],
        //            Main4 = model.selectedPerkIds[3],
        //            Dputy1 = model.selectedPerkIds[4],
        //            Dputy2 = model.selectedPerkIds[5],
        //            Extra1 = model.selectedPerkIds[6],
        //            Extra2 = model.selectedPerkIds[7],
        //            Extra3 = model.selectedPerkIds[8],
        //        };

        //        RuneModule runeModule = new RuneModule()
        //        {
        //            ChampId = ChooseHero.ChampId,
        //            Common = new List<RuneModule>(),
        //            Aram = new List<RuneModule>()
        //        };

        //        if (CustomerRunes != null)
        //        {
        //            runeModule = CustomerRunes.Item1;
        //        }

        //        if (ChooseMode == "峡谷5v5")
        //        {
        //            runeModule.Common.Add(detail);
        //        }
        //        else
        //        {
        //            runeModule.Aram.Add(detail);
        //        }

        //        await _runeHelper.WriteCustomerRuneAsync(ChooseHero.ChampId, runeModule);
        //        SwitchHeroOrModeAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "导入符文出错");
        //        Growl.WarningGlobal(new GrowlInfo()
        //        {
        //            WaitTime = 2,
        //            Message = "导入符文出错",
        //            ShowDateTime = false
        //        });

        //        return;
        //    }
        //}

        //private async Task DeleteCustomerRuneAsync(RuneModule RuneModule)
        //{
        //    if (CustomerRunes.Item1.Common.Contains(RuneModule))
        //        CustomerRunes.Item1.Common.Remove(RuneModule);
        //    if (CustomerRunes.Item1.Aram.Contains(RuneModule))
        //        CustomerRunes.Item1.Aram.Remove(RuneModule);

        //    await _runeHelper.WriteCustomerRuneAsync(ChooseHero.ChampId, CustomerRunes.Item1);
        //    SwitchHeroOrModeAsync();
        //}

        //private async Task AutoApplyCustomerAsync(RuneModule RuneModule)
        //{
        //    if (ChooseMode == "峡谷5v5")
        //    {
        //        CustomerRunes.Item1.Common.Where(x => x != RuneModule).ToList().ForEach(x => x.IsAutoApply = false);
        //        SystemRunes.Item1.Rune.Common.ToList().ForEach(x => x.IsAutoApply = false);
        //    }
        //    else
        //    {
        //        CustomerRunes.Item1.Aram.Where(x => x != RuneModule).ToList().ForEach(x => x.IsAutoApply = false);
        //        SystemRunes.Item1.Rune.Aram.ToList().ForEach(x => x.IsAutoApply = false);
        //    }

        //    await _runeHelper.WriteCustomerRuneAsync(ChooseHero.ChampId, CustomerRunes.Item1);
        //    await _runeHelper.WriteSystemRuneAsync(ChooseHero.ChampId, SystemRunes.Item1);

        //    SwitchHeroOrModeAsync();
        //}

        //private async Task UnAutoApplyCustomerAsync()
        //{
        //    if (ChooseMode == "峡谷5v5")
        //    {
        //        CustomerRunes.Item1.Common.ToList().ForEach(x => x.IsAutoApply = false);
        //        SystemRunes.Item1.Rune.Common.ToList().ForEach(x => x.IsAutoApply = false);
        //    }
        //    else
        //    {
        //        CustomerRunes.Item1.Aram.ToList().ForEach(x => x.IsAutoApply = false);
        //        SystemRunes.Item1.Rune.Aram.ToList().ForEach(x => x.IsAutoApply = false);
        //    }

        //    await _runeHelper.WriteCustomerRuneAsync(ChooseHero.ChampId, CustomerRunes.Item1);
        //    await _runeHelper.WriteSystemRuneAsync(ChooseHero.ChampId, SystemRunes.Item1);

        //    SwitchHeroOrModeAsync();
        //}
        //private async Task AutoApplySystemAsync(RuneModule RuneModule)
        //{
        //    if (ChooseMode == "峡谷5v5")
        //    {
        //        CustomerRunes.Item1.Common.ToList().ForEach(x => x.IsAutoApply = false);
        //        SystemRunes.Item1.Rune.Common.Where(x => x != RuneModule).ToList().ForEach(x => x.IsAutoApply = false);
        //    }
        //    else
        //    {
        //        CustomerRunes.Item1.Aram.ToList().ForEach(x => x.IsAutoApply = false);
        //        SystemRunes.Item1.Rune.Aram.Where(x => x != RuneModule).ToList().ForEach(x => x.IsAutoApply = false);
        //    }

        //    await _runeHelper.WriteCustomerRuneAsync(ChooseHero.ChampId, CustomerRunes.Item1);
        //    await _runeHelper.WriteSystemRuneAsync(ChooseHero.ChampId, SystemRunes.Item1);

        //    SwitchHeroOrModeAsync();
        //}

        //private async Task UnAutoApplySystemAsync()
        //{
        //    if (ChooseMode == "峡谷5v5")
        //    {
        //        CustomerRunes.Item1.Common.ToList().ForEach(x => x.IsAutoApply = false);
        //        SystemRunes.Item1.Rune.Common.ToList().ForEach(x => x.IsAutoApply = false);
        //    }
        //    else
        //    {
        //        CustomerRunes.Item1.Aram.ToList().ForEach(x => x.IsAutoApply = false);
        //        SystemRunes.Item1.Rune.Aram.ToList().ForEach(x => x.IsAutoApply = false);
        //    }

        //    await _runeHelper.WriteCustomerRuneAsync(ChooseHero.ChampId, CustomerRunes.Item1);
        //    await _runeHelper.WriteSystemRuneAsync(ChooseHero.ChampId, SystemRunes.Item1);

        //    SwitchHeroOrModeAsync();
        //}
    }
}
