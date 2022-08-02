namespace LeagueOfLegendsBoxer.Application.Game
{
    public interface IGameService
    {
        Task AutoAcceptGameAsync();
        Task<string> GetGameSessionAsync();
        Task AutoLockHeroAsync(int actionID, int champID);
        Task AutoDisableHeroAsync();
        Task<string> GetChatConversation();
        Task<string> GetChatConversationMessages(string chatId);
        Task<string> GetChampRankAsync(string lane, int tier, int time);
        Task<string> GetChampRestraintDataAsync(int champID);
        Task SendMessageAsync(string chatId,string message);
        Task<string> QueryGameDetailAsync(long gameId);
        Task<string> QuerySummonerSuperChampDataAsync(long summonerId);
        Task<string> test(long id);
        Task<string> GetCurrentGameInfoAsync();
        Task<string> GetCurrentChampionInfoAsync();
        Task<string> GetPickableChampionsAsync();
        Task BenchSwapChampionsAsync(int champID);
        //rune
        Task<string> GetAllRunePages();
        Task DeleteRunePage(long id);
        Task AddRunePage(dynamic body);
        Task<string> GetSkinsByHeroId(int id);
        Task<byte[]> GetResourceByUrl(string url);
        Task<string> SetSkinAsync(dynamic body);
        Task<string> SetIconAsync(dynamic body);
        Task<string> SendGameChatMessageAsync(string message);
        Task<string> GetItems();
        Task<string> GetIcons();
        Task<string> GetRecordsByPage(int pageStart, int pageEnd, string id);
    }
}
