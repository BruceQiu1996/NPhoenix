using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using HandyControl.Data;
using LeagueOfLegendsBoxer.Application.Teamup;
using LeagueOfLegendsBoxer.Application.Teamup.Dtos;
using LeagueOfLegendsBoxer.Models;
using LeagueOfLegendsBoxer.Resources;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LeagueOfLegendsBoxer.ViewModels
{
    public class ServerAreaViewModel : ObservableObject
    {
        private string _name;
        public string Name 
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private ServerArea _serverArea;
        public ServerArea ServerArea 
        {
            get { return _serverArea; }
            set { SetProperty(ref _serverArea, value); }
        }

        private ObservableCollection<ServerArea> _serverAreas;
        public ObservableCollection<ServerArea> ServerAreas 
        {
            get { return _serverAreas; }
            set { SetProperty(ref _serverAreas, value); }
        }

        public AsyncRelayCommand SaveCommandAsync { get; set; }
        public RelayCommand LoadCommand { get; set; }

        private readonly ITeamupService _teamupService;
        private readonly ILogger<ServerAreaViewModel> _logger;
        public ServerAreaViewModel(IOptions<List<ServerArea>> options, 
                                   ITeamupService teamupService,
                                   ILogger<ServerAreaViewModel> logger)
        {
            _logger = logger;
            _teamupService = teamupService;
            ServerAreas = new ObservableCollection<ServerArea>(options.Value);
            SaveCommandAsync = new AsyncRelayCommand(SaveAsync);
            LoadCommand = new RelayCommand(Load);
        }

        private async Task SaveAsync() 
        {
            if (ServerArea == null)
                return;

            try
            {
                var result = await _teamupService.UpdateServerAreaAsync(new UserServerAreaUpdateDto()
                {
                    Id = Constant.Account.SummonerId,
                    ServerArea = ServerArea.Label
                });

                if (result)
                {
                    Growl.SuccessGlobal(new GrowlInfo()
                    {
                        WaitTime = 2,
                        Message = "保存成功",
                        ShowDateTime = false
                    });
                }
                else
                {
                    Growl.WarningGlobal(new GrowlInfo()
                    {
                        WaitTime = 2,
                        Message = "保存失败",
                        ShowDateTime = false
                    });
                }
            }
            catch (Exception ex)
            {
                Growl.WarningGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "保存失败",
                    ShowDateTime = false
                });

                _logger.LogError(ex.ToString());
                return;
            }
        }

        private void Load() 
        {
            Name = Constant.Account.DisplayName;
            ServerArea = string.IsNullOrEmpty(Constant.Account?.ServerArea) ? null : ServerAreas.FirstOrDefault(x => x.Label == Constant.Account?.ServerArea);
        }
    }
}
