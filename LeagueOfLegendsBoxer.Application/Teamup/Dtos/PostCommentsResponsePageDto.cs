namespace LeagueOfLegendsBoxer.Application.Teamup.Dtos
{
    public class PostCommentsResponsePageDto
    {
        public int Count { get; set; }
        public IEnumerable<PostCommentDto> Data { get; set; }
    }
}
