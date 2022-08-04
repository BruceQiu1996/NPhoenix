using LeagueOfLegendsBoxer.Application.ApplicationControl;
using LeagueOfLegendsBoxer.Application.Settings;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LeagueOfLegendsBoxer.Resources
{
    public class IniSettingsModel
    {
        private readonly ISettingsService _settingsService;
        private readonly IApplicationService _applicationService;
        private readonly IConfiguration _configuration;

        public bool AutoAcceptGame { get; set; }
        public bool AutoLockHero { get; set; }
        public bool AutoDisableHero { get; set; }
        public bool AutoLockHeroInAram { get; set; }
        public List<int> LockHerosInAram { get; set; }
        public List<int> ReadedNotices { get; set; }
        public int AutoDisableChampId { get; set; }
        public int AutoLockHeroChampId { get; set; }
        public string GameExeLocation { get; set; }
        public string RankSetting { get; set; }
        public bool IsCloseRecommand { get; set; }
        public bool CloseSendOtherWhenBegin { get; set; }
        public string HorseTemplate { get; set; }
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
                throw new System.ArgumentNullException("can't find init file section in appsettings.json");

            await _settingsService.Initialize(file);
            AutoAcceptGame = bool.TryParse(
                await _settingsService.ReadAsync(Constant.Game, Constant.AutoAcceptGame), out var tempAutoAcceptGame) ? tempAutoAcceptGame : false;
            AutoLockHero = bool.TryParse(
                await _settingsService.ReadAsync(Constant.Game, Constant.AutoLockHero), out var tempAutoLockHero) ? tempAutoLockHero : false;
            AutoDisableHero = bool.TryParse(
                await _settingsService.ReadAsync(Constant.Game, Constant.AutoDisableHero), out var tempAutoDisableHero) ? tempAutoDisableHero : false;
            AutoLockHeroInAram = bool.TryParse(
                await _settingsService.ReadAsync(Constant.Game, Constant.AutoLockHeroInAram), out var tempAutoLockHeroInAram) ? tempAutoLockHeroInAram : false;
            var lockHerosInAramData = await _settingsService.ReadAsync(Constant.Game, Constant.LockHerosInAram);
            LockHerosInAram = string.IsNullOrEmpty(lockHerosInAramData) ? new List<int>() : JsonSerializer.Deserialize<List<int>>(lockHerosInAramData);
            AutoLockHeroChampId = int.TryParse(
                await _settingsService.ReadAsync(Constant.Game, Constant.AutoLockHeroId), out var tempAutoLockHeroId) ? tempAutoLockHeroId : 0;
            AutoDisableChampId = int.TryParse(
                await _settingsService.ReadAsync(Constant.Game, Constant.AutoDisableHeroId), out var tempAutoDisableHeroId) ? tempAutoDisableHeroId : 0;
            GameExeLocation = await _settingsService.ReadAsync(Constant.Game, Constant.GameLocation);
            RankSetting = await _settingsService.ReadAsync(Constant.Game, Constant.RankSetting);
            IsCloseRecommand = bool.TryParse(await _settingsService.ReadAsync(Constant.Game, Constant.IsCloseRecommand), out var tempRecommand) ? tempRecommand : false;
            CloseSendOtherWhenBegin = bool.TryParse(await _settingsService.ReadAsync(Constant.Game, Constant.CloseSendOtherWhenBegin), out var tempCloseSendOtherWhenBegin) ? tempCloseSendOtherWhenBegin : false;
            HorseTemplate = await _settingsService.ReadAsync(Constant.Game, Constant.HorseTemplate);
            var readedNotices = await _settingsService.ReadAsync(Constant.Game, Constant.ReadedNotice);
            ReadedNotices = string.IsNullOrEmpty(readedNotices) ? new List<int>() : JsonSerializer.Deserialize<List<int>>(readedNotices);
        }

        public async Task WriteAutoAcceptAsync(bool value)
        {
            await _settingsService.WriteAsync(Constant.Game, Constant.AutoAcceptGame, value.ToString());
            AutoAcceptGame = value;
        }

        public async Task WriteAutoLockHeroAsync(bool value)
        {
            await _settingsService.WriteAsync(Constant.Game, Constant.AutoLockHero, value.ToString());
            AutoLockHero = value;
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

        public async Task WriteIsCloseRecommandAsync(bool isCloseRecommand)
        {
            await _settingsService.WriteAsync(Constant.Game, Constant.IsCloseRecommand, isCloseRecommand.ToString());
            IsCloseRecommand = isCloseRecommand;
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
        public async Task WriteReadedNoticesAsync(int value)
        {
            ReadedNotices.Add(value);
            var data = JsonSerializer.Serialize(ReadedNotices);
            await _settingsService.WriteAsync(Constant.Game, Constant.ReadedNotice, data);
        }
    }
}
