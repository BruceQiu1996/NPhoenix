namespace LeagueOfLegendsBoxer.Application.LiveGame
{
    public interface ILiveGameService
    {
        Task<string> GetGameEventAsync();
    }
}
