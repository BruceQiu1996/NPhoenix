using NPhoenixSPA.Resources;
using System.Linq;

namespace NPhoenixSPA.Models
{
    public class ChampRestraint
    {
        public string Championid2 { get; set; }
        public int Championid_2 => int.Parse(Championid2);
        public string Name => Constant.Heroes.FirstOrDefault(x => x.ChampId == Championid_2).Label;
        public string Avatar => $"https://game.gtimg.cn/images/lol/act/img/champion/{Constant.Heroes.FirstOrDefault(x => x.ChampId == Championid_2).Alias}.png";
        public string Winrate { get; set; }
        public int WinrateValue { get; set; }
        public string WinrateStr => "胜率:" + ((10000 - WinrateValue) / 100.0).ToString("0.0") + "%";
        public bool AboveHalfRate => WinrateValue <= 5000;
    }
}
