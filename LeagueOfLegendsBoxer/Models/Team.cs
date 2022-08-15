using System.Collections.Generic;

namespace LeagueOfLegendsBoxer.Models
{
    public class Team
    {
        public int CellId { get; set; }
        public int ChampionId { get; set; }
        public long SummonerId { get; set; }
        public string AssignedPosition { get; set; }
    }

    public class Ban
    {
        public int[] MyTeamBans { get; set; }
        public int[] TheirTeamBans { get; set; }
    }
}
