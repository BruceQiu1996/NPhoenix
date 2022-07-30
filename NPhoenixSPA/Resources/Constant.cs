using NPhoenixSPA.Models;
using System.Collections.Generic;

namespace NPhoenixSPA.Resources
{
    internal class Constant
    {
        public static string Token = string.Empty;
        public static int Port = 0;
        public static int Pid = 0;
        public static IEnumerable<Hero> Heroes = new List<Hero>();
        public static IEnumerable<Rune> Runes = new List<Rune>();
        public static IEnumerable<Item> Items = new List<Item>();
        public static IEnumerable<Icon> Icons = new List<Icon>();
        public static IEnumerable<ServerArea> ServerAreas = new List<ServerArea>();
        //ini settings sections
        public const string GameName = "LeagueClient";
        public const string Game = nameof(Game);
        public static Account Account = null;
        public const string AutoAcceptGame = nameof(AutoAcceptGame);
        public const string AutoLockHero = nameof(AutoLockHero);
        public const string AutoDisableHero = nameof(AutoDisableHero);
        public const string AutoLockHeroId = nameof(AutoLockHeroId);
        public const string AutoDisableHeroId = nameof(AutoDisableHeroId);
        public const string AutoLockHeroInAram = nameof(AutoLockHeroInAram);
        public const string LockHerosInAram = nameof(LockHerosInAram);
        public const string SubscribeFriendsList = nameof(SubscribeFriendsList);
        public const string GameLocation = nameof(GameLocation);
        public const string RankSetting = nameof(RankSetting);
        //event uri
        public const string GameFlow = @"/lol-gameflow/v1/gameflow-phase";
        public const string ChampSelect = @"/lol-champ-select/v1/session";
        public const string Avatar = "https://wegame.gtimg.com/g.26-r.c2d3c/helper/lol/assis/images/resources/usericon/{0}.png";
        public const string HeroAvatar = "https://game.gtimg.cn/images/lol/act/img/champion/{0}.png";
    }
}
