using System.ComponentModel.DataAnnotations;

namespace LeagueOfLegendsBoxer.Application.Teamup.Dtos
{
    public class CreatePostCommentDto
    {
        public long PostId { get; set; }
        public string Content { get; set; }
    }
}
