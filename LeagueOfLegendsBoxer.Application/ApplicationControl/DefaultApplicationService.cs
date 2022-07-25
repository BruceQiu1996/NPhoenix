using LeagueOfLegendsBoxer.Application.Request;

namespace LeagueOfLegendsBoxer.Application.ApplicationControl
{
    public class DefaultApplicationService : IApplicationService
    {
        private readonly string _baseUrl = "process-control/v1/process/";
        private readonly string _installUrl = "data-store/v1/install-dir";
        private readonly string _setRankUrl = "lol-chat/v1/me";
        private readonly IRequestService _requestService;
        public DefaultApplicationService(IRequestService requestService)
        {
            _requestService = requestService;
        }

        public async Task<string> GetInstallLocation()
        {
            return await _requestService.GetJsonResponseAsync(HttpMethod.Get, _installUrl);
        }

        public async Task QuitAsync()
        {
            await _requestService.GetJsonResponseAsync(HttpMethod.Post, $"{_baseUrl}quit");
        }

        public async Task RestartAsync(int delaySeconds)
        {
            var queryParameters = new List<string>();
            await RestartAsync(delaySeconds, queryParameters);
        }

        public async Task RestartAsync(int delaySeconds, int restartVersion)
        {
            var queryParameters = new List<string> { $"restartVersion={restartVersion}" };
            await RestartAsync(delaySeconds, queryParameters);
        }

        public async Task RestartToRepair()
        {
            await _requestService.GetJsonResponseAsync(HttpMethod.Post, $"{_baseUrl}restart-to-repair");
        }

        public async Task RestartToUpdate(int delaySeconds, string selfUpdateUrl)
        {
            var queryParameters = new string[]
            {
                $"delaySeconds={delaySeconds}",
                $"selfUpdateUrl={selfUpdateUrl}"
            };
            await _requestService.GetJsonResponseAsync(HttpMethod.Post, $"{_baseUrl}restart-to-update", queryParameters);
        }

        public async Task<string> SetRankAsync(dynamic body)
        {
            return await _requestService.GetJsonResponseAsync(HttpMethod.Put, _setRankUrl,null, body);
        }

        private async Task RestartAsync(int delaySeconds, ICollection<string> queryParameters)
        {
            queryParameters.Add($"delaySeconds={delaySeconds}");
            await _requestService.GetJsonResponseAsync(HttpMethod.Post, $"{_baseUrl}restart", queryParameters);
        }
    }
}
