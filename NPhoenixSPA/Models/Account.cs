using CommunityToolkit.Mvvm.ComponentModel;
using NPhoenixSPA.Resources;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;

namespace NPhoenixSPA.Models
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

        public string Horse => GetHorse();
        private string GetHorse()
        {
            if (Records == null || Records.Count <= 0)
                return "未知的马";

            var score = Records.Average(x => x.GetScore());
            if (score >= 120)
            {
                return "上等马";
            }
            else if (score >= 110)
            {
                return "中等马";
            }
            else if (score >= 100)
            {
                return "下等马";
            }
            else if (score < 100)
            {
                return "牛马";
            }

            return "未知的马";
        }
    }
}
