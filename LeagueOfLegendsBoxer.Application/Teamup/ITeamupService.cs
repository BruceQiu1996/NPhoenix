using LeagueOfLegendsBoxer.Application.Teamup.Dtos;

namespace LeagueOfLegendsBoxer.Application.Teamup
{
    public interface ITeamupService
    {
        Task Initialize(string baseAddress);
        void SetToken(string token);
        Task<UserCreateOrUpdateByClientResponseDto> LoginAsync(UserCreateOrUpdateByClientDto dto);
        Task<bool> UploadRecordAsync(CreateGameRecordByClientDto dto);
        Task<bool> UpdateServerAreaAsync(UserServerAreaUpdateDto dto);
        Task<string> GetRankDataAsync();
        Task<IEnumerable<PostResponseDto>> GetTopPostsAsync();
        Task<(int, IEnumerable<PostResponseDto>)> GetPostsAsync(string key, PostCategory? postCategory, int page, int pageSize = 10);
        Task<bool> CreateOrUpdatePostAsync(PostCreateOrUpdateDto dto);
        Task<string> UploadImageAsync(UploadPostImageDto dto);
        Task<(bool, int)> GoodAsync(long postId);
        Task<PostDetailResponseDto> GetPostDetailAsync(long postId);
        Task<bool> CreatePostCommentAsync(CreatePostCommentDto dto);
        Task<PostCommentsResponsePageDto> GetPostCommentsByPage(long postId, int page);
    }
}
