namespace LeagueOfLegendsBoxer.Models
{
    public class InternalSpell
    {
        public Spell SummonerSpellOne { get; set; }
        public Spell SummonerSpellTwo { get; set; }
    }

    public class Spell 
    {
        public string DisplayName { get; set; }
        public string RawDescription { get; set; }
    }

    public class SpellModel
    {
        public string Name { get; set; }
        public long Id { get; set; }
    }
}
