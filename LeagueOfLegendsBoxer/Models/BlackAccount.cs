using System;

namespace LeagueOfLegendsBoxer.Models
{
    public class BlackAccount
    {
        public long Id { get; set; }
        public DateTime CreateTime { get; set; }
        public string Reason { get; set; }
        public string AccountName { get; set; }
    }
}
