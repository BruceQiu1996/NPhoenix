using System.Collections.Generic;

namespace LeagueOfLegendsBoxer.Models
{
    /// <summary>
    /// 大乱斗分析数据模型
    /// </summary>
    public class AramChooseHeroModel
    {
        public IEnumerable<int> ChampIds { get; init; }
        public IEnumerable<int> BenchChamps { get; init; }

        public AramChooseHeroModel(IEnumerable<int> champIds, IEnumerable<int> benchChamps)
        {
            ChampIds = champIds;
            BenchChamps = benchChamps;
        }
    }
}
