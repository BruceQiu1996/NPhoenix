namespace LeagueOfLegendsBoxer.Application.LiveGame
{
    public interface ILiveGameService
    {
        Task<string> GetGameEventAsync();
        Task<string> GetPlayersAsync(int teamId);
        Task<string> GetDataByNameAsync(string summonerName);
        Task<string> GetSpellByNameAsync(string summonerName);
    }
}
