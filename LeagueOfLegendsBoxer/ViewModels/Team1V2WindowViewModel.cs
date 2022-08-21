using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LeagueOfLegendsBoxer.Models;
using LeagueOfLegendsBoxer.Resources;
using LeagueOfLegendsBoxer.Windows;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueOfLegendsBoxer.ViewModels
{
    public class Team1V2WindowViewModel : ObservableObject
    {
        public RelayCommand<Account> SearchAccountRecordCommand { get; set; }

        public ObservableCollection<Account> _team1Accounts;
        public ObservableCollection<Account> Team1Accounts
        {
            get { return _team1Accounts; }
            set { SetProperty(ref _team1Accounts, value); }
        }

        public ObservableCollection<Account> _team2Accounts;
        public ObservableCollection<Account> Team2Accounts
        {
            get { return _team2Accounts; }
            set { SetProperty(ref _team2Accounts, value); }
        }

        private readonly IniSettingsModel _iniSettingsModel;
        public Team1V2WindowViewModel(IniSettingsModel iniSettingsModel)
        {
            SearchAccountRecordCommand = new RelayCommand<Account>(SearchAccountRecord);
            _iniSettingsModel = iniSettingsModel;
        }

        public void LoadData(IList<Account> t1, IList<Account> t2)
        {
            Team1Accounts = new ObservableCollection<Account>(t1);
            Team2Accounts = new ObservableCollection<Account>(t2);

            foreach (var item in Team1Accounts.Concat(Team2Accounts))
            {
                item.IsInBlackList = _iniSettingsModel.BlackAccounts?.FirstOrDefault(x => x.Id == item.SummonerId) != null;
                if (item.IsInBlackList) 
                {
                    var sb = new StringBuilder();
                    var records = _iniSettingsModel.BlackAccounts?.Where(x => x.Id == item.SummonerId);
                    foreach (var record in records.OrderByDescending(x=>x.CreateTime)) 
                    {
                        sb.Append(record.Reason + record.CreateTime.ToString("d"));
                        sb.Append("\n");
                    }

                    item.BlackInfo = sb.ToString();
                }
            }
        }

        private async void SearchAccountRecord(Account account)
        {
            var summonerAnalyse = App.ServiceProvider.GetRequiredService<SummonerAnalyse>();
            var summonerAnalyseViewModel = App.ServiceProvider.GetRequiredService<SummonerAnalyseViewModel>();
            summonerAnalyseViewModel.Account = account;
            summonerAnalyse.DataContext = summonerAnalyseViewModel;
            summonerAnalyse.Topmost = true;
            summonerAnalyse.Show();
            await Task.Delay(1000);
            summonerAnalyse.Topmost = false;
        }
    }
}
