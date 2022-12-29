namespace LeagueOfLegendsBoxer.Application.Teamup.Dtos
{
    public class PostResponsePageDto
    {
        public int Count { get; set; }
        public IEnumerable<PostResponseDto> Data { get; set; }
    }
}
