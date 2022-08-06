using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using HandyControl.Data;
using LeagueOfLegendsBoxer.Models;
using LeagueOfLegendsBoxer.Resources;
using LeagueOfLegendsBoxer.ViewModels.Pages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace LeagueOfLegendsBoxer.ViewModels
{
    public class BlackListViewModel : ObservableObject
    {
        public ObservableCollection<Account> _accounts;
        public ObservableCollection<Account> Accounts
        {
            get { return _accounts; }
            set { SetProperty(ref _accounts, value); }
        }

        public AsyncRelayCommand<Account> SubmitBlackListCommanmdAsync { get; set; }
        public RelayCommand<Account> ToggleBlackInfoCommand { get; set; }
        public RelayCommand<Account> SearchRecordCommand { get; set; }

        private readonly IniSettingsModel _iniSettingsModel;
        private readonly ILogger<BlackListViewModel> _logger;
        public BlackListViewModel(IniSettingsModel iniSettingsModel, ILogger<BlackListViewModel> logger)
        {
            Accounts = new ObservableCollection<Account>();
            _iniSettingsModel = iniSettingsModel;
            _logger = logger;
            SubmitBlackListCommanmdAsync = new AsyncRelayCommand<Account>(SubmitBlackListAsync);
            ToggleBlackInfoCommand = new RelayCommand<Account>(ToggleBlackInfo);
            SearchRecordCommand = new RelayCommand<Account>(SearchRecord);
        }

        public void LoadAccount(IEnumerable<Account> accounts)
        {
            Accounts = new ObservableCollection<Account>(accounts);
        }

        private async Task SubmitBlackListAsync(Account account)
        {
            try
            {
                var blackAccount = new BlackAccount()
                {
                    Id = account.SummonerId,
                    AccountName = account.DisplayName,
                    CreateTime = System.DateTime.Now,
                    Reason = account.Reason,
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

        private void ToggleBlackInfo(Account account)
        {
            account.IsOpenBlack = !account.IsOpenBlack;
        }

        private void SearchRecord(Account account) 
        {
            App.ServiceProvider.GetRequiredService<TeammateViewModel>().ShowRecord(account);
        }
    }
}
