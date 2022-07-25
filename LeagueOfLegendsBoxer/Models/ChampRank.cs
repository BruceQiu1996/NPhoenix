using LeagueOfLegendsBoxer.Resources;
using System.Linq;

namespace LeagueOfLegendsBoxer.Models
{
    public class ChampRank
    {
        public int Sort { get; set; }
        public int ChampId { get; set; }
        public string Name => Constant.Heroes.FirstOrDefault(x => x.ChampId == ChampId).Label;
        public string Avatar => $"https://game.gtimg.cn/images/lol/act/img/champion/{Constant.Heroes.FirstOrDefault(x => x.ChampId == ChampId).Alias}.png";
        public double Win { get; set; }
        public string WinStr => (Win * 100).ToString("0.0");
        public double Ban { get; set; }
        public string BanStr => (Ban * 100).ToString("0.0");
        public double Appearance { get; set; }
        public string AppearanceStr => Appearance.ToString("0.0");
        public double TLevel { get; set; }
        public string TLevelIcon => $"/Resources/Positions/t{TLevel}.svg";
    }
}
