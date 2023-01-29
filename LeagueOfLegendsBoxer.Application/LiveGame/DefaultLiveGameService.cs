using System.Net.Http.Headers;

namespace LeagueOfLegendsBoxer.Application.LiveGame
{
    public class DefaultLiveGameService : ILiveGameService
    {
        private HttpClient _httpClient;
        private const string _gameEvent = "liveclientdata/eventdata?eventID=0";
        private const string _teamData = "liveclientdata/playerlist?teamID={0}";
        private const string _userItems = "liveclientdata/playeritems?summonerName={0}";
        private const string _userSpells = "liveclientdata/playersummonerspells?summonerName={0}";
        public DefaultLiveGameService()
        {
            var _httpClientHandler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual
            };
            
            _httpClientHandler.ServerCertificateCustomValidationCallback = (response, cert, chain, errors) => true;
            _httpClient = new HttpClient(_httpClientHandler);
            _httpClient.BaseAddress = new Uri("https://127.0.0.1:2999");
            _httpClient.DefaultRequestVersion = new Version(2, 0);
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "LeagueOfLegendsClient/12.7.433.4138 (CEF 91)");
            _httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        public async Task<string> GetGameEventAsync()
        {
            try
            {
                return await _httpClient.GetStringAsync(_gameEvent);
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> GetPlayersAsync(int teamId)
        {
            try
            {
                return await _httpClient.GetStringAsync(string.Format(_teamData, teamId));
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> GetDataByNameAsync(string summonerName)
        {
            try
            {
                return await _httpClient.GetStringAsync(string.Format(_userItems, summonerName));
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> GetSpellByNameAsync(string summonerName)
        {
            try
            {
                return await _httpClient.GetStringAsync(string.Format(_userSpells, summonerName));
            }
            catch
            {
                return null;
            }
        }
    }
}
