using CommunityToolkit.Mvvm.ComponentModel;
using LeagueOfLegendsBoxer.Application.Teamup.Dtos;

namespace LeagueOfLegendsBoxer.Models
{
    public class PostDetail : ObservableObject
    {
        public long? Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public PostCategory PostCategory { get; set; }
        public string Image_1 { get; set; }
        public string Image_2 { get; set; }
        public string Image_3 { get; set; }
        public long Publisher { get; set; }
        public bool HasImages { get; set; }

    }
}
