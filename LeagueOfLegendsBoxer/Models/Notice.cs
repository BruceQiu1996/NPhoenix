namespace LeagueOfLegendsBoxer.Models
{
    public class Notice
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public long Time { get; set; }
        public int Priority { get; set; }
    }
}
