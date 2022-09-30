namespace LeagueOfLegendsBoxer.Application.Teamup.Dtos
{
    public class UserCreateOrUpdateByClientDto
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string Rank_SOLO_5x5 { get; set; }
        public string Rank_FLEX_SR { get; set; }
        public string Version { get; set; }
    }
}
