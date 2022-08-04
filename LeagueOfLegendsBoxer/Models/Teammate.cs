namespace LeagueOfLegendsBoxer.Models
{
    public class Teammate
    {
        public long SummonerId { get; set; }
        public string SummonerName { get; set; }
        public string Puuid { get; set; }
        public GameCustomization GameCustomization { get; set; }
    }

    public class GameCustomization 
    {
        public string Perks { get; set; }
    }
}
