using LeagueOfLegendsBoxer.Resources;
using System.Linq;

namespace LeagueOfLegendsBoxer.Models
{
    public class AramChampDescModel
    {
        public int Id { get; set; }
        public string ChampImage => $"https://game.gtimg.cn/images/lol/act/img/champion/{Constant.Heroes.FirstOrDefault(x => x.ChampId == Id)?.Alias}.png";
        public AramBuff Buff { get; set; }
        
    }
}
