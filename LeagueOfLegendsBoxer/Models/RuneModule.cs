using LeagueOfLegendsBoxer.Resources;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace LeagueOfLegendsBoxer.Models
{
    public class HeroRecommandModule
    {
        public IEnumerable<RuneModule> Perk { get; set; }
        public IEnumerable<ItemModule> Equip { get; set; }
    }

    public class RuneModule
    {
        public long Id { get; set; }
        public int Champion_id { get; set; }
        public string Title { get; set; }
        public string Lane { get; set; }
        public string CnLane => Lane.ToUpper() switch
        {
            "MID" => "中单",
            "JUNGLE" => "打野",
            "BOTTOM" => "下路",
            "SUPPORT" => "辅助",
            "TOP" => "上路",
            _ => "未知"
        };
        public int GameType { get; set; } //1.5v5 2.大乱斗
        public int PrimaryStyleId { get; set; }
        public int SubStyleId { get; set; }
        public int[] SelectedPerkIds { get; set; }
        public int Showrate { get; set; }
        public int Winrate { get; set; }
        [JsonIgnore]
        public Rune MainRune => Constant.Runes.FirstOrDefault(x => x.Id == PrimaryStyleId);
        [JsonIgnore]
        public Rune DputyRune => Constant.Runes.FirstOrDefault(x => x.Id == SubStyleId);
        [JsonIgnore]
        public Rune Main1Rune => Constant.Runes.FirstOrDefault(x => x.Id == SelectedPerkIds[0]);
        [JsonIgnore]
        public Rune Main2Rune => Constant.Runes.FirstOrDefault(x => x.Id == SelectedPerkIds[1]);
        [JsonIgnore]
        public Rune Main3Rune => Constant.Runes.FirstOrDefault(x => x.Id == SelectedPerkIds[2]);
        [JsonIgnore]
        public Rune Main4Rune => Constant.Runes.FirstOrDefault(x => x.Id == SelectedPerkIds[3]);
        [JsonIgnore]
        public Rune Dputy1Rune => Constant.Runes.FirstOrDefault(x => x.Id == SelectedPerkIds[4]);
        [JsonIgnore]
        public Rune Dputy2Rune => Constant.Runes.FirstOrDefault(x => x.Id == SelectedPerkIds[5]);
        [JsonIgnore]
        public Rune Extra1Rune => Constant.Runes.FirstOrDefault(x => x.Id == SelectedPerkIds[6]);
        [JsonIgnore]
        public Rune Extra2Rune => Constant.Runes.FirstOrDefault(x => x.Id == SelectedPerkIds[7]);
        [JsonIgnore]
        public Rune Extra3Rune => Constant.Runes.FirstOrDefault(x => x.Id == SelectedPerkIds[8]);
        [JsonIgnore]

        public bool IsCustomer { get; set; } //是否是自定义的符文
        public bool IsAutoApply { get; set; }//是否自动应用符文
        public string PopularTxt => $"{(Showrate / 100.0).ToString("0.0")}%";
        public string WinRateTxt => $"{(Winrate / 100.0).ToString("0.0")}%";
    }

    public class ItemModule
    {
        public long Id { get; set; }
        public string Lane { get; set; }
        public string CnLane => Lane.ToUpper() switch
        {
            "MID" => "中单",
            "JUNGLE" => "打野",
            "BOTTOM" => "下路",
            "SUPPORT" => "辅助",
            "TOP" => "上路",
            _ => "未知"
        };
        public int Champion_id { get; set; }
        public int Map_id { get; set; }
        public string Title { get; set; }
        public string Item1 { get; set; }
        public string Item2 { get; set; }
        public string Item3 { get; set; }
        public int Showrate { get; set; }
        public int Winrate { get; set; }
        public string PopularTxt => $"{(Showrate / 100.0).ToString("0.0")}%";
        public string WinRateTxt => $"{(Winrate / 100.0).ToString("0.0")}%";
        public IEnumerable<Item> Item1s => string.IsNullOrEmpty(Item1) ? null : Constant.Items.Where(x => Item1.Split(";").Contains(x.Id.ToString()));
        public IEnumerable<Item> Item2s => string.IsNullOrEmpty(Item1) ? null : Constant.Items.Where(x => Item2.Split(";").Contains(x.Id.ToString()));
        public IEnumerable<Item> Item3s => string.IsNullOrEmpty(Item1) ? null : Constant.Items.Where(x => Item3.Split(";").Contains(x.Id.ToString()));
    }
}
