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
        public string Image_1 { get; set; }
        public string Image_2 { get; set; }
        public string Image_3 { get; set; }
        public int GoodCount { get; set; }
        public int CommmentsCount { get; set; }
        public bool HadGood { get; set; }
        //发布人信息
        public long Publisher { get; set; }
        public string UserName { get; set; }
        public string Desc { get; set; }
    }
    public enum PostCategory
    {
        [Description("日常分享")]
        Daily, //日常
        [Description("资讯信息")]
        Information,//资讯
        [Description("开黑交友")]
        Teamup//开黑
    }
}
