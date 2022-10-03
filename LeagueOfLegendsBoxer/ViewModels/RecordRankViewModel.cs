using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using HandyControl.Data;
using LeagueOfLegendsBoxer.Application.Teamup;
using LeagueOfLegendsBoxer.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace LeagueOfLegendsBoxer.ViewModels
{
    public class RecordRankViewModel : ObservableObject
    {
        private readonly ITeamupService _teamupService;
        private readonly ILogger<RecordRankViewModel> _logger;
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

        public RecordRankViewModel(ITeamupService teamupService, ILogger<RecordRankViewModel> logger)
        {
            _logger = logger;
            _teamupService = teamupService;
            LoadCommandAsync = new AsyncRelayCommand(LoadAsync);
        }

        public AsyncRelayCommand LoadCommandAsync { get; set; }

        private async Task LoadAsync() 
        {
            try
            {
                var result = await _teamupService.GetRankDataAsync();
                if (string.IsNullOrEmpty(result))
                {
                    Growl.WarningGlobal(new GrowlInfo()
                    {
                        WaitTime = 2,
                        Message = "拉取排位信息失败",
                        ShowDateTime = false
                    });

                    return;
                }

                TotalRank = JsonConvert.DeserializeObject<TotalRank>(result);
                var mvpRank = 0;
                foreach (var item in TotalRank.Mvp) 
                {
                    item.Rank = ++mvpRank;
                }
                var svpRank = 0;
                foreach (var item in TotalRank.Svp)
                {
                    item.Rank = ++svpRank;
                }
                var noobRank = 0;
                foreach (var item in TotalRank.Noob)
                {
                    item.Rank = ++noobRank;
                }
                var xiaguRank = 0;
                foreach (var item in TotalRank.Xiagu) 
                {
                    item.Rank = ++xiaguRank;
                }
                var aramRank = 0;
                foreach (var item in TotalRank.Aram)
                {
                    item.Rank = ++aramRank;
                }
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex.ToString());
                Growl.WarningGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "拉取排位信息失败",
                    ShowDateTime = false
                });
            }
        }
    }
}
