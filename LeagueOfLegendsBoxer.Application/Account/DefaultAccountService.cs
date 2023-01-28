using LeagueOfLegendsBoxer.Application.Request;

namespace LeagueOfLegendsBoxer.Application.Account
{
    public class DefaultAccountService : IAccountService
    {
        private const string _loginInfo = "/lol-login/v1/session";
        private const string _gameData = "/liveclientdata/allgamedata";
        private const string _friendsList = "/lol-chat/v1/friends";
        private const string _accountProfile = "lol-summoner/v1/current-summoner";
        private const string _rank = "lol-ranked/v1/current-ranked-stats";
        private const string _hero = "/lol-collections/v1/inventories/{0}/champion-mastery";
        private const string _record = "/lol-match-history/v3/matchlist/account/{0}";
        private const string _summonerProfile = "lol-summoner/v1/summoners/{0}";
        private const string _summonerProfileByName = "lol-summoner/v1/summoners?name={0}";
        private const string _summonerRank = "/lol-ranked/v1/ranked-stats/{0}";
        private const string _matchList = "/lol-acs/v2/matchlists";

        private readonly IRequestService _requestService;
        public DefaultAccountService(IRequestService requestService)
        {
            _requestService = requestService;
        }

        public async Task<string> GetRecordInformationAsync(long summonerId)
        {
            return await _requestService.GetStringAsync(string.Format(_record, summonerId),null);
        }

        public async Task<string> GetSummonerInformationAsync(long summonerId)
        {
            return await _requestService.GetJsonResponseAsync(HttpMethod.Get, string.Format(_summonerProfile, summonerId));
        }

        public async Task<string> GetUserAccountInformationAsync()
        {
            return await _requestService.GetJsonResponseAsync(HttpMethod.Get, _accountProfile);
        }

        public async Task<string> GetUserHeroInformationAsync(long summonerId)
        {
            return await _requestService.GetJsonResponseAsync(HttpMethod.Get, string.Format(_hero, summonerId));
        }

        public async Task<string> GetUserRankInformationAsync()
        {
            return await _requestService.GetStringAsync(_rank,null);
        }

        public async Task<string> GetSummonerRankInformationAsync(string puuid)
        {
            var data =  await _requestService.GetJsonResponseAsync(HttpMethod.Get, string.Format(_summonerRank, puuid));
            return data;
        }

        public async Task<string> GetRecordInformationAsync1(long summonerId)
        {

            return await _requestService.GetJsonResponseAsync(HttpMethod.Get,_matchList,new List<string>()
            {
                $"accountId={summonerId}",
                "begIndex = 0",
                "begIndex = 10"
            });
        }

        public async Task<string> GetLoginInfoAsync()
        {
            return await _requestService.GetJsonResponseAsync(HttpMethod.Get, _loginInfo);
        }

        public async Task<string> GetLoginTimeAsync()
        {
            return await _requestService.GetJsonResponseAsync(HttpMethod.Get, _gameData);
        }

        public async Task<string> GetFriendsAsync()
        {
            return await _requestService.GetJsonResponseAsync(HttpMethod.Get, _friendsList);
        }

        public async Task<string> GetSummonerInformationAsync(string summonerName)
        {
            return await _requestService.GetJsonResponseAsync(HttpMethod.Get, string.Format(_summonerProfileByName,summonerName));
        }
    }
}
