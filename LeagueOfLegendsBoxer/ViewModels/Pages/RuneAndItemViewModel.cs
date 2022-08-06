using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LeagueOfLegendsBoxer.Application.Game;
using LeagueOfLegendsBoxer.Helpers;
using LeagueOfLegendsBoxer.Models;
using LeagueOfLegendsBoxer.Resources;
using LeagueOfLegendsBoxer.Windows;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public AsyncRelayCommand LoadCommandAsync { get; set; }
        public RelayCommand SwitchRuneToCommonCommand { get; set; }
        public RelayCommand SwitchRuneToAramCommand { get; set; }
        public RelayCommand SwitchItemPageCommand { get; set; }
        public RelayCommand SwitchRunePageCommand { get; set; }
        public AsyncRelayCommand<RuneDetail> SetRuneCommandAsync { get; set; }
        public RuneAndItemViewModel(RuneHelper runeHelper, IGameService gameService, IniSettingsModel iniSettingsModel)
        {
            _runeHelper = runeHelper;
            _gameService = gameService;
            SwitchRuneToCommonCommand = new RelayCommand(SwitchRuneToCommon);
            SwitchRuneToAramCommand = new RelayCommand(SwitchRuneToAram);
            SetRuneCommandAsync = new AsyncRelayCommand<RuneDetail>(SetRuneAsync);
            SwitchItemPageCommand = new RelayCommand(() => IsRunePage = false);
            SwitchRunePageCommand = new RelayCommand(() => IsRunePage = true);
            _iniSettingsModel = iniSettingsModel;
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

        public async Task LoadChampInfoAsync(int champId, bool isAram)
        {
            priviouschampId = currentchampId;
            currentchampId = champId;
            if (priviouschampId == currentchampId)
                return;

            var module = await _runeHelper.GetRuneAsync(champId);
            Hero = Constant.Heroes?.FirstOrDefault(x => x.ChampId == champId);
            AramRunes = new ObservableCollection<RuneDetail>(module.Rune.Aram);
            CommonRunes = new ObservableCollection<RuneDetail>(module.Rune.Common);
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
            var conversations = await _gameService.GetChatConversation();
            var token = JArray.Parse(conversations).FirstOrDefault(x => x.Value<string>("type") == "championSelect");
            if (token != null && !_iniSettingsModel.IsCloseRecommand)
            {
                string chatID = token.Value<string>("id");
                await _gameService.SendMessageAsync(chatID, $"{Hero.Name}一键符文配置成功,来自NPhoenix助手");
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
    }
}
