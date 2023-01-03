using LeagueOfLegendsBoxer.Application.Event;
using LeagueOfLegendsBoxer.Helpers;
using LeagueOfLegendsBoxer.Pages;
using LeagueOfLegendsBoxer.Resources;
using LeagueOfLegendsBoxer.ViewModels;
using LeagueOfLegendsBoxer.ViewModels.Pages;
using LeagueOfLegendsBoxer.Windows;
using LeagueOfLegendsBoxerApplication.Extensions;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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
        public EventWaitHandle ProgramStarted { get; set; }
        private const int SW_SHOWNOMAL = 1;
        public static Theme CURRENT_THEME;
        public static IServiceProvider ServiceProvider;
        public static HubConnection HubConnection;

        protected async override void OnStartup(StartupEventArgs e)
        {
            string mutexName = "32283F61-EC4D-43B1-9C44-40280D5854DD";
            ProgramStarted = new EventWaitHandle(false, EventResetMode.AutoReset, mutexName, out var createNew);
            if (!createNew)
            {
                Process current = Process.GetCurrentProcess();
                var processes = Process.GetProcessesByName(Assembly.GetExecutingAssembly().GetName().Name);
                foreach (Process process in processes)
                {
                    if (process.Id != current.Id)
                    {
                        SetForegroundWindow(process.MainWindowHandle);
                        ShowWindow(process.MainWindowHandle, WindowShowStyle.Show);
                        break;
                    }
                }
                Current.Shutdown();
                Environment.Exit(-1);
            }

            base.OnStartup(e);
            DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            var hostbuilder = CreateHostBuilder(e.Args);
            var host = await hostbuilder.StartAsync();
            ServiceProvider = host.Services;
            await host.Services.GetRequiredService<IniSettingsModel>().Initialize();
            ChangeTheme(host.Services.GetRequiredService<IniSettingsModel>().IsDarkTheme ? Theme.Dark : Theme.Light);
            host.Services.GetRequiredService<MainWindow>()?.Show();
            CheckForStartGame();
        }

        protected async override void OnExit(ExitEventArgs e)
        {
            await ServiceProvider.GetRequiredService<IEventService>().DisconnectAsync();
        }

        private void CheckForStartGame()
        {
            if (ServiceProvider.GetRequiredService<IniSettingsModel>().AutoStartGame)
            {
                Process[] ps = Process.GetProcesses();
                if (!ps.Where(x => !string.IsNullOrEmpty(x.ProcessName) && x.ProcessName.Contains("LeagueClient")).Any())
                {
                    ServiceProvider.GetRequiredService<SettingsViewModel>().StartGame();
                }
            }
        }

        public static void ChangeTheme(Theme theme)
        {
            var mergedDictionaries = Current.Resources.MergedDictionaries;

            foreach (var merged in mergedDictionaries)
            {
                if (merged.Source != null && merged.Source.ToString().Contains("_theme"))
                {
                    mergedDictionaries.Remove(merged);
                    break;
                }
            }

            mergedDictionaries.Add(new ResourceDictionary { Source = new Uri($"pack://application:,,,/LeagueOfLegendsBoxer;component/Resources/Theme/{theme}_theme.xaml") });
            CURRENT_THEME = theme;
        }

        public enum Theme
        {
            Dark,
            Light
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args).UseSerilog((context, logger) =>//注册Serilog
            {
                logger.ReadFrom.Configuration(context.Configuration);
                logger.Enrich.FromLogContext();
            });
            hostBuilder.ConfigureServices((ctx, services) =>
            {
                services.AddApplicationServices();
                services.AddSingleton<MainWindow>();
                services.AddSingleton<MainWindowViewModel>();
                services.AddTransient<SummonerAnalyse>();
                services.AddTransient<SummonerAnalyseViewModel>();
                services.AddSingleton<ServerArea>();
                services.AddSingleton<ServerAreaViewModel>();
                services.AddSingleton<ChampionSelectTool>();
                services.AddSingleton<ChampionSelectToolViewModel>();
                services.AddTransient<SkinsWindow>();
                services.AddTransient<SkinsWindowViewModel>();
                services.AddTransient<AramQuickChoose>();
                services.AddTransient<AramQuickChooseViewModel>();
                services.AddSingleton<IniSettingsModel>();
                services.AddSingleton<ImageManager>();
                services.AddSingleton<RuneHelper>();
                services.AddSingleton<HtmlHelper>();
                services.AddSingleton<EnumHelper>();
                services.AddSingleton<SoftwareHelper>();
                services.AddSingleton<Team1V2Window>();
                services.AddSingleton<Team1V2WindowViewModel>();
                services.AddSingleton<BlackList>();
                services.AddSingleton<BlackListViewModel>();
                services.AddSingleton<BlackRecord>();
                services.AddSingleton<BlackRecordViewModel>();
                services.AddSingleton<ManageRune>();
                services.AddSingleton<ManageRuneViewModel>();
                services.AddSingleton<BlackTip>();
                services.AddSingleton<BlackTipViewModel>();
                services.AddSingleton<Pay>();
                services.AddSingleton<Post>();
                services.AddSingleton<PostViewModel>();
                services.AddSingleton<PostDetailWindow>();
                services.AddSingleton<PostDetailWindowViewModel>();
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
                services.AddSingleton<RecordRank>();
                services.AddSingleton<RecordRankViewModel>();
                services.AddTransient<SelectKey>();
                services.AddSingleton<Teamup>();
                services.AddSingleton<TeamupViewModel>();

                services.Configure<List<Models.ServerArea>>(ctx.Configuration.GetSection("ServerAreas"));
            });

            return hostBuilder;
        }

        #region 全局异常处理
        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                e.Handled = true; //把 Handled 属性设为true，表示此异常已处理，程序可以继续运行，不会强制退出      
                File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error.txt"), e.Exception.ToString());
            }
            catch (Exception ex)
            {
                File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error.txt"), ex.ToString());
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
                sbEx.Append(((Exception)e.ExceptionObject).ToString());
            }
            else
            {
                sbEx.Append(e.ExceptionObject);
            }

            File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error.txt"), sbEx.ToString());
        }

        async void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            await File.AppendAllTextAsync(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error.txt"), e.Exception.ToString());
            e.SetObserved();
        }
        #endregion

        internal enum WindowShowStyle : uint
        {
            /// <summary>Hides the window and activates another window.</summary>
            /// <remarks>See SW_HIDE</remarks>
            Hide = 0,
            /// <summary>Activates and displays a window. If the window ..
            /// or maximized, the system restores it to its original size and
            /// position. An application should specify this flag when displaying
            /// the window for the first time.</summary>
            /// <remarks>See SW_SHOWNORMAL</remarks>
            ShowNormal = 1,
            /// <summary>Activates the window and displays it ..</summary>
            /// <remarks>See SW_SHOWMINIMIZED</remarks>
            ShowMinimized = 2,
            /// <summary>Activates the window and displays it ..</summary>
            /// <remarks>See SW_SHOWMAXIMIZED</remarks>
            ShowMaximized = 3,
            /// <summary>Maximizes the specified window.</summary>
            /// <remarks>See SW_MAXIMIZE</remarks>
            Maximize = 3,
            /// <summary>Displays a window in its most recent size and position.
            /// This value is similar to "ShowNormal", except the window is not
            /// actived.</summary>
            /// <remarks>See SW_SHOWNOACTIVATE</remarks>
            ShowNormalNoActivate = 4,
            /// <summary>Activates the window and displays it in its current size
            /// and position.</summary>
            /// <remarks>See SW_SHOW</remarks>
            Show = 5,
            /// <summary>Minimizes the specified window and activates the next
            /// top-level window in the Z order.</summary>
            /// <remarks>See SW_MINIMIZE</remarks>
            Minimize = 6,
            /// <summary>Displays the window as a minimized window. This value is
            /// similar to "ShowMinimized", except the window ..</summary>
            /// <remarks>See SW_SHOWMINNOACTIVE</remarks>
            ShowMinNoActivate = 7,
            /// <summary>Displays the window in its current size and position. This
            /// value is similar to "Show", except the window ..</summary>
            /// <remarks>See SW_SHOWNA</remarks>
            ShowNoActivate = 8,
            /// <summary>Activates and displays the window. If the window is
            /// minimized or maximized, the system restores it to its original size
            /// and position. An application should specify this flag ..
            /// a minimized window.</summary>
            /// <remarks>See SW_RESTORE</remarks>
            Restore = 9,
            /// <summary>Sets the show state based on the SW_ value specified ..
            /// STARTUPINFO structure passed to the CreateProcess function by the
            /// program that started the application.</summary>
            /// <remarks>See SW_SHOWDEFAULT</remarks>
            ShowDefault = 10,
            /// <summary>Windows 2000/XP: Minimizes a window, even if the thread
            /// that owns the window is hung. This flag should only be used when
            /// minimizing windows from a different thread.</summary>
            /// <remarks>See SW_FORCEMINIMIZE</remarks>
            ForceMinimized = 11
        }

        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, WindowShowStyle nCmdShow);

        [DllImport("User32.dll")]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}
