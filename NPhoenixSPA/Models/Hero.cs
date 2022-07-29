using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace NPhoenixSPA.Models
{
    public class Hero
    {
        [JsonProperty("heroId")]
        public int ChampId { get; set; }
        [JsonProperty("name")]
        public string Label { get; set; }
        [JsonProperty("alias")]
        public string Alias { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        public string Name => $"{Label} {Title}";
        public string Avatar => $"https://game.gtimg.cn/images/lol/act/img/champion/{Alias}.png";
    }
}
