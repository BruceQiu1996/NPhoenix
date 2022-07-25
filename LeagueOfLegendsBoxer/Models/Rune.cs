using System.Text.Json.Serialization;

namespace LeagueOfLegendsBoxer.Models
{
    public class Rune
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("icon")]
        public string Icon { get; set; }
        [JsonPropertyName("tooltip")]
        public string Tooltip { get; set; }
        [JsonPropertyName("shortdesc")]
        public string Shortdesc { get; set; }
        public int Id { get; set; }
        public string Image => $"/Resources/Runes/{Id}.png";
    }
}
