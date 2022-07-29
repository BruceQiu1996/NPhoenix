using NPhoenixSPA.Resources;
using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace NPhoenixSPA.Models
{
    public class Champ
    {
        [JsonPropertyName("championId")]
        public int ChampionId { get; set; }
        [JsonPropertyName("championLevel")]
        public int ChampionLevel { get; set; }
        [JsonPropertyName("championPoints")]
        public int ChampionPoints { get; set; }
        public int Rank { get; set; }
        public string ChampionLevelDesc => "英雄等级:" + ChampionLevel;
        public string ChampionPointsDesc => "熟练度:" + ChampionPoints;
        [JsonPropertyName("lastPlayTime")]
        public long LastPlayTime { get; set; }
        public string LastPlayTimeString => TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddMilliseconds(LastPlayTime).ToString("d");
        public string ChampImage => $"https://game.gtimg.cn/images/lol/act/img/champion/{Constant.Heroes.FirstOrDefault(x => x.ChampId == ChampionId)?.Alias}.png";
    }
}
