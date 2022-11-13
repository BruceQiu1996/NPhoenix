using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LeagueOfLegendsBoxer.Application.Teamup;
using LeagueOfLegendsBoxer.Helpers;
using LeagueOfLegendsBoxer.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace LeagueOfLegendsBoxer.ViewModels.Pages
{
    public class RecordRankViewModel : ObservableObject
    {
        private readonly ITeamupService _teamupService;
        private readonly ILogger<RecordRankViewModel> _logger;
        private readonly SoftwareHelper _softwareHelper;

        private TotalRank _totalRank;
        public TotalRank TotalRank
        {
            get => _totalRank;
            set => SetProperty(ref _totalRank, value);
        }

        private DateTime _now = DateTime.Now;
        public DateTime Now
        {
            get => _now;
            set => SetProperty(ref _now, value);
        }

        public RecordRankViewModel(ITeamupService teamupService, ILogger<RecordRankViewModel> logger, SoftwareHelper softwareHelper)
        {
            _logger = logger;
            _teamupService = teamupService;
            _softwareHelper = softwareHelper;
            LoadCommandAsync = new AsyncRelayCommand(LoadAsync);
        }

        public AsyncRelayCommand LoadCommandAsync { get; set; }

        private async Task LoadAsync()
        {
            TotalRank = await _softwareHelper.GetRanksAsync();
        }
    }
}
