using LeagueOfLegendsBoxer.Application.Teamup.Dtos;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text.Json;

namespace LeagueOfLegendsBoxer.Application.Teamup
{
    public class TeamupService : ITeamupService
    {
        private readonly string _user = "/Users";
        private readonly string _record = "/Records";

        private HttpClient _httpClient;

        public TeamupService()
        {
            
        }

        public Task Initialize(string baseAddress)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(baseAddress);
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return Task.CompletedTask;
        }


        public async Task<UserCreateOrUpdateByClientResponseDto> LoginAsync(UserCreateOrUpdateByClientDto dto)
        {
            StringContent content = new StringContent(JsonConvert.SerializeObject(dto));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var resp = await _httpClient.PostAsync(_user, content);
            if (resp.StatusCode == System.Net.HttpStatusCode.OK) 
            {
                var result = await resp.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<UserCreateOrUpdateByClientResponseDto>(result);
            }

            return null;
        }

        public void SetToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        public async Task<bool> UploadRecordAsync(CreateGameRecordByClientDto dto)
        {
            StringContent content = new StringContent(JsonConvert.SerializeObject(dto));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var resp = await _httpClient.PostAsync(_record, content);
            return resp.IsSuccessStatusCode;
        }
    }
}
