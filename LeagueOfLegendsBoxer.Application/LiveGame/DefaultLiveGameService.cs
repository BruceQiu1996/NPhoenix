using System.Net.Http.Headers;

namespace LeagueOfLegendsBoxer.Application.LiveGame
{
    public class DefaultLiveGameService : ILiveGameService
    {
        private HttpClient _httpClient;
        private const string _gameEvent = "https://127.0.0.1:2999/liveclientdata/eventdata?eventID=0";

        public DefaultLiveGameService()
        {
            var _httpClientHandler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual
            };
            _httpClientHandler.ServerCertificateCustomValidationCallback = (response, cert, chain, errors) => true;
            _httpClient = new HttpClient(_httpClientHandler);
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
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
    }
}
