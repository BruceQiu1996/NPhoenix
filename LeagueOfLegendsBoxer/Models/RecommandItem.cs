using System;
using System.Collections.Generic;

namespace LeagueOfLegendsBoxer.Models
{
    public class RecommandItem
    {
        public string uid { get; set; } = Guid.NewGuid().ToString();
        public string mode { get; set; } = "any";
        public string champion { get; set; }
        public bool priority { get; set; }
        public int[] associatedMaps { get; set; }
        public string map { get; set; } = "any";
        public string title { get; set; }
        public int sortrank { get; set; }
        public bool isGlobalForMaps { get; set; }
        public string type { get; set; } = "custom";
        public bool isGlobalForChampions { get; set; } = true;
        public List<Block> blocks { get; set; } = new List<Block>();
    }

    public class Block 
    {
        public string type { get; set; }
        public List<RItem> items { get; set; } = new List<RItem>();
    }

    public class RItem 
    {
        public string id { get; set; }
        public int count { get; set; } = 1;
    }
}
