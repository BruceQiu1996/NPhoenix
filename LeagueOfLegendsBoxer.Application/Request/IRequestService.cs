namespace LeagueOfLegendsBoxer.Application.Request
{
    public interface IRequestService
    {
        int Port { get; set; }
        string Token { get; set; }
        Task Initialize(int port, string token);
        Task<string> GetJsonResponseAsync(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters = null);
        Task<byte[]> GetByteArrayResponseAsync(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters = null);
        Task<string> GetJsonResponseAsync(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters, dynamic body);
        Task<TResponse> GetResponseAsync<TResponse>(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters = null);
        Task<TResponse> GetResponseAsync<TResponse>(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters, dynamic body);
        Task<string> GetStringAsync(string relativeUrl, IEnumerable<string> queryParameters);
    }
}
