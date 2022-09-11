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
            _httpClientHandler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
            };
            _httpClientHandler.ServerCertificateCustomValidationCallback = (response, cert, chain, errors) => true;

            var authTokenBytes = Encoding.ASCII.GetBytes($"riot:{token}");
            _httpClient = new HttpClient(_httpClientHandler);
            _httpClient.BaseAddress = new Uri($"https://127.0.0.1:{port}/");
            _httpClient.DefaultRequestVersion = new Version(2, 0);
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "LeagueOfLegendsClient/12.7.433.4138 (CEF 91)");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authTokenBytes));

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
            catch
            {
                return null;
            }
        }

        public async Task<string> GetStringAsync(string relativeUrl, IEnumerable<string> queryParameters)
        {
            try
            {
                var url = queryParameters == null ? relativeUrl : relativeUrl + BuildQueryParameterString(queryParameters);
                return await _httpClient.GetStringAsync(url);
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> PatchAsync(string relativeUrl, IEnumerable<string> queryParameters, dynamic body)
        {
            try
            {
                var url = queryParameters == null ? relativeUrl : relativeUrl + BuildQueryParameterString(queryParameters);
                StringContent stringContent = null;
                if (body != null)
                {
                    var json = JsonSerializer.Serialize(body);
                    stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                }

                var resp =  await _httpClient.PatchAsync(url, stringContent);
                resp.EnsureSuccessStatusCode();
                return await resp.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
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

        public async Task<string> PostAsync(string relativeUrl, IEnumerable<string> queryParameters, dynamic body)
        {
            try
            {
                var url = queryParameters == null ? relativeUrl : relativeUrl + BuildQueryParameterString(queryParameters);
                StringContent stringContent = null;
                if (body != null)
                {
                    var json = JsonSerializer.Serialize(body);
                    stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                }

                var resp = await _httpClient.PostAsync(url, stringContent);
                resp.EnsureSuccessStatusCode();
                return await resp.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
