using LeagueOfLegendsBoxer.Application.Request;

namespace LeagueOfLegendsBoxer.Application.Account
{
    public class DefaultAccountService : IAccountService
    {
        private const string _accountProfile = "lol-summoner/v1/current-summoner";
        private const string _rank = "lol-ranked/v1/current-ranked-stats";
        private const string _hero = "/lol-collections/v1/inventories/{0}/champion-mastery";
        private const string _record = "/lol-match-history/v3/matchlist/account/{0}";
        private const string _summonerProfile = "lol-summoner/v1/summoners/{0}";
        private const string _summonerRank = "/lol-ranked/v1/ranked-stats/{0}";
        private const string _matchList = "/lol-acs/v2/matchlists";

        private readonly IRequestService _requestService;
        public DefaultAccountService(IRequestService requestService)
        {
            _requestService = requestService;
        }

        public async Task<string> GetRecordInformationAsync(long summonerId)
        {
            return await _requestService.GetJsonResponseAsync(HttpMethod.Get, string.Format(_record, summonerId));
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
            return await _requestService.GetJsonResponseAsync(HttpMethod.Get, _rank);
        }

        public async Task<string> GetSummonerRankInformationAsync(string puuid)
        {
            return await _requestService.GetJsonResponseAsync(HttpMethod.Get, string.Format(_summonerRank, puuid));
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
    }
}
