using Newtonsoft.Json;

namespace LeagueOfLegendsBoxer.Models
{
    public class RuneModel
    {
        public string Name { get; set; }
        public long Id { get; set; }
        public bool IsDeletable { get; set; }
        public bool Current { get; set; }
        public int PrimaryStyleId { get; set; }
        public int SubStyleId { get; set; }
        public int[] SelectedPerkIds { get; set; }
    }

    public class CreateRuneModel
    {
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("current")]
        public bool current { get; set; }
        [JsonProperty("primaryStyleId")]
        public int primaryStyleId { get; set; }
        [JsonProperty("subStyleId")]
        public int subStyleId { get; set; }
        [JsonProperty("selectedPerkIds")]
        public int[] selectedPerkIds { get; set; }
    }

    public class GetRuneModel
    {
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("current")]
        public bool current { get; set; }
        [JsonProperty("isValid")]
        public bool IsValid { get; set; }
        [JsonProperty("primaryStyleId")]
        public int primaryStyleId { get; set; }
        [JsonProperty("subStyleId")]
        public int subStyleId { get; set; }
        [JsonProperty("selectedPerkIds")]
        public int[] selectedPerkIds { get; set; }
    }
}
