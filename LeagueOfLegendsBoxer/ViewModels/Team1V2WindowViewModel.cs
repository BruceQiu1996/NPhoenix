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
            var teams = new List<IList<Account>>() { t1, t2 };
            foreach (var team in teams)
            {
                List<(int, int, int)> data1 = new List<(int, int, int)>();//1.下标，2下标，3重合率
                                                                          //分析过去的20局比赛 //todo过去100场
                if (team.Count >= 2)
                {
                    for (var i = 0; i < team.Count - 1; i++)
                    {
                        for (var j = i + 1; j < team.Count; j++)
                        {
                            var count = team[i].Records.Intersect(team[j].Records, new RecordEqual()).Count();
                            data1.Add((i, j, count));
                        }
                    }
                }

                List<List<int>> virteams = new List<List<int>>();
                foreach (var item in data1.Where(x => x.Item3 >= 2))
                {
                    var virteam = virteams.FirstOrDefault(x => x.Contains(item.Item1));
                    var virteam1 = virteams.FirstOrDefault(x => x.Contains(item.Item2));
                    if (virteam == null && virteam1 == null)
                    {
                        virteams.Add(new List<int>() { item.Item1, item.Item2 });
                    }
                    else if (virteam != null && virteam1 == null)
                    {
                        virteam.Add(item.Item2);
                    }
                    else if (virteam == null && virteam1 != null)
                    {
                        virteam1.Add(item.Item1);
                    }
                }

                var startIndex = virteams.Count;
                for (var i = 0; i < team.Count; i++)
                {
                    var virteam = virteams.FirstOrDefault(x => x.Contains(i));
                    if (virteam != null)
                    {
                        t1[i].TeamID = virteams.IndexOf(virteam) + 1;
                    }
                    else
                    {
                        t1[i].TeamID = ++startIndex;
                    }
                }
            }

            Team1Accounts = new ObservableCollection<Account>(t1);
            Team2Accounts = new ObservableCollection<Account>(t2);
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
