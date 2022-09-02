using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using HandyControl.Data;
using LeagueOfLegendsBoxer.Models;
using LeagueOfLegendsBoxer.Resources;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace LeagueOfLegendsBoxer.ViewModels
{
    public class BlackTipViewModel : ObservableObject
    {
        public long _id;
        public long Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        public string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public string _reason;
        public string Reason
        {
            get { return _reason; }
            set { SetProperty(ref _reason, value); }
        }
        private readonly ILogger<BlackTipViewModel> _logger;
        private readonly IniSettingsModel _iniSettingsModel;

        public AsyncRelayCommand AddAccountBlackListCommandAsync { get; set; }
        public BlackTipViewModel(ILogger<BlackTipViewModel> logger, IniSettingsModel iniSettingsModel)
        {
            _logger = logger;
            _iniSettingsModel = iniSettingsModel;
            AddAccountBlackListCommandAsync = new AsyncRelayCommand(AddAccountBlackListAsync);
        }

        public void Load(long id, string name)
        {
            Id = id;
            Name = name;
            Reason = null;
        }

        private async Task AddAccountBlackListAsync()
        {
            try
            {
                var blackAccount = new BlackAccount()
                {
                    Id = Id,
                    AccountName = Name,
                    CreateTime = DateTime.Now,
                    Reason = Reason,
                };

                await _iniSettingsModel.WriteBlackAccountAsync(blackAccount);
                Growl.SuccessGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "拉黑成功",
                    ShowDateTime = false
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Growl.WarningGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "拉黑失败",
                    ShowDateTime = false
                });
            }
        }
    }
}
