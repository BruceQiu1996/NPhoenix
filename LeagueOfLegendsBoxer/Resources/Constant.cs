using LeagueOfLegendsBoxer.Models;
using System.Collections.Generic;

namespace LeagueOfLegendsBoxer.Resources
{
    internal class Constant
    {
        public static string Token = string.Empty;
        public static int Port = 0;
        public static int Pid = 0;
        public static IEnumerable<Hero> Heroes = new List<Hero>();
        public static IEnumerable<Rune> Runes = new List<Rune>();
        public static IEnumerable<Item> Items = new List<Item>();
        public static IEnumerable<ServerArea> ServerAreas = new List<ServerArea>();
        public static Account Account = null;
        //ini settings sections
        public const string GameName = "LeagueClient";
        public const string Game = nameof(Game);
        public const string AutoAcceptGame = nameof(AutoAcceptGame);
        public const string AutoLockHero = nameof(AutoLockHero);
        public const string AutoDisableHero = nameof(AutoDisableHero);
        public const string AutoLockHeroId = nameof(AutoLockHeroId);
        public const string AutoDisableHeroId = nameof(AutoDisableHeroId);
        public const string AutoLockHeroInAram = nameof(AutoLockHeroInAram);
        public const string LockHerosInAram = nameof(LockHerosInAram);
        public const string GameLocation = nameof(GameLocation);
        public const string RankSetting = nameof(RankSetting);
        public const string IsCloseRecommand = nameof(IsCloseRecommand);
        public const string CloseSendOtherWhenBegin = nameof(CloseSendOtherWhenBegin);
        public const string HorseTemplate = nameof(HorseTemplate);
        public const string ReadedNotice = nameof(ReadedNotice);
        public const string AutoAcceptGameDelay = nameof(AutoAcceptGameDelay);
        public const string Above120ScoreTxt = nameof(Above120ScoreTxt);
        public const string Above110ScoreTxt = nameof(Above110ScoreTxt);
        public const string Above100ScoreTxt = nameof(Above100ScoreTxt);
        public const string Below100ScoreTxt = nameof(Below100ScoreTxt);
        public static string _chatId = null;
        //event uri
        public const string GameFlow = @"/lol-gameflow/v1/gameflow-phase";
        public const string DataStore = @"/data-store/v1/install-settings/gameflow-patcher-lock";
        public const string ChampSelect = @"/lol-champ-select/v1/session";
        public const string Avatar = "https://wegame.gtimg.com/g.26-r.c2d3c/helper/lol/assis/images/resources/usericon/{0}.png";
        public const string HeroAvatar = "https://game.gtimg.cn/images/lol/act/img/champion/{0}.png";
        //info
        public const string Name = "{name}";
        public const string Horse = "{horse}";
        public const string Kda = "{kda}";
        public const string Solorank = "{solorank}";
        public const string SolorankDetail = "{solorankDetail}";
        public const string Flexrank = "{flexrank}";
        public const string FlexrankDetail = "{flexrankDetail}";
        public const string Score = "{score}";
    }
}
