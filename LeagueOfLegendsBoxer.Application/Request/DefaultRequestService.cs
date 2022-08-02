using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace LeagueOfLegendsBoxer.Application.Request
{
    public class DefaultRequestService : AbstractRequestService, IRequestService
    {
        public int Port { get; set; }
        public string Token { get; set; }

        public Task Initialize(int port, string token)
        {
            Port = port;
            Token = token;
            CreateHttpClient();
            var authTokenBytes = Encoding.ASCII.GetBytes($"riot:{token}");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authTokenBytes));
            _httpClient.BaseAddress = new Uri($"https://127.0.0.1:{port}/");

            return Task.CompletedTask;
        }

        public async Task<string> GetJsonResponseAsync(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters = null)
        {
            return await GetJsonResponseAsync(httpMethod, relativeUrl, queryParameters, null);
        }

        public async Task<string> GetJsonResponseAsync(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters, dynamic body)
        {
            try
            {
                var request = PrepareRequest(httpMethod, relativeUrl, queryParameters, body);
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return await GetResponseContentAsync(response);
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public async Task<TResponse> GetResponseAsync<TResponse>(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters = null)
        {
            return await GetResponseAsync<TResponse>(httpMethod, relativeUrl, queryParameters, null);
        }

        public async Task<TResponse> GetResponseAsync<TResponse>(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters, dynamic body)
        {
            var json = await GetJsonResponseAsync(httpMethod, relativeUrl, queryParameters, body);
            return JsonSerializer.Deserialize<TResponse>(json);
        }

        public async Task<byte[]> GetByteArrayResponseAsync(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters = null)
        {
            try
            {
                var request = PrepareRequest(httpMethod, relativeUrl, queryParameters, null);
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return await GetResponseContentByByteArrayAsync(response);
            }
            catch
            {
                return null;
            }
        }
    }
}
