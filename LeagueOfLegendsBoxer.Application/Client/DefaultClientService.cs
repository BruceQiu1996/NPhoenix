using LeagueOfLegendsBoxer.Application.Request;

namespace LeagueOfLegendsBoxer.Application.Client
{
    public class DefaultClientService : IClientService
    {
        private const string BaseUrl = "riotclient/";

        private readonly IRequestService _requestService;
        public DefaultClientService(IRequestService requestService) 
        {
            _requestService = requestService;
        }

        /// <inheritdoc />
        public async Task MinimizeUxAsync()
        {
            await _requestService.GetJsonResponseAsync(HttpMethod.Post, $"{BaseUrl}ux-minimize");
        }

        /// <inheritdoc />
        public async Task ShowUxAsync()
        {
            await _requestService.GetJsonResponseAsync(HttpMethod.Post, $"{BaseUrl}ux-show");
        }

        /// <inheritdoc />
        public async Task FlashUxAsync()
        {
            await _requestService.GetJsonResponseAsync(HttpMethod.Post, $"{BaseUrl}ux-flash");
        }

        /// <inheritdoc />
        public async Task KillUxAsync()
        {
            await _requestService.GetJsonResponseAsync(HttpMethod.Post, $"{BaseUrl}kill-ux");
        }

        /// <inheritdoc />
        public async Task KillAndRestartUxAsync()
        {
            await _requestService.GetJsonResponseAsync(HttpMethod.Post, $"{BaseUrl}kill-and-restart-ux");
        }

        /// <inheritdoc />
        public async Task UnloadUxAsync()
        {
            await _requestService.GetJsonResponseAsync(HttpMethod.Post, $"{BaseUrl}unload");
        }

        /// <inheritdoc />
        public async Task LaunchUxAsync()
        {
            await _requestService.GetJsonResponseAsync(HttpMethod.Post, $"{BaseUrl}launch-ux");
        }

        /// <inheritdoc />
        public async Task<double> GetZoomScaleAsync()
        {
            return double.Parse(await _requestService.GetJsonResponseAsync(HttpMethod.Get, $"{BaseUrl}zoom-scale"));
        }

        /// <inheritdoc />
        public async Task SetZoomScaleAsync(double scale)
        {
            var queryParameters = new string[] { $"newZoomScale={scale}" };
            await _requestService.GetJsonResponseAsync(HttpMethod.Post, $"{BaseUrl}zoom-scale", queryParameters);
        }
    }
}
