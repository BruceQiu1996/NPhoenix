namespace LeagueOfLegendsBoxer.Models
{
    public class Teammate
    {
        public long SummonerId { get; set; }
        public string SummonerName { get; set; }
        public string Puuid { get; set; }
        public int? TeamParticipantId { get; set; }
        public GameCustomization GameCustomization { get; set; }
        public int TeamId { get; set; }
        public string SummonerInternalName { get; set; }
    }

    public class GameCustomization 
    {
        public string Perks { get; set; }
    }

    public class PlayerChampionSelection 
    {
        public string SummonerInternalName { get; set; }
        public double Spell1Id { get; set; }
        public double Spell2Id { get; set; }
    }
}
