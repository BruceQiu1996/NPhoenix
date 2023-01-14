using LeagueOfLegendsBoxer.Application.ApplicationControl;
using LeagueOfLegendsBoxer.Application.Settings;
using LeagueOfLegendsBoxer.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace LeagueOfLegendsBoxer.Resources
{
    public class IniSettingsModel
    {
        private readonly ISettingsService _settingsService;
        private readonly IApplicationService _applicationService;
        private readonly IConfiguration _configuration;
        private readonly string _blackListLoc = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/blackList.json");
        private readonly string _fuckLoc = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/fuckwords.txt");
        private readonly string _goodLoc = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/goodwords.txt");

        public List<BlackAccount> BlackAccounts { get; set; }
        public bool AutoAcceptGame { get; set; }
        public bool AutoStartGame { get; set; }
        public bool AutoLockHero { get; set; }
        public bool RankAutoLockHero { get; set; }
        public bool AutoDisableHero { get; set; }
        public bool AutoEndGame { get; set; }
        public bool AutoLockHeroInAram { get; set; }
        public bool IsDarkTheme { get; set; }
        public List<int> LockHerosInAram { get; set; }
        public List<int> ReadedNotices { get; set; }
        public string FuckWords { get; set; }
        public string GoodWords { get; set; }
        public List<string> FuckWordCollection { get; set; }
        public List<string> GoodWordCollection { get; set; }
        public int AutoDisableChampId { get; set; }
        public int AutoLockHeroChampId { get; set; }
        public int TopAutoLockHeroChampId1 { get; set; }
        public int JungleAutoLockHeroChampId1 { get; set; }
        public int MiddleAutoLockHeroChampId1 { get; set; }
        public int BottomAutoLockHeroChampId1 { get; set; }
        public int UtilityAutoLockHeroChampId1 { get; set; }
        public int TopAutoLockHeroChampId2 { get; set; }
        public int JungleAutoLockHeroChampId2 { get; set; }
        public int MiddleAutoLockHeroChampId2 { get; set; }
        public int BottomAutoLockHeroChampId2 { get; set; }
        public int UtilityAutoLockHeroChampId2 { get; set; }
        public string GameExeLocation { get; set; }
        public string RankSetting { get; set; }
        public bool CloseSendOtherWhenBegin { get; set; }
        public string HorseTemplate { get; set; }
        public string ChatMessageTemplate { get; set; }
        public string Above120ScoreTxt { get; set; }
        public string Above110ScoreTxt { get; set; }
        public string Above100ScoreTxt { get; set; }
        public string Below100ScoreTxt { get; set; }
        public int AutoAcceptGameDelay { get; set; }
        public bool IsAltQOpenVsDetail { get; set; }
        public string TeamDetailKeys { get; set; }
        public bool AutoStartWhenComputerRun { get; set; }
        public bool AutoUseRuneByUseCount { get; set; }
        public bool AutoUseRuneByWinRate { get; set; }
        public bool AutoUseRune { get; set; }
        public long TeamDetailHash { get; set; }
        public List<string> TeamDetailKeyList { get; set; } = new List<string>();

        public IniSettingsModel(ISettingsService settingsService,
                                IApplicationService applicationService,
                                IConfiguration configuration)
        {
            _applicationService = applicationService;
            _settingsService = settingsService;
            _configuration = configuration;
            LockHerosInAram = new List<int>();
        }

        public async Task Initialize()
        {
            var file = _configuration.GetSection("SettingsFileLocation").Value;
            if (string.IsNullOrEmpty(file))
                throw new ArgumentNullException("can't find init file section in appsettings.json");

            await _settingsService.Initialize(file);
            AutoAcceptGame = bool.TryParse(
                await _settingsService.ReadAsync(Constant.Game, Constant.AutoAcceptGame), out var tempAutoAcceptGame) ? tempAutoAcceptGame : false;
            AutoEndGame = bool.TryParse(
                await _settingsService.ReadAsync(Constant.Game, Constant.AutoEndGame), out var tempAutoEndGame) ? tempAutoEndGame : false;
            AutoStartGame = bool.TryParse(
                await _settingsService.ReadAsync(Constant.Game, Constant.AutoStartGame), out var tempAutoStartGame) ? tempAutoStartGame : false;
            AutoLockHero = bool.TryParse(
                await _settingsService.ReadAsync(Constant.Game, Constant.AutoLockHero), out var tempAutoLockHero) ? tempAutoLockHero : false;
            RankAutoLockHero = bool.TryParse(
                await _settingsService.ReadAsync(Constant.Game, Constant.RankAutoLockHero), out var tempRankAutoLockHero) ? tempRankAutoLockHero : false;
            AutoDisableHero = bool.TryParse(
                await _settingsService.ReadAsync(Constant.Game, Constant.AutoDisableHero), out var tempAutoDisableHero) ? tempAutoDisableHero : false;
            AutoLockHeroInAram = bool.TryParse(
                await _settingsService.ReadAsync(Constant.Game, Constant.AutoLockHeroInAram), out var tempAutoLockHeroInAram) ? tempAutoLockHeroInAram : false;
            AutoStartWhenComputerRun = bool.TryParse(
                await _settingsService.ReadAsync(Constant.Game, Constant.AutoStartWhenComputerRun), out var tempAutoStartWhenComputerRun) ? tempAutoStartWhenComputerRun : false;
            AutoUseRuneByUseCount = bool.TryParse(
                await _settingsService.ReadAsync(Constant.Game, Constant.AutoUseRuneByUseCount), out var tempAutoUseRuneByUseCount) ? tempAutoUseRuneByUseCount : false;
            AutoUseRuneByWinRate = bool.TryParse(
                await _settingsService.ReadAsync(Constant.Game, Constant.AutoUseRuneByWinRate), out var tempAutoUseRuneByWinRate) ? tempAutoUseRuneByWinRate : false;
            AutoUseRune = bool.TryParse(
                await _settingsService.ReadAsync(Constant.Game, Constant.AutoUseRune), out var tempAutoUseRune) ? tempAutoUseRune : false;
            var lockHerosInAramData = await _settingsService.ReadAsync(Constant.Game, Constant.LockHerosInAram);
            LockHerosInAram = string.IsNullOrEmpty(lockHerosInAramData) ? new List<int>() : JsonSerializer.Deserialize<List<int>>(lockHerosInAramData);
            AutoLockHeroChampId = int.TryParse(
                await _settingsService.ReadAsync(Constant.Game, Constant.AutoLockHeroId), out var tempAutoLockHeroId) ? tempAutoLockHeroId : 0;

            TopAutoLockHeroChampId1 = int.TryParse(
                await _settingsService.ReadAsync(Constant.Game, Constant.TopAutoLockHeroChampId1), out var tempTopAutoLockHeroChampId1) ? tempTopAutoLockHeroChampId1 : 0;
            TopAutoLockHeroChampId2 = int.TryParse(
                await _settingsService.ReadAsync(Constant.Game, Constant.TopAutoLockHeroChampId2), out var tempTopAutoLockHeroChampId2) ? tempTopAutoLockHeroChampId2 : 0;
            JungleAutoLockHeroChampId1 = int.TryParse(
                await _settingsService.ReadAsync(Constant.Game, Constant.JungleAutoLockHeroChampId1), out var tempJungleAutoLockHeroChampId1) ? tempJungleAutoLockHeroChampId1 : 0;
            JungleAutoLockHeroChampId2 = int.TryParse(
                await _settingsService.ReadAsync(Constant.Game, Constant.JungleAutoLockHeroChampId2), out var tempJungleAutoLockHeroChampId2) ? tempJungleAutoLockHeroChampId2 : 0;
            MiddleAutoLockHeroChampId1 = int.TryParse(
                await _settingsService.ReadAsync(Constant.Game, Constant.MiddleAutoLockHeroChampId1), out var tempMiddleAutoLockHeroChampId1) ? tempMiddleAutoLockHeroChampId1 : 0;
            MiddleAutoLockHeroChampId2 = int.TryParse(
                await _settingsService.ReadAsync(Constant.Game, Constant.MiddleAutoLockHeroChampId2), out var tempMiddleAutoLockHeroChampId2) ? tempMiddleAutoLockHeroChampId2 : 0;
            BottomAutoLockHeroChampId1 = int.TryParse(
                await _settingsService.ReadAsync(Constant.Game, Constant.BottomAutoLockHeroChampId1), out var tempBottomAutoLockHeroChampId1) ? tempBottomAutoLockHeroChampId1 : 0;
            BottomAutoLockHeroChampId2 = int.TryParse(
                await _settingsService.ReadAsync(Constant.Game, Constant.BottomAutoLockHeroChampId2), out var tempBottomAutoLockHeroChampId2) ? tempBottomAutoLockHeroChampId2 : 0;
            UtilityAutoLockHeroChampId1 = int.TryParse(
                await _settingsService.ReadAsync(Constant.Game, Constant.UtilityAutoLockHeroChampId1), out var tempUtilityAutoLockHeroChampId1) ? tempUtilityAutoLockHeroChampId1 : 0;
            UtilityAutoLockHeroChampId2 = int.TryParse(
                await _settingsService.ReadAsync(Constant.Game, Constant.UtilityAutoLockHeroChampId2), out var tempUtilityAutoLockHeroChampId2) ? tempUtilityAutoLockHeroChampId2 : 0;

            AutoDisableChampId = int.TryParse(
                await _settingsService.ReadAsync(Constant.Game, Constant.AutoDisableHeroId), out var tempAutoDisableHeroId) ? tempAutoDisableHeroId : 0;
            GameExeLocation = await _settingsService.ReadAsync(Constant.Game, Constant.GameLocation);
            RankSetting = await _settingsService.ReadAsync(Constant.Game, Constant.RankSetting);
            CloseSendOtherWhenBegin = bool.TryParse(await _settingsService.ReadAsync(Constant.Game, Constant.CloseSendOtherWhenBegin), out var tempCloseSendOtherWhenBegin) ? tempCloseSendOtherWhenBegin : false;
            IsAltQOpenVsDetail = bool.TryParse(await _settingsService.ReadAsync(Constant.Game, Constant.IsAltQOpenVsDetail), out var tempIsAltQOpenVsDetail) ? tempIsAltQOpenVsDetail : true;
            HorseTemplate = await _settingsService.ReadAsync(Constant.Game, Constant.HorseTemplate);
            ChatMessageTemplate = await _settingsService.ReadAsync(Constant.Game, Constant.ChatMessageTemplate);
            Above120ScoreTxt = await _settingsService.ReadAsync(Constant.Game, Constant.Above120ScoreTxt);
            Above120ScoreTxt = string.IsNullOrEmpty(Above120ScoreTxt) ? "上等马" : Above120ScoreTxt;

            Above110ScoreTxt = await _settingsService.ReadAsync(Constant.Game, Constant.Above110ScoreTxt);
            Above110ScoreTxt = string.IsNullOrEmpty(Above110ScoreTxt) ? "中等马" : Above110ScoreTxt;

            Above100ScoreTxt = await _settingsService.ReadAsync(Constant.Game, Constant.Above100ScoreTxt);
            Above100ScoreTxt = string.IsNullOrEmpty(Above100ScoreTxt) ? "下等马" : Above100ScoreTxt;

            Below100ScoreTxt = await _settingsService.ReadAsync(Constant.Game, Constant.Below100ScoreTxt);
            Below100ScoreTxt = string.IsNullOrEmpty(Below100ScoreTxt) ? "牛马" : Below100ScoreTxt;
            AutoAcceptGameDelay = int.TryParse(
                await _settingsService.ReadAsync(Constant.Game, Constant.AutoAcceptGameDelay), out var tempAutoAcceptGameDelay) ? tempAutoAcceptGameDelay : 0;
            var readedNotices = await _settingsService.ReadAsync(Constant.Game, Constant.ReadedNotice);
            ReadedNotices = string.IsNullOrEmpty(readedNotices) ? new List<int>() : JsonSerializer.Deserialize<List<int>>(readedNotices);
            IsDarkTheme = bool.TryParse(await _settingsService.ReadAsync(Constant.Game, Constant.IsDarkTheme), out var tempIsDarkTheme) ? tempIsDarkTheme : false;

            TeamDetailKeys = await _settingsService.ReadAsync(Constant.Game, Constant.TeamDetailKey);
            TeamDetailKeys = string.IsNullOrWhiteSpace(TeamDetailKeys) ? "Alt+Q" : TeamDetailKeys;
            
            if (!File.Exists(_blackListLoc)) 
            {
                File.Create(_blackListLoc).Close();
            }
            if (!File.Exists(_fuckLoc))
            {
                File.Create(_fuckLoc).Close();
            }
            if (!File.Exists(_goodLoc))
            {
                File.Create(_goodLoc).Close();
            }
            var blackAccounts = await File.ReadAllTextAsync(_blackListLoc);
            if (string.IsNullOrEmpty(blackAccounts))
            {
                BlackAccounts = new List<BlackAccount>();
            }
            else 
            {
                BlackAccounts = JsonSerializer.Deserialize<List<BlackAccount>>(blackAccounts);
            }
            FuckWords = await File.ReadAllTextAsync(_fuckLoc);
            GoodWords = await File.ReadAllTextAsync(_goodLoc);
            FuckWordCollection = string.IsNullOrEmpty(FuckWords) ? new List<string>() : FuckWords.Split("\n").ToList();
            GoodWordCollection = string.IsNullOrEmpty(GoodWords) ? new List<string>() : GoodWords.Split("\n").ToList();

            TeamDetailKeyList = TeamDetailKeys.Split('+').ToList();
            TeamDetailHash = ComputeHash(TeamDetailKeyList);
        }

        public async Task WriteAutoAcceptAsync(bool value)
        {
            await _settingsService.WriteAsync(Constant.Game, Constant.AutoAcceptGame, value.ToString());
            AutoAcceptGame = value;
        }

        public async Task WriteAutoEndGameAsync(bool value)
        {
            await _settingsService.WriteAsync(Constant.Game, Constant.AutoEndGame, value.ToString());
            AutoEndGame = value;
        }

        public async Task WriteAutoStartGameAsync(bool value)
        {
            await _settingsService.WriteAsync(Constant.Game, Constant.AutoStartGame, value.ToString());
            AutoStartGame = value;
        }

        public async Task WriteAutoLockHeroAsync(bool value)
        {
            await _settingsService.WriteAsync(Constant.Game, Constant.AutoLockHero, value.ToString());
            AutoLockHero = value;
        }

        public async Task WriteRankAutoLockHeroAsync(bool value)
        {
            await _settingsService.WriteAsync(Constant.Game, Constant.RankAutoLockHero, value.ToString());
            RankAutoLockHero = value;
        }

        public async Task WriteAutoDisableHeroAsync(bool value)
        {
            await _settingsService.WriteAsync(Constant.Game, Constant.AutoDisableHero, value.ToString());
            AutoDisableHero = value;
        }

        public async Task WriteAutoLockHeroInAramAsync(bool value)
        {
            await _settingsService.WriteAsync(Constant.Game, Constant.AutoLockHeroInAram, value.ToString());
            AutoLockHeroInAram = value;
        }

        public async Task WriteLockHerosInAramAsync(List<int> values)
        {
            var data = JsonSerializer.Serialize(values);
            await _settingsService.WriteAsync(Constant.Game, Constant.LockHerosInAram, data);
            LockHerosInAram = values;
        }

        public async Task WriteAutoLockHeroIdAsync(int champId)
        {
            await _settingsService.WriteAsync(Constant.Game, Constant.AutoLockHeroId, champId.ToString());
            AutoLockHeroChampId = champId;
        }

        public async Task WriteTopAutoLockHeroChampId1Async(int champId)
        {
            await _settingsService.WriteAsync(Constant.Game, Constant.TopAutoLockHeroChampId1, champId.ToString());
            TopAutoLockHeroChampId1 = champId;
        }

        public async Task WriteTopAutoLockHeroChampId2Async(int champId)
        {
            await _settingsService.WriteAsync(Constant.Game, Constant.TopAutoLockHeroChampId2, champId.ToString());
            TopAutoLockHeroChampId2 = champId;
        }

        public async Task WriteJungleAutoLockHeroChampId1Async(int champId)
        {
            await _settingsService.WriteAsync(Constant.Game, Constant.JungleAutoLockHeroChampId1, champId.ToString());
            JungleAutoLockHeroChampId1 = champId;
        }

        public async Task WriteJungleAutoLockHeroChampId2Async(int champId)
        {
            await _settingsService.WriteAsync(Constant.Game, Constant.JungleAutoLockHeroChampId2, champId.ToString());
            JungleAutoLockHeroChampId2 = champId;
        }

        public async Task WriteMiddleAutoLockHeroChampId1Async(int champId)
        {
            await _settingsService.WriteAsync(Constant.Game, Constant.MiddleAutoLockHeroChampId1, champId.ToString());
            MiddleAutoLockHeroChampId1 = champId;
        }

        public async Task WriteMiddleAutoLockHeroChampId2Async(int champId)
        {
            await _settingsService.WriteAsync(Constant.Game, Constant.MiddleAutoLockHeroChampId2, champId.ToString());
            MiddleAutoLockHeroChampId2 = champId;

        }

        public async Task WriteBottomAutoLockHeroChampId1Async(int champId)
        {
            await _settingsService.WriteAsync(Constant.Game, Constant.BottomAutoLockHeroChampId1, champId.ToString());
            BottomAutoLockHeroChampId1 = champId;
        }

        public async Task WriteBottomAutoLockHeroChampId2Async(int champId)
        {
            await _settingsService.WriteAsync(Constant.Game, Constant.BottomAutoLockHeroChampId2, champId.ToString());
            BottomAutoLockHeroChampId2 = champId;
        }

        public async Task WriteUtilityAutoLockHeroChampId1Async(int champId)
        {
            await _settingsService.WriteAsync(Constant.Game, Constant.UtilityAutoLockHeroChampId1, champId.ToString());
            UtilityAutoLockHeroChampId1 = champId;
        }
        public async Task WriteUtilityAutoLockHeroChampId2Async(int champId)
        {
            await _settingsService.WriteAsync(Constant.Game, Constant.UtilityAutoLockHeroChampId2, champId.ToString());
            UtilityAutoLockHeroChampId2 = champId;
        }

        public async Task WriteAutoDisableHeroIdAsync(int champId)
        {
            await _settingsService.WriteAsync(Constant.Game, Constant.AutoDisableHeroId, champId.ToString());
            AutoDisableChampId = champId;
        }

        public async Task WriteGameLocationAsync(string location)
        {
            await _settingsService.WriteAsync(Constant.Game, Constant.GameLocation, location);
            GameExeLocation = location;
        }

        public async Task WriteRankSettingAsync(string setting)
        {
            await _settingsService.WriteAsync(Constant.Game, Constant.RankSetting, setting);
            RankSetting = setting;
        }
        public async Task WriteCloseSendOtherWhenBeginAsync(bool closeSendOtherWhenBegin)
        {
            await _settingsService.WriteAsync(Constant.Game, Constant.CloseSendOtherWhenBegin, closeSendOtherWhenBegin.ToString());
            CloseSendOtherWhenBegin = closeSendOtherWhenBegin;
        }
        public async Task WriteHorseTemplateAsync(string horseTemplate)
        {
            await _settingsService.WriteAsync(Constant.Game, Constant.HorseTemplate, horseTemplate);
            HorseTemplate = horseTemplate;
        }
        public async Task WriteChatMessageTemplateAsync(string chatMessageTemplate)
        {
            await _settingsService.WriteAsync(Constant.Game, Constant.ChatMessageTemplate, chatMessageTemplate);
            ChatMessageTemplate = chatMessageTemplate;
        }
        public async Task WriteAbove120ScoreTxtAsync(string above120ScoreTxt)
        {
            await _settingsService.WriteAsync(Constant.Game, Constant.Above120ScoreTxt, above120ScoreTxt);
            Above120ScoreTxt = above120ScoreTxt;
        }
        public async Task WriteAbove110ScoreTxtAsync(string above110ScoreTxt)
        {
            await _settingsService.WriteAsync(Constant.Game, Constant.Above110ScoreTxt, above110ScoreTxt);
            Above110ScoreTxt = above110ScoreTxt;
        }
        public async Task WriteAbove100ScoreTxtAsync(string above100ScoreTxt)
        {
            await _settingsService.WriteAsync(Constant.Game, Constant.Above100ScoreTxt, above100ScoreTxt);
            Above100ScoreTxt = above100ScoreTxt;
        }
        public async Task WriteBelow100ScoreTxtAsync(string below100ScoreTxt)
        {
            await _settingsService.WriteAsync(Constant.Game, Constant.Below100ScoreTxt, below100ScoreTxt);
            Below100ScoreTxt = below100ScoreTxt;
        }
        public async Task WriteReadedNoticesAsync(int value)
        {
            ReadedNotices.Add(value);
            var data = JsonSerializer.Serialize(ReadedNotices);
            await _settingsService.WriteAsync(Constant.Game, Constant.ReadedNotice, data);
        }
        public async Task WriteAutoAcceptGameDelay(int autoAcceptGameDelay)
        {
            await _settingsService.WriteAsync(Constant.Game, Constant.AutoAcceptGameDelay, autoAcceptGameDelay.ToString());
            AutoAcceptGameDelay = autoAcceptGameDelay;
        }
        public async Task WriteBlackAccountAsync(BlackAccount account)
        {
            BlackAccounts.Add(account);
            var data = JsonSerializer.Serialize(BlackAccounts);
            await File.WriteAllTextAsync(_blackListLoc, data);
        }

        public async Task WriteIsAltQOpenVsDetail(bool value) 
        {
            await _settingsService.WriteAsync(Constant.Game, Constant.IsAltQOpenVsDetail, value.ToString());
            IsAltQOpenVsDetail = value;
        }

        public async Task WriteIsDarkTheme(bool isDarkTheme)
        {
            await _settingsService.WriteAsync(Constant.Game, Constant.IsDarkTheme, isDarkTheme.ToString());
            IsDarkTheme = isDarkTheme;
        }

        public async Task WriteFuckWords(string fuckwords)
        {
            await File.WriteAllTextAsync(_fuckLoc, fuckwords);
            FuckWords = fuckwords;
            FuckWordCollection = string.IsNullOrEmpty(FuckWords) ? new List<string>() : FuckWords.Split("\n").ToList();
        }

        public async Task WriteGoodWords(string goodwords)
        {
            await File.WriteAllTextAsync(_goodLoc, goodwords);
            GoodWords = goodwords;
            GoodWordCollection = string.IsNullOrEmpty(GoodWords) ? new List<string>() : GoodWords.Split("\n").ToList();
        }

        public async Task WriteAutoStartWhenComputerRun(bool autoStartWhenComputerRun)
        {
            await _settingsService.WriteAsync(Constant.Game, Constant.AutoStartWhenComputerRun, autoStartWhenComputerRun.ToString());
            AutoStartWhenComputerRun = autoStartWhenComputerRun;
        }

        public async Task WriteAutoUseRune(bool autoUseRune)
        {
            await _settingsService.WriteAsync(Constant.Game, Constant.AutoUseRune, autoUseRune.ToString());
            AutoUseRune = autoUseRune;
        }

        public async Task WriteAutoUseRunePolicy(bool autoUseRuneByUseCount , bool autoUseRuneByWinRate)
        {
            await _settingsService.WriteAsync(Constant.Game, Constant.AutoUseRuneByUseCount, autoUseRuneByUseCount.ToString());
            await _settingsService.WriteAsync(Constant.Game, Constant.AutoUseRuneByWinRate, autoUseRuneByWinRate.ToString());

            AutoUseRuneByUseCount = autoUseRuneByUseCount;
            AutoUseRuneByWinRate = autoUseRuneByWinRate;
        }

        public async Task WriteTeamDetailKeys(string teamDetailKeys)
        {
            await _settingsService.WriteAsync(Constant.Game, Constant.TeamDetailKey, teamDetailKeys);
            TeamDetailHash = ComputeHash(teamDetailKeys.Split('+'));
            TeamDetailKeyList.Clear();
            TeamDetailKeyList.AddRange(teamDetailKeys.Split('+'));
            TeamDetailKeys = teamDetailKeys;
        }

        public async Task RemoveBlackAccountAsync(long id)
        {
            for (var i = BlackAccounts.Count - 1; i >= 0; i--)
            {
                if (BlackAccounts[i].Id == id)
                    BlackAccounts.RemoveAt(i);
            }

            var data = JsonSerializer.Serialize(BlackAccounts);
            await File.WriteAllTextAsync(_blackListLoc, data);
        }

        private long ComputeHash(IEnumerable<string> source)
        {
            long hash = 0;
            foreach (var item in source)
            {
                hash += item.GetHashCode();
            }

            return hash;
        }
    }
}
