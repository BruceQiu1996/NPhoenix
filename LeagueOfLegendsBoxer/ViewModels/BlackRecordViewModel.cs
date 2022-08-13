using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using HandyControl.Data;
using LeagueOfLegendsBoxer.Models;
using LeagueOfLegendsBoxer.Resources;
using LeagueOfLegendsBoxer.ViewModels.Pages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LeagueOfLegendsBoxer.ViewModels
{
    public class BlackRecordViewModel : ObservableObject 
    {
        public ObservableCollection<BlackAccount> _blackAccounts;
        public ObservableCollection<BlackAccount> BlackAccounts
        {
            get { return _blackAccounts; }
            set { SetProperty(ref _blackAccounts, value); }
        }

        public string _desc;
        public string Desc
        {
            get { return _desc; }
            set { SetProperty(ref _desc, value); }
        }
        public AsyncRelayCommand<BlackAccount> SearchRecordCommandAsync { get; set; }
        public AsyncRelayCommand<BlackAccount> RemoveBlackListCommanmdAsync { get; set; }

        private readonly IniSettingsModel _iniSettingsModel;
        private readonly ILogger<BlackRecordViewModel> _logger;
        public BlackRecordViewModel(IniSettingsModel iniSettingsModel, ILogger<BlackRecordViewModel> logger)
        {
            _iniSettingsModel = iniSettingsModel;
            _logger = logger;
            BlackAccounts = new ObservableCollection<BlackAccount>();
            SearchRecordCommandAsync = new AsyncRelayCommand<BlackAccount>(SearchRecordAsync);
            RemoveBlackListCommanmdAsync = new AsyncRelayCommand<BlackAccount>(RemoveBlackListAsync);
        }

        public void Load(IEnumerable<long> ids = null) 
        {
            if (ids == null)
            {
                BlackAccounts = new ObservableCollection<BlackAccount>(
                    _iniSettingsModel.BlackAccounts.OrderByDescending(x => x.CreateTime));

                Desc = $"一共拉黑{BlackAccounts.GroupBy(x=>x.Id).Count()}人,拉黑记录{BlackAccounts.Count}条";
            }
            else 
            {
                BlackAccounts = new ObservableCollection<BlackAccount>(
                    _iniSettingsModel.BlackAccounts.OrderByDescending(x => x.CreateTime).Where(x => ids.Contains(x.Id)));
                Desc = null;
            }
        }

        private async Task SearchRecordAsync(BlackAccount blackAccount) 
        {
            var teamvm = App.ServiceProvider.GetRequiredService<TeammateViewModel>();
            var account = await teamvm.GetAccountAsync(blackAccount.Id);
            if (account == null) 
            {
                Growl.WarningGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "获取信息失败",
                    ShowDateTime = false
                });

                return;
            }

            teamvm.ShowRecord(account);
        }

        private async Task RemoveBlackListAsync(BlackAccount blackAccount) 
        {
            await _iniSettingsModel.RemoveBlackAccountAsync(blackAccount.Id);
            Growl.SuccessGlobal(new GrowlInfo()
            {
                WaitTime = 2,
                Message = "移除成功",
                ShowDateTime = false
            });
        }
    }
}
