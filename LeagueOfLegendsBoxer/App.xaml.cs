using LeagueOfLegendsBoxer.Helpers;
using LeagueOfLegendsBoxer.Pages;
using LeagueOfLegendsBoxer.Resources;
using LeagueOfLegendsBoxer.ViewModels;
using LeagueOfLegendsBoxer.ViewModels.Pages;
using LeagueOfLegendsBoxer.Windows;
using LeagueOfLegendsBoxerApplication.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Windows;

namespace LeagueOfLegendsBoxer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public static IServiceProvider ServiceProvider;
        protected async override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var hostbuilder = CreateHostBuilder(e.Args);
            var host = await hostbuilder.StartAsync();
            ServiceProvider = host.Services;
            host.Services.GetRequiredService<MainWindow>()?.Show();
        }
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args).UseSerilog((context, logger) =>//注册Serilog
            {
                logger.ReadFrom.Configuration(context.Configuration);
                logger.Enrich.FromLogContext();
            });
            hostBuilder.ConfigureServices((ctx,services) =>
            {
                services.AddApplicationServices();
                services.AddSingleton<MainWindow>();
                services.AddSingleton<MainWindowViewModel>();
                services.AddTransient<SummonerAnalyse>();
                services.AddTransient<SummonerAnalyseViewModel>();
                services.AddSingleton<ChampionSelectTool>();
                services.AddSingleton<ChampionSelectToolViewModel>();
                services.AddTransient<SkinsWindow>();
                services.AddTransient<SkinsWindowViewModel>();
                services.AddTransient<AramQuickChoose>();
                services.AddTransient<AramQuickChooseViewModel>();
                services.AddSingleton<IniSettingsModel>();
                services.AddSingleton<ImageManager>();
                services.AddSingleton<RuneHelper>();
                services.AddSingleton<Team1V2Window>();
                services.AddSingleton<Team1V2WindowViewModel>();
                //pages
                services.AddSingleton<MainPage>();
                services.AddSingleton<MainViewModel>();
                services.AddSingleton<Settings>();
                services.AddSingleton<SettingsViewModel>();
                services.AddSingleton<RunePage>();
                services.AddSingleton<RuneViewModel>();
                services.AddSingleton<HeroData>();
                services.AddSingleton<HeroDataViewModel>();
                services.AddSingleton<Teammate>();
                services.AddSingleton<TeammateViewModel>();
                services.AddTransient<SummonerDetail>();
                services.AddTransient<SummonerDetailViewModel>();
            });

            return hostBuilder;
        }
    }
}
