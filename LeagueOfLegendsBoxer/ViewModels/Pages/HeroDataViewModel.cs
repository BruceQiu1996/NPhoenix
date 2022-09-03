using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LeagueOfLegendsBoxer.Application.Game;
using LeagueOfLegendsBoxer.Models;
using LeagueOfLegendsBoxer.Resources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LeagueOfLegendsBoxer.ViewModels.Pages
{
    public class HeroDataViewModel : ObservableObject
    {
        private ObservableCollection<ChampRestraint> _champRestraints;
        public ObservableCollection<ChampRestraint> ChampRestraints
        {
            get { return _champRestraints; }
            set { SetProperty(ref _champRestraints, value); }
        }

        private ObservableCollection<Tuple<string, int>> _levelOptions;
        public ObservableCollection<Tuple<string, int>> LevelOptions
        {
            get { return _levelOptions; }
            set { SetProperty(ref _levelOptions, value); }
        }

        private Tuple<string, int> _levelOption;
        public Tuple<string, int> LevelOption
        {
            get { return _levelOption; }
            set { SetProperty(ref _levelOption, value); }
        }

        private ObservableCollection<Tuple<string,Action>> _sortRoles;
        public ObservableCollection<Tuple<string,Action>> SortRoles
        {
            get { return _sortRoles; }
            set { SetProperty(ref _sortRoles, value); }
        }

        private Tuple<string, Action> _sortRole;
        public Tuple<string, Action> SortRole
        {
            get { return _sortRole; }
            set 
            {
                SetProperty(ref _sortRole, value);
                _sortRole.Item2.Invoke();
            }
        }

        private ObservableCollection<Tuple<string, string>> _positions;
        public ObservableCollection<Tuple<string, string>> Positions
        {
            get { return _positions; }
            set { SetProperty(ref _positions, value); }
        }

        private Tuple<string, string> _position;
        public Tuple<string, string> Position
        {
            get { return _position; }
            set
            {
                SetProperty(ref _position, value);
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                SetProperty(ref _isLoading, value);
            }
        }

        private bool _champRankDataVisibility;
        public bool ChampRankDataVisibility
        {
            get { return _champRankDataVisibility; }
            set
            {
                SetProperty(ref _champRankDataVisibility, value);
            }
        }

        private ObservableCollection<ChampRank> _champRanks;
        public ObservableCollection<ChampRank> ChampRanks
        {
            get { return _champRanks; }
            set { SetProperty(ref _champRanks, value); }
        }

        private ChampRank _champRank;
        public ChampRank ChampRank
        {
            get { return _champRank; }
            set { SetProperty(ref _champRank, value); }
        }

        private readonly IGameService _gameService;
        private readonly IniSettingsModel _iniSettingsModel;
        private readonly IConfiguration _configuration;
        private readonly ILogger<HeroDataViewModel> _logger;
        public AsyncRelayCommand LoadCommandAsync { get; set; }
        public AsyncRelayCommand LevelOptionSelectionChangedCommandAsync { get; set; }
        public AsyncRelayCommand PositionSelectionChangedCommandAsync { get; set; }
        public AsyncRelayCommand ViewChampDetailCommandAsync { get; set; }
        public RelayCommand BackChampRankDataPageCommand { get; set; }
        public HeroDataViewModel(IGameService gameService,
                                 ILogger<HeroDataViewModel> logger,
                                 IniSettingsModel iniSettingsModel,
                                 IConfiguration configuration)
        {
            ChampRankDataVisibility = true;
            _gameService = gameService;
            _logger = logger;
            _iniSettingsModel = iniSettingsModel;
            _configuration = configuration;
            LevelOptions = new ObservableCollection<Tuple<string, int>>()
            {
                new Tuple<string, int>("王者",0),
                new Tuple<string, int>("宗师",5),
                new Tuple<string, int>("大师",6),
                new Tuple<string, int>("钻石",10),
                new Tuple<string, int>("铂金",20),
                new Tuple<string, int>("黄金",30),
                new Tuple<string, int>("白银",40),
                new Tuple<string, int>("黄铜",50),
                new Tuple<string, int>("黑铁",80),
                new Tuple<string, int>("铂金及以上",200),
            };

            SortRoles = new ObservableCollection<Tuple<string, Action>>()
            {
                new Tuple<string, Action>("综合",Sort),
                new Tuple<string, Action>("胜率",Sort),
                new Tuple<string, Action>("出场率",Sort),
                new Tuple<string, Action>("禁用率",Sort),
            };

            Positions = new ObservableCollection<Tuple<string, string>>()
            {
                new Tuple<string, string>("上单","top"),
                new Tuple<string, string>("打野","jungle"),
                new Tuple<string, string>("中单","mid"),
                new Tuple<string, string>("射手","bottom"),
                new Tuple<string, string>("辅助","support"),
            };
            Position = Positions.FirstOrDefault();
            LevelOption = LevelOptions.FirstOrDefault(x => x.Item2 == 200);
            SortRole = SortRoles.FirstOrDefault();
            LoadCommandAsync = new AsyncRelayCommand(LoadAsync);
            LevelOptionSelectionChangedCommandAsync = new AsyncRelayCommand(LevelOptionSelectionChangedAsync);
            PositionSelectionChangedCommandAsync = new AsyncRelayCommand(PositionSelectionChangedAsync);
            ViewChampDetailCommandAsync = new AsyncRelayCommand(ViewChampDetailAsync) ;
            BackChampRankDataPageCommand = new RelayCommand(Back);
            ChampRanks = new ObservableCollection<ChampRank>();
        }

        private async Task LoadAsync()
        {       
            Position = Positions.FirstOrDefault();
            LevelOption = LevelOptions.FirstOrDefault(x => x.Item2 == 200);
            SortRole = SortRoles.FirstOrDefault();
            await FetchChampRankDataAsync();
        }

        private async Task FetchChampRankDataAsync() 
        {
            try
            {
                IsLoading = true;
                ChampRanks.Clear();
                var data = await _gameService.GetChampRankAsync(Position.Item2, LevelOption.Item2, getLacalDateInteger());
                var championdetails = JToken.Parse(data)["data"].FirstOrDefault()?.FirstOrDefault()?.FirstOrDefault()?.FirstOrDefault();
                if (championdetails != null)
                {
                    var datas = championdetails.ToString().Split("#");
                    datas[0] = datas[0].Substring(datas[0].IndexOf(":\"") + 2);
                    foreach (var dr in datas)
                    {
                        var dataList = dr.Split("_");
                        var champId = int.TryParse(dataList[1], out var tempChampId) ? tempChampId : 0;
                        if (Constant.Heroes.FirstOrDefault(x => x.ChampId == champId) == null)
                            continue;

                        ChampRanks.Add(new ChampRank()
                        {
                            Sort = int.TryParse(dataList[0], out var tempSort) ? tempSort : 0,
                            ChampId = champId,
                            TLevel = double.TryParse(dataList[2], out var temptLevel) ? temptLevel : 0,
                            Win = double.TryParse(dataList[3], out var tempWin) ? tempWin : 0,
                            Ban = double.TryParse(dataList[4], out var tempBan) ? tempBan : 0,
                            Appearance = double.TryParse(dataList[5], out var tempAppearance) ? tempAppearance : 0,
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            finally 
            {
                IsLoading = false;
            }

            Sort();
        }

        public async Task ViewChampDetailAsync()
        {
            if (ChampRank == null)
                return;

            IsLoading = true;
            try
            {
                var data = await _gameService.GetChampRestraintDataAsync(ChampRank.ChampId);
                var detailsData = JToken.Parse(data.Split("=")[1].Split(";/*")[0]);
                var restraintJson = detailsData["list"]["championFight"][Position.Item2];
                if (restraintJson == null)
                    return;

                var list = restraintJson.ToObject<IEnumerable<ChampRestraint>>();
                list = list.Select(x =>
                {
                    x.WinrateValue = int.Parse(x.Winrate);
                    return x;
                });
                ChampRestraints = new ObservableCollection<ChampRestraint>(list.OrderBy(x => x.WinrateValue));
                ChampRankDataVisibility = false;
            }
            catch (Exception ex)
            {

            }
            finally 
            {
                IsLoading = false;
            }
        }

        private void Sort()
        {
            if (ChampRanks == null || ChampRanks.Count <= 0)
                return;

            switch (SortRole.Item1)
            {
                case "综合":
                    ChampRanks = new ObservableCollection<ChampRank>(ChampRanks.OrderBy(x => x.Sort));
                    break;
                case "胜率":
                    ChampRanks = new ObservableCollection<ChampRank>(ChampRanks.OrderByDescending(x => x.Win));
                    break;
                case "出场率":
                    ChampRanks = new ObservableCollection<ChampRank>(ChampRanks.OrderByDescending(x => x.Appearance));
                    break;
                case "禁用率":
                    ChampRanks = new ObservableCollection<ChampRank>(ChampRanks.OrderByDescending(x => x.Ban));
                    break;
                default:
                    ChampRanks = new ObservableCollection<ChampRank>(ChampRanks.OrderBy(x => x.Sort));
                    break;
            }
        }

        private void Back() 
        {
            ChampRankDataVisibility = true;    
        }

        private async Task LevelOptionSelectionChangedAsync() 
        {
            await FetchChampRankDataAsync();
        }

        private async Task PositionSelectionChangedAsync() 
        {
            await FetchChampRankDataAsync();
        }

        private int getLacalDateInteger()
        {
            var dateNows = DateTime.Now.ToString("d").Split("/");
            if (dateNows[1].Length == 1)
                dateNows[1] = "0" + dateNows[1];

            return int.Parse(string.Join(string.Empty, dateNows)) - 2;
        }
    }
}
