using LeagueOfLegendsBoxer.Application.ApplicationControl;
using LeagueOfLegendsBoxer.Application.Settings;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace NPhoenixSPA.Resources
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
        public List<long> SubscribeFriendsList { get; set; }
        public int AutoDisableChampId { get; set; }
        public int AutoLockHeroChampId { get; set; }
        public string GameExeLocation { get; set; }
        public string RankSetting { get; set; }
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
        }

        #region write
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

        public async Task WriteSubscribeFriendsListAsync(List<long> friends)
        {
            var data = JsonSerializer.Serialize(friends);
            await _settingsService.WriteAsync(Constant.Account.SummonerId.ToString(), Constant.SubscribeFriendsList, data);
            SubscribeFriendsList = friends;
        }

        #endregion
        public async Task RefreshSubscribeFriendsList() 
        {
            var lockHerosInAramData = await _settingsService.ReadAsync(Constant.Account.SummonerId.ToString(), Constant.SubscribeFriendsList);
            SubscribeFriendsList = string.IsNullOrEmpty(lockHerosInAramData) ? new List<long>() : JsonSerializer.Deserialize<List<long>>(lockHerosInAramData);
        }
    }
}
