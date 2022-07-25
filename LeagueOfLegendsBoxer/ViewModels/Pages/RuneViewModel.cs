using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LeagueOfLegendsBoxer.Application.Game;
using LeagueOfLegendsBoxer.Application.Request;
using LeagueOfLegendsBoxer.Helpers;
using LeagueOfLegendsBoxer.Models;
using LeagueOfLegendsBoxer.Resources;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LeagueOfLegendsBoxer.ViewModels.Pages
{
    public class RuneViewModel : ObservableObject
    {
        private readonly RuneHelper _runeHelper;
        private readonly IGameService _gameService;
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

        public AsyncRelayCommand LoadCommandAsync { get; set; }
        public RelayCommand SwitchRuneToCommonCommand { get; set; }
        public RelayCommand SwitchRuneToAramCommand { get; set; }
        public AsyncRelayCommand<RuneDetail> SetRuneCommandAsync { get; set; }
        public RuneViewModel(RuneHelper runeHelper, IGameService gameService)
        {
            _runeHelper = runeHelper;
            _gameService = gameService;
            SwitchRuneToCommonCommand = new RelayCommand(SwitchRuneToCommon);
            SwitchRuneToAramCommand = new RelayCommand(SwitchRuneToAram);
            SetRuneCommandAsync = new AsyncRelayCommand<RuneDetail>(SetRuneAsync);
        }

        private bool _isAramPage;
        public bool IsAramPage
        {
            get { return _isAramPage; }
            set { SetProperty(ref _isAramPage, value); }

        }

        public async Task LoadChampInfoAsync(int champId, bool isAram)
        {
            priviouschampId = currentchampId;
            currentchampId = champId;
            if (priviouschampId == currentchampId)
                return;

            var module = await _runeHelper.GetRuneAsync(champId);
            Hero = Constant.Heroes?.FirstOrDefault(x => x.ChampId == champId);
            AramRunes = new ObservableCollection<RuneDetail>(module.Aram);
            CommonRunes = new ObservableCollection<RuneDetail>(module.Common);
            DisplayRunes = isAram ? AramRunes : CommonRunes;
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
            if (token != null)
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
        }

        private void SwitchRuneToAram()
        {
            if (IsAramPage)
                return;

            IsAramPage = true;
            DisplayRunes = AramRunes;
        }
    }
}
