using CommunityToolkit.Mvvm.ComponentModel;
using LeagueOfLegendsBoxer.Resources;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;

namespace LeagueOfLegendsBoxer.Models
{
    public class Account : ObservableObject
    {
        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }
        [JsonPropertyName("summonerId")]
        public long SummonerId { get; set; }
        [JsonPropertyName("summonerLevel")]
        public int SummonerLevel { get; set; }
        [JsonPropertyName("puuid")]
        public string Puuid { get; set; }
        [JsonPropertyName("xpSinceLastLevel")]
        public int XpSinceLastLevel { get; set; }
        [JsonPropertyName("xpUntilNextLevel")]
        public int XpUntilNextLevel { get; set; }
        [JsonPropertyName("profileIconId")]
        public int ProfileIconId { get; set; }
        public string Avatar => string.Format(Constant.Avatar, ProfileIconId);
        public string XpTip => $"经验值:{XpSinceLastLevel}/{XpUntilNextLevel}";

        private Rank _rank;
        public Rank Rank
        {
            get => _rank;
            set => SetProperty(ref _rank, value);
        }

        private ObservableCollection<Record> _records;
        public ObservableCollection<Record> Records
        {
            get => _records;
            set => SetProperty(ref _records, value);
        }

        public int TeamID { get; set; }
        public string Horse => GetHorse();
        private string GetHorse()
        {
            if (Records == null || Records.Count <= 0)
                return "未知的马";

            var score = Records.Where(x => x.CnQueue != "其他").Average(x => x.GetScore());
            if (score >= 120)
            {
                return App.ServiceProvider.GetRequiredService<IniSettingsModel>().Above120ScoreTxt;
            }
            else if (score >= 110)
            {
                return App.ServiceProvider.GetRequiredService<IniSettingsModel>().Above110ScoreTxt;
            }
            else if (score >= 100)
            {
                return App.ServiceProvider.GetRequiredService<IniSettingsModel>().Above100ScoreTxt;
            }
            else if (score < 100)
            {
                return App.ServiceProvider.GetRequiredService<IniSettingsModel>().Below100ScoreTxt;
            }

            return "未知的马";
        }

        //live
        private Hero _champion;
        public Hero Champion
        {
            get => _champion;
            set => SetProperty(ref _champion, value);
        }

        private ObservableCollection<Rune> runes;
        public ObservableCollection<Rune> Runes 
        {
            get => runes;
            set => SetProperty(ref runes, value);
        }
        //blacklist
        private bool _isOpenBlack;
        public bool IsOpenBlack
        {
            get => _isOpenBlack;
            set => SetProperty(ref _isOpenBlack, value);
        }

        public string _reason;
        public string Reason
        {
            get { return _reason; }
            set { SetProperty(ref _reason, value); }
        }

        public bool IsCurrentUser => Constant.Account.SummonerId == SummonerId;
        public bool IsInBlackList { get; set; }
        public string BlackInfo{ get; set; }
        public Account ShallowCopy() 
        {
            return this.MemberwiseClone() as Account;
        }
    }
}
