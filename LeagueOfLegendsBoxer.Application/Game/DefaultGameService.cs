using LeagueOfLegendsBoxer.Application.Request;

namespace LeagueOfLegendsBoxer.Application.Game
{
    public class DefaultGameService : IGameService
    {
        private const string _checkUrl = "lol-matchmaking/v1/ready-check/";
        private const string _gameSessionUrl = "lol-champ-select/v1/session";
        private const string _gameActionUrl = "lol-champ-select/v1/session/actions/{0}";
        private const string _conversationChat = "lol-chat/v1/conversations";
        private const string _conversationChatMessages = "/lol-chat/v1/conversations/{0}/messages";
        private const string _sendGameChat = "lol-simple-dialog-messages/v1/messages";
        private const string _SendMessage = "lol-chat/v1/conversations/{0}/messages";
        private const string _gameDetails = "lol-match-history/v1/games/{0}";
        private const string _summonerSuperChamp = "lol-collections/v1/inventories/{0}/champion-mastery";
        private const string _champDataUrl = "https://x1-6833.native.qq.com/x1/6833/1061021&3af49f";
        private const string _test = "/data-store/v1/install-settings/gameflow-patcher-lock";
        private const string _gameSessionData = "lol-gameflow/v1/session";
        private const string _currentChampion = "/lol-champ-select/v1/current-champion";
        private const string _pickableChampion = "/lol-champ-select/v1/pickable-champions";
        private const string _benchSwapChampion = " /lol-champ-select/v1/session/bench/swap/{0}";
        private const string _champRestraintData = "https://lol.qq.com/act/lbp/common/guides/champDetail/champDetail_{0}.js?ts=2760378";
        private const string _rune = "lol-perks/v1/pages";
        private const string _currentRune = "lol-perks/v1/currentpage";
        private const string _getskins = "lol-game-data/assets/v1/champions/{0}.json";
        private const string _getIcons = "lol-game-data/assets/v1/profile-icons.json";
        private const string _getSpells = "lol-game-data/assets/v1/summoner-spells.json";
        private const string _getItems = "lol-game-data/assets/v1/items.json";
        private const string _setSkinBackground = "lol-summoner/v1/current-summoner/summoner-profile";
        private const string _setIcon = "lol-summoner/v1/current-summoner/icon";
        private const string _getRecordsByPage = "lol-match-history/v1/products/lol/{0}/matches";
        private const string _getRuneItemsOnline = "https://www.wegame.com.cn/lol/resources/js/champion/recommend/{0}.js ";
        private const string _championColorSkins = "lol-champ-select/v1/skin-carousel-skins";
        private const string _useColorSkin = "/lol-champ-select/v1/session/my-selection";
        private readonly IRequestService _requestService;

        public DefaultGameService(IRequestService requestService)
        {
            _requestService = requestService;
        }

        public async Task AddRunePage(dynamic body)
        {
            var result = await _requestService.GetJsonResponseAsync(HttpMethod.Post, _rune, null, body);
        }

        public async Task<string> GetCurrentRunePage()
        {
            return await _requestService.GetStringAsync(_currentRune, null);
        }

        public async Task AutoAcceptGameAsync()
        {
            await _requestService.PostAsync($"{_checkUrl}accept", null, null);
        }

        public Task AutoDisableHeroAsync()
        {
            throw new NotImplementedException();
        }

        public async Task AutoLockHeroAsync(int actionID, int champID, string type)
        {
            var body = new
            {
                completed = true,
                type = type,
                championId = champID
            };

            await _requestService.PatchAsync(string.Format(_gameActionUrl, actionID), null, body);
        }

        public async Task BenchSwapChampionsAsync(int champID)
        {
            await _requestService.GetJsonResponseAsync(HttpMethod.Post, string.Format(_benchSwapChampion, champID));
        }

        public async Task DeleteRunePage(long id)
        {
            await _requestService.GetJsonResponseAsync(HttpMethod.Delete, $"{_rune}/{id}");
        }

        public async Task<string> GetAllRunePages()
        {
            return await _requestService.GetJsonResponseAsync(HttpMethod.Get, _rune);
        }

        public async Task<string> GetChampRankAsync(string lane, int tier, int time)
        {
            return await _requestService.GetJsonResponseAsync(HttpMethod.Get, _champDataUrl, new List<string>()
            {
                "championid=666",
                $"lane={lane}",
                $"dtstatdate={time}",
                $"tier={tier}",
                "ijob=all",
                "gamequeueconfigid=420"
            });
        }

