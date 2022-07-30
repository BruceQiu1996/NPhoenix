using NPhoenixSPA.Resources;
using System;

namespace NPhoenixSPA.Models
{
    public class Friend
    {
        public long SummonerId { get; set; }
        public string Puuid { get; set; }
        public string Name { get; set; }
        public long Time { get; set; }
        public int Icon { get; set; }
        public string Avatar  => string.Format(Constant.Avatar, Icon);
        public string Availability { get; set; }
        public string AvailabilityDesc => Availability == "offline" ? "不在线" : "在线";
        public string LastLoginTimeDesc => Time == 0 ? "暂无" : TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddMilliseconds(Time).ToString("g");
        public bool IsSubscribe { get; set; }
        public LolStatus LOL { get; set; }
    }

    public class LolStatus
    { 
        public int QueueId { get; set; }
        public long TimeStamp { get; set; }
        public string GameStatus { get; set; }
        public string GameStatusDesc => GameStatus == "inGame" ? "游戏中" : "准备中";
        public string TimeStampDesc => TimeStamp == 0 ? "暂无" : TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddMilliseconds(TimeStamp).ToString("g");
        public string CnQueue => QueueId switch
        {
            420 => "单双排",
            430 => "匹配",
            440 => "灵活排位",
            450 => "大乱斗",
            _ => "其他"
        };
    }
}
