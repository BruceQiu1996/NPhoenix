namespace LeagueOfLegendsBoxer.Application.Settings
{
    public interface ISettingsService
    {
        Task Initialize(string path);
        Task WriteAsync(string section, string key, string value);
        Task<string> ReadAsync(string section, string key);
    }
}
