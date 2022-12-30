namespace LeagueOfLegendsBoxer.Application.Teamup.Dtos
{
    public class PostCreateOrUpdateDto
    {
        public long? Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreateTime { get; set; }
        public PostCategory PostCategory { get; set; }
        public string Image_1 { get; set; }
        public string Image_2 { get; set; }
        public string Image_3 { get; set; }
        //发布人信息
        public long Publisher { get; set; }
        public string UserName { get; set; }
        public string ServerArea { get; set; }
        public string Rank_SOLO_5x5 { get; set; }
        public string Rank_FLEX_SR { get; set; }
        public string Desc { get; set; }
    }
}
