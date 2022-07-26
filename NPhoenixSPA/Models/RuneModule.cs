﻿using NPhoenixSPA.Resources;
using System.Collections.Generic;
using System.Linq;

namespace NPhoenixSPA.Models
{
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
        public int Main1 { get; set; }
        public int Main2 { get; set; }
        public int Main3 { get; set; }
        public int Main4 { get; set; }
        public int Dputy1 { get; set; }
        public int Dputy2 { get; set; }
        public int Extra1 { get; set; }
        public int Extra2 { get; set; }
        public int Extra3 { get; set; }
        public Rune MainRune => Constant.Runes.FirstOrDefault(x => x.Id == Main);
        public Rune DputyRune => Constant.Runes.FirstOrDefault(x => x.Id == Dputy);
        public Rune Main1Rune => Constant.Runes.FirstOrDefault(x => x.Id == Main1);
        public Rune Main2Rune => Constant.Runes.FirstOrDefault(x => x.Id == Main2);
        public Rune Main3Rune => Constant.Runes.FirstOrDefault(x => x.Id == Main3);
        public Rune Main4Rune => Constant.Runes.FirstOrDefault(x => x.Id == Main4);
        public Rune Dputy1Rune => Constant.Runes.FirstOrDefault(x => x.Id == Dputy1);
        public Rune Dputy2Rune => Constant.Runes.FirstOrDefault(x => x.Id == Dputy2);
        public Rune Extra1Rune => Constant.Runes.FirstOrDefault(x => x.Id == Extra1);
        public Rune Extra2Rune => Constant.Runes.FirstOrDefault(x => x.Id == Extra2);
        public Rune Extra3Rune => Constant.Runes.FirstOrDefault(x => x.Id == Extra3);
        public double Popular { get; set; }
        public double WinRate { get; set; }
        public string PopularTxt => $"{(Popular * 100.0).ToString("0.0")}%";
        public string WinRateTxt => $"{(WinRate * 100.0).ToString("0.0")}%";
    }
}
