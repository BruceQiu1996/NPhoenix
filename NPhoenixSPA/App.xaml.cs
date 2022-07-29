using HandyControl.Controls;
using HandyControl.Data;
using LeagueOfLegendsBoxerApplication.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NPhoenixSPA.Pages;
using NPhoenixSPA.Resources;
using NPhoenixSPA.ViewModels;
using Serilog;
using System;
using System.Security.Principal;
using System.Windows;

namespace NPhoenixSPA
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider;
        protected async override void OnStartup(StartupEventArgs e)
        {
            if (!IsAdministrator()) 
            {
                HandyControl.Controls.MessageBox.Show("请以管理员身份运行该软件");
                Environment.Exit(0);
            }
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
            hostBuilder.ConfigureServices((ctx, services) =>
            {
                services.AddApplicationServices();
                services.AddSingleton<MainWindow>();
                services.AddSingleton<MainWindowViewModel>();
                services.AddSingleton<IniSettingsModel>();
                //pages
                services.AddSingleton<Account>();
                services.AddSingleton<AccountViewModel>();
            });

            return hostBuilder;
        }
        public static bool IsAdministrator()
        {
            WindowsIdentity current = WindowsIdentity.GetCurrent();
            WindowsPrincipal windowsPrincipal = new WindowsPrincipal(current);
            return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
