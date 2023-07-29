using LeagueOfLegendsBoxer.Application.Teamup.Dtos;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace LeagueOfLegendsBoxer.Application.Teamup
{
    public class TeamupService : ITeamupService
    {
        private readonly string _user = "/Users";
        private readonly string _record = "/Records";
        private readonly string _posts = "/Posts";
        private readonly string _topPosts = "/Posts/top";
        private readonly string _uploadImage = "/Posts/upload-image";
        private readonly string _good = "/Posts/good/{0}";
        private readonly string _postDetail = "/Posts/detail/{0}";
        private readonly string _postComment = "/Posts/comment";
        private readonly string _getPostComment = "/Posts/comments/{0}/{1}";
        private readonly string _denyChat = "/Manage/mushin";

        private HttpClient _httpClient;

        public TeamupService()
        {

        }

        public async Task<string> GetRankDataAsync()
        {
            try
            {
                return await _httpClient.GetStringAsync(_record);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Task Initialize(string baseAddress)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(baseAddress);
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
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

        public async Task<bool> UpdateServerAreaAsync(UserServerAreaUpdateDto dto)
        {
            StringContent content = new StringContent(JsonConvert.SerializeObject(dto));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var resp = await _httpClient.PutAsync(_user, content);

            return resp.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<PostResponseDto>> GetTopPostsAsync()
        {
            var resp = await _httpClient.GetAsync(_topPosts);
            if (resp.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<IEnumerable<PostResponseDto>>(await resp.Content.ReadAsStringAsync());
            }

            return null;
        }

        public async Task<(int, IEnumerable<PostResponseDto>)> GetPostsAsync(string key, PostCategory? postCategory, int page, int pageSize = 10)
        {
            var sb = new StringBuilder(_posts);

            sb.Append($"?keyWord={key}&");
            sb.Append($"postCategory={postCategory}&");
            sb.Append($"page={page}&");
            sb.Append($"pageSize={pageSize}");

            var resp = await _httpClient.GetAsync(sb.ToString());
            if (resp.IsSuccessStatusCode)
            {
                var obj = JsonConvert.DeserializeObject<PostResponsePageDto>(await resp.Content.ReadAsStringAsync());
                return (obj.Count, obj.Data);
            }

            return default;
        }

        public async Task<bool> CreateOrUpdatePostAsync(PostCreateOrUpdateDto dto)
        {
            StringContent content = new StringContent(JsonConvert.SerializeObject(dto));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var resp = await _httpClient.PostAsync(_posts, content);

            return resp.IsSuccessStatusCode;
        }

        public async Task<string> UploadImageAsync(UploadPostImageDto dto)
        {
            StringContent content = new StringContent(JsonConvert.SerializeObject(dto));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var resp = await _httpClient.PostAsync(_uploadImage, content);
            if (resp.IsSuccessStatusCode)
            {
                var obj = JsonConvert.DeserializeObject<dynamic>(await resp.Content.ReadAsStringAsync());
                return $"{obj.fileLoc}";
            }

            return null;
        }

        public async Task<(bool, int)> GoodAsync(long postId)
        {
            var resp = await _httpClient.GetAsync(string.Format(_good, postId));
            if (resp.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<dynamic>(await resp.Content.ReadAsStringAsync());

                return (result.isGood, result.count);
            }

            throw new Exception("faild to good or nogood");
        }

        public async Task<PostDetailResponseDto> GetPostDetailAsync(long postId)
        {
            var resp = await _httpClient.GetAsync(string.Format(_postDetail, postId));
            if (resp.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<PostDetailResponseDto>(await resp.Content.ReadAsStringAsync());

                return result;
            }

            throw new Exception();
        }

        public async Task<bool> CreatePostCommentAsync(CreatePostCommentDto dto)
        {
            StringContent content = new StringContent(JsonConvert.SerializeObject(dto));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var resp = await _httpClient.PostAsync(_postComment, content);
            if (resp.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }

        public async Task<PostCommentsResponsePageDto> GetPostCommentsByPage(long postId, int page)
        {
            var resp = await _httpClient.GetAsync(string.Format(_getPostComment, postId,page));
            if (resp.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<PostCommentsResponsePageDto>(await resp.Content.ReadAsStringAsync());

                return result;
            }

            throw new Exception();
        }

        public async Task<bool> DenyChatAsync(long userId)
        {
            StringContent content = new StringContent(JsonConvert.SerializeObject(new 
            {
                UserId = userId,
                Type = 2
            }));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var resp = await _httpClient.PutAsync(_denyChat, content);
            if (resp.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }
    }
}
