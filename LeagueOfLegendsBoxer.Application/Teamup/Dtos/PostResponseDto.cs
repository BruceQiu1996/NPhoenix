using System.ComponentModel;

namespace LeagueOfLegendsBoxer.Application.Teamup.Dtos
{
    public class PostResponseDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreateTime { get; set; }
        public PostCategory PostCategory { get; set; }
        public bool IsTop { get; set; }
        public bool IsDeleted { get; set; }
        public string PublisherName { get; set; }
        public string Tag { get; set; }
        public int GoodCount { get; set; }
        public bool HadGood { get; set; }
    }

    public enum PostCategory
    {
        [Description("赛事资讯")]
        Race, //赛事
        [Description("游戏资讯")]
        Information,//资讯
        [Description("其他事件")]
        Other,//其他
    }
}
