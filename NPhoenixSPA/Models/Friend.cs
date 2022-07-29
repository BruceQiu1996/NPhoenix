using System;

namespace NPhoenixSPA.Models
{
    public class Friend
    {
        public long SummonerId { get; set; }
        public string Puuid { get; set; }
        public string Name { get; set; }
        public long Time { get; set; }
        public string Availability { get; set; }
        public string AvailabilityDesc => Availability == "offline" ? "不在线" : "在线";
        public string LastLoginTimeDesc => Time == 0 ? "暂无" : TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddMilliseconds(Time).ToString("g");
        public bool IsSubscribe { get; set; }
    }
}
