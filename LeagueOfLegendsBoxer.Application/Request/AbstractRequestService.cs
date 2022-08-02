using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace LeagueOfLegendsBoxer.Application.Request
{
    public abstract class AbstractRequestService
    {
        private readonly HttpClientHandler _httpClientHandler;
        protected HttpClient _httpClient;

        public AbstractRequestService()
        {
            _httpClientHandler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual
            };
            _httpClientHandler.ServerCertificateCustomValidationCallback = (response, cert, chain, errors) => true;
            CreateHttpClient();
        }

        protected void CreateHttpClient()
        {
            _httpClient = new HttpClient(_httpClientHandler);
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        protected HttpRequestMessage PrepareRequest(HttpMethod httpMethod, 
                                                    string relativeUrl,
                                                    IEnumerable<string> queryParameters, 
                                                    dynamic body)
        {
            var url = queryParameters == null ? relativeUrl : relativeUrl + BuildQueryParameterString(queryParameters);
            var request = new HttpRequestMessage(httpMethod, url);

            if (body != null)
            {
                var json = JsonSerializer.Serialize(body);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            return request;
        }

        protected string BuildQueryParameterString(IEnumerable<string> queryParameters)
        {
            return "?" + string.Join("&", queryParameters.Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        protected async Task<string> GetResponseContentAsync(HttpResponseMessage response)
        {
            return await response.Content.ReadAsStringAsync();
        }
        protected async Task<byte[]> GetResponseContentByByteArrayAsync(HttpResponseMessage response)
        {
            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}
