using LeagueOfLegendsBoxer.Helpers;
using LeagueOfLegendsBoxer.Pages;
using LeagueOfLegendsBoxer.Resources;
using LeagueOfLegendsBoxer.ViewModels;
using LeagueOfLegendsBoxer.ViewModels.Pages;
using LeagueOfLegendsBoxer.Windows;
using LeagueOfLegendsBoxerApplication.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

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
            DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

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
                services.AddSingleton<BlackList>();
                services.AddSingleton<BlackListViewModel>();
                services.AddSingleton<BlackRecord>();
                services.AddSingleton<BlackRecordViewModel>();
                //pages
                services.AddSingleton<MainPage>();
                services.AddSingleton<MainViewModel>();
                services.AddSingleton<Settings>();
                services.AddSingleton<SettingsViewModel>();
                services.AddSingleton<RuneAndItemPage>();
                services.AddSingleton<RuneAndItemViewModel>();
                services.AddSingleton<HeroData>();
                services.AddSingleton<HeroDataViewModel>();
                services.AddSingleton<Teammate>();
                services.AddSingleton<TeammateViewModel>();
                services.AddTransient<SummonerDetail>();
                services.AddTransient<SummonerDetailViewModel>();
                services.AddSingleton<Notice>();
                services.AddSingleton<NoticeViewModel>();
            });

            return hostBuilder;
        }

        #region 全局异常处理
        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                e.Handled = true; //把 Handled 属性设为true，表示此异常已处理，程序可以继续运行，不会强制退出      
                ServiceProvider.GetRequiredService<ILogger<App>>()?.LogError(e.ToString());
            }
            catch (Exception ex)
            {
                ServiceProvider.GetRequiredService<ILogger<App>>()?.LogError(ex.ToString());
            }

        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            StringBuilder sbEx = new StringBuilder();
            if (e.IsTerminating)
            {
                sbEx.Append("非UI线程发生致命错误");
            }
            sbEx.Append("非UI线程异常：");
            if (e.ExceptionObject is Exception)
            {
                sbEx.Append(((Exception)e.ExceptionObject).Message);
            }
            else
            {
                sbEx.Append(e.ExceptionObject);
            }

            ServiceProvider.GetRequiredService<ILogger<App>>()?.LogError(sbEx.ToString());
        }

        void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            ServiceProvider.GetRequiredService<ILogger<App>>()?.LogError(e.Exception.ToString());
            e.SetObserved();
        }
        #endregion
    }
}
