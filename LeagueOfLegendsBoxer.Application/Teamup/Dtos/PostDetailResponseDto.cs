namespace LeagueOfLegendsBoxer.Application.Teamup.Dtos
{
    public class PostDetailResponseDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreateTime { get; set; }
        public PostCategory PostCategory { get; set; }
        public string Image_1 { get; set; }
        public string Image_2 { get; set; }
        public string Image_3 { get; set; }
        public int GoodCount { get; set; }//点赞数
        public int CommmentsCount { get; set; }//评论数
        public bool HadGood { get; set; } //查询者是否点赞
        //发布人信息
        public long Publisher { get; set; }
        public string UserName { get; set; }
        public string ServerArea { get; set; }
        public string Rank_SOLO_5x5 { get; set; }
        public string Rank_FLEX_SR { get; set; }
        public string Desc { get; set; }

        //前十条评论
        public IEnumerable<PostCommentDto> PostCommentDtos { get; set; }
    }

    public class PostCommentDto
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
    }
}
