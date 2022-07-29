namespace LeagueOfLegendsBoxer.Application.Account
{
    public interface IAccountService
    {
        Task<string> GetFriendsAsync();
        Task<string> GetLoginInfoAsync();
        Task<string> GetLoginTimeAsync();
        Task<string> GetUserAccountInformationAsync();
        Task<string> GetUserRankInformationAsync();
        Task<string> GetUserHeroInformationAsync(long summonerId);
        Task<string> GetRecordInformationAsync(long summonerId);
        Task<string> GetRecordInformationAsync1(long summonerId);
        Task<string> GetSummonerInformationAsync(long summonerId);
        Task<string> GetSummonerRankInformationAsync(string puuid);
    }
}
