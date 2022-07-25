using LeagueOfLegendsBoxer.Resources;
using System.Windows.Media;

namespace LeagueOfLegendsBoxer.Models
{
    public class Skin
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SplashPath { get; set; }
        public string Description { get; set; }
        public ImageSource Image { get; set; }
    }
}
