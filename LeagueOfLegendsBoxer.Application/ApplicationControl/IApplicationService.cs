using System.Threading.Tasks;

namespace LeagueOfLegendsBoxer.Application.ApplicationControl
{
    public interface IApplicationService
    {
        Task QuitAsync();
        Task RestartAsync(int delaySeconds);
        Task RestartAsync(int delaySeconds, int restartVersion);
        Task RestartToRepair();
        Task RestartToUpdate(int delaySeconds, string selfUpdateUrl);
        Task<string> GetInstallLocation();
        Task<string> SetRankAsync(dynamic body);
        Task<string> SetSignatureAsync(dynamic body);
        Task CreateQueueAsync(int queue);
        Task<string> GetQueuesAsync();
    }
}
