using CommunityToolkit.Mvvm.ComponentModel;
using LeagueOfLegendsBoxer.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace LeagueOfLegendsBoxer.ViewModels
{
    public class Team1V2WindowViewModel : ObservableObject
    {
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

        public Team1V2WindowViewModel()
        {

        }

        public void LoadData(IList<Account> t1, IList<Account> t2)
        {
            AnalyseTeamID(t1, t2);

            Team1Accounts = new ObservableCollection<Account>(t1);
            Team2Accounts = new ObservableCollection<Account>(t2);
        }

        public void AnalyseTeamID(IList<Account> t1, IList<Account> t2) 
        {
            
        }

        public class RecordEqual : IEqualityComparer<Record>
        {
            public bool Equals(Record x, Record y)
            {
                return x.GameId == y.GameId && x.Participants.FirstOrDefault()?.TeamId == y.Participants.FirstOrDefault()?.TeamId;
            }

            public int GetHashCode([DisallowNull] Record obj)
            {
                return obj.ToString().GetHashCode();
            }
        }
    }
}
