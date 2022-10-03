using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LeagueOfLegendsBoxer.Models
{
    public class TotalRank 
    {
        public ObservableCollection<RankModel> Mvp { get; set; }
        public ObservableCollection<RankModel> Svp { get; set; }
        public ObservableCollection<RankModel> Noob { get; set; }
        public ObservableCollection<RankModel> Xiagu { get; set; }
        public ObservableCollection<RankModel> Aram { get; set; }
    }

    public class RankModel
    {
        public int Rank { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string Desc { get; set; }
        public int Times { get; set; }
    }
}
