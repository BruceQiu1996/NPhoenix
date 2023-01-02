using System;

namespace LeagueOfLegendsBoxer.Models
{
    public class PostComment
    {
        public long Publisher { get; set; }
        public DateTime CreateTime { get; set; }
        public string CommentContent { get; set; }
        //评论人信息
        public string UserName { get; set; }
        public string ServerArea { get; set; }
        public string Rank_SOLO_5x5 { get; set; }
        public string Rank_FLEX_SR { get; set; }
        public string Desc { get; set; }
        public int Index { get; set; }
        public string CreateTimeText => ConvertDateTimeToText(CreateTime);

        //TODO 统一
        public string ConvertDateTimeToText(DateTime dateTime)
        {
            if (dateTime.Year == DateTime.Now.Year && dateTime.DayOfYear == DateTime.Now.DayOfYear)
            {
                //今天
                return dateTime.ToString("t");
            }
            else if (dateTime.Year == DateTime.Now.Year)
            {
                //今年
                return dateTime.ToString("MM-dd HH:mm");
            }
            else
            {
                return dateTime.ToString("yyyy/MM/dd HH:mm");
            }
        }
    }
}
