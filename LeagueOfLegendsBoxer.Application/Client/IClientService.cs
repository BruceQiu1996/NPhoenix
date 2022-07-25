namespace LeagueOfLegendsBoxer.Application.Client
{
    public interface IClientService
    {
        Task MinimizeUxAsync();
        Task ShowUxAsync();
        Task FlashUxAsync();
        Task KillUxAsync();
        Task KillAndRestartUxAsync();
        Task UnloadUxAsync();
        Task LaunchUxAsync();
        Task<double> GetZoomScaleAsync();
        Task SetZoomScaleAsync(double scale);
    }
}
