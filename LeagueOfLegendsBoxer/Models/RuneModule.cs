using CommunityToolkit.Mvvm.ComponentModel;
using LeagueOfLegendsBoxer.Resources;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace LeagueOfLegendsBoxer.Models
{
    public class HeroRecommandModule 
    {
        public RuneModule Rune { get; set; }
        public ItemModule Item { get; set; }
    }

    public class RuneModule
    {
        public int ChampId { get; set; }
        public IList<RuneDetail> Common { get; set; }
        public IList<RuneDetail> Aram { get; set; }
        public RuneModule()
        {
            Common = new List<RuneDetail>();
            Aram = new List<RuneDetail>();
        }
    }

    public class RuneDetail
    {
        public int Main { get; set; }
        public int Dputy { get; set; }
        public string Name { get; set; }
        public int Main1 { get; set; }
        public int Main2 { get; set; }
        public int Main3 { get; set; }
        public int Main4 { get; set; }
        public int Dputy1 { get; set; }
        public int Dputy2 { get; set; }
        public int Extra1 { get; set; }
        public int Extra2 { get; set; }
        public int Extra3 { get; set; }
        [JsonIgnore]
        public Rune MainRune => Constant.Runes.FirstOrDefault(x => x.Id == Main);
        [JsonIgnore]
        public Rune DputyRune => Constant.Runes.FirstOrDefault(x => x.Id == Dputy);
        [JsonIgnore]
        public Rune Main1Rune => Constant.Runes.FirstOrDefault(x => x.Id == Main1);
        [JsonIgnore]
        public Rune Main2Rune => Constant.Runes.FirstOrDefault(x => x.Id == Main2);
        [JsonIgnore]
        public Rune Main3Rune => Constant.Runes.FirstOrDefault(x => x.Id == Main3);
        [JsonIgnore]
        public Rune Main4Rune => Constant.Runes.FirstOrDefault(x => x.Id == Main4);
        [JsonIgnore]
        public Rune Dputy1Rune => Constant.Runes.FirstOrDefault(x => x.Id == Dputy1);
        [JsonIgnore]
        public Rune Dputy2Rune => Constant.Runes.FirstOrDefault(x => x.Id == Dputy2);
        [JsonIgnore]
        public Rune Extra1Rune => Constant.Runes.FirstOrDefault(x => x.Id == Extra1);
        [JsonIgnore]
        public Rune Extra2Rune => Constant.Runes.FirstOrDefault(x => x.Id == Extra2);
        [JsonIgnore]
        public Rune Extra3Rune => Constant.Runes.FirstOrDefault(x => x.Id == Extra3);
        [JsonIgnore]
        public bool IsCustomer { get; set; }
        public double Popular { get; set; }
        public double WinRate { get; set; }
        public string PopularTxt => $"{(Popular * 100.0).ToString("0.0")}%";
        public string WinRateTxt => $"{(WinRate * 100.0).ToString("0.0")}%";
        public bool IsAutoApply { get; set; }
    }
    public class ItemModule
    {
        public int ChampId { get; set; }
        public ItemsDetail Common { get; set; }
        public ItemsDetail Aram { get; set; }
    }

    public class ItemsDetail : ObservableObject
    {
        public List<ItemDetail> StartItems { get; set; } = new List<ItemDetail>();
        public List<ItemDetail> CoreItems { get; set; } = new List<ItemDetail>();
        public List<ItemDetail> ShoeItems { get; set; } = new List<ItemDetail>();

        private ItemDetail _startItem;
        public ItemDetail StartItem 
        {
            get => _startItem;
            set => SetProperty(ref _startItem, value);
        }

        private ItemDetail _coreItem;
        public ItemDetail CoreItem
        {
            get => _coreItem;
            set => SetProperty(ref _coreItem, value);
        }

        private ItemDetail _shoeItem;
        public ItemDetail ShoeItem
        {
            get => _shoeItem;
            set => SetProperty(ref _shoeItem, value);
        }
    }

    public class ItemDetail
    {
        public List<int> ItemIds { get; set; } = new List<int>();
        public double Popular { get; set; }
        public double WinRate { get; set; }
        public string PopularTxt => $"{(Popular * 100.0).ToString("0.0")}%";
        public string WinRateTxt => $"{(WinRate * 100.0).ToString("0.0")}%";
        public IEnumerable<Item> Items => Constant.Items.Where(x => ItemIds.Contains(x.Id));
    }
}
