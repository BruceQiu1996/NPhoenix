namespace LeagueOfLegendsBoxer.Models
{
    public class ChatMessage
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string Desc { get; set; }
        public string ServerArea { get; set; }
        public string Rank_SOLO_5x5 { get; set; }
        public string Rank_FLEX_SR { get; set; }
        public string Message { get; set; }
        public bool IsSender { get; set; }
        public bool IsAdministrator { get; set; }
        public string Role { get; set; }
    }
}
