using System.Text.Json.Serialization;


namespace LeagueOfLegendsBoxer.Application.Event
{
    public class EventArgument
    {
        [JsonPropertyName("data")]
        public dynamic Data { get; set; }

        [JsonPropertyName("eventType")]
        public string EventType { get; set; }

        [JsonPropertyName("uri")]
        public string Uri { get; set; }
    }
}