        public async Task<string> GetChampRestraintDataAsync(int champID)
        {
            return await _requestService.GetJsonResponseAsync(HttpMethod.Get, string.Format(_champRestraintData, champID));
        }

        public async Task<string> GetChatConversation()
        {
            return await _requestService.GetJsonResponseAsync(HttpMethod.Get, _conversationChat);
        }

        public async Task<string> GetChatConversationMessages(string chatId)
        {
            return await _requestService.GetJsonResponseAsync(HttpMethod.Get, string.Format(_conversationChatMessages, chatId));
        }

        public async Task<string> GetCurrentChampionInfoAsync()
        {
            return await _requestService.GetJsonResponseAsync(HttpMethod.Get, _currentChampion);
        }

        public async Task<string> GetCurrentChampionColorSkinsAsync()
        {
            return await _requestService.GetJsonResponseAsync(HttpMethod.Get, _championColorSkins);
        }

        public async Task<string> GetCurrentGameInfoAsync()
        {
            return await _requestService.GetJsonResponseAsync(HttpMethod.Get, _gameSessionData);
        }

        public async Task<string> GetGameSessionAsync()
        {
            return await _requestService.GetStringAsync(_gameSessionUrl, null);
        }

        public async Task<string> GetIcons()
        {
            return await _requestService.GetJsonResponseAsync(HttpMethod.Get, _getIcons);
        }

        public async Task<string> GetSpells()
        {
            return await _requestService.GetStringAsync(_getSpells, null);
        }

        public async Task<string> GetItems()
        {
            return await _requestService.GetStringAsync(_getItems, null);
        }

        public async Task<string> GetPickableChampionsAsync()
        {
            return await _requestService.GetJsonResponseAsync(HttpMethod.Get, _pickableChampion);
        }

        public async Task<string> GetRecordsByPage(int pageStart = 0, int pageEnd = 20, string id = null)
        {
            return await _requestService.GetStringAsync(string.Format(_getRecordsByPage, id), new List<string>()
            {
                $"begIndex={pageStart}",
                $"endIndex={pageEnd}",
            });
        }

        public async Task<byte[]> GetResourceByUrl(string url)
        {
            return await _requestService.GetByteArrayResponseAsync(HttpMethod.Get, url);
        }

        public async Task<string> GetSkinsByHeroId(int id)
        {
            return await _requestService.GetJsonResponseAsync(HttpMethod.Get, string.Format(_getskins, id));
        }

        public async Task<string> QueryGameDetailAsync(long gameId)
        {
            return await _requestService.GetStringAsync(string.Format(_gameDetails, gameId), null);
        }

        public async Task<string> QuerySummonerSuperChampDataAsync(long summonerId)
        {
            return await _requestService.GetJsonResponseAsync(HttpMethod.Get, string.Format(_summonerSuperChamp, summonerId));
        }

        public async Task<string> SendGameChatMessageAsync(string message)
        {
            return await _requestService.GetJsonResponseAsync(HttpMethod.Get, "lol-game-client-chat/v1/buddies");
        }

        public async Task SendMessageAsync(string chatId, string message)
        {
            var body = new
            {
                body = message,
                type = "chat"
            };

            await _requestService.GetJsonResponseAsync(HttpMethod.Post, string.Format(_SendMessage, chatId), null, body);
        }

        public async Task<string> SetIconAsync(dynamic body)
        {
            return await _requestService.GetJsonResponseAsync(HttpMethod.Put, _setIcon, null, body);
        }

        public async Task<string> SetSkinAsync(dynamic body)
        {
            return await _requestService.GetJsonResponseAsync(HttpMethod.Post, _setSkinBackground, null, body);
        }

        public async Task<string> test(long id)
        {
            return await _requestService.GetJsonResponseAsync(HttpMethod.Get, string.Format(_test, id));
        }

        public async Task<string> GetRuneItemsFromOnlineAsync(int champId)
        {
            return await _requestService.GetJsonResponseAsync(HttpMethod.Get, string.Format(_getRuneItemsOnline, champId));
        }

        public async Task UseColorSkinAsync(dynamic body)
        {
            await _requestService.PatchAsync(_useColorSkin, null, body);
        }
    }
}
