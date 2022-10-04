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
    }
}
