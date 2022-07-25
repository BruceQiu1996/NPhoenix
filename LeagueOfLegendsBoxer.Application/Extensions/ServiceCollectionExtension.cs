using LeagueOfLegendsBoxer.Application.Account;
using LeagueOfLegendsBoxer.Application.ApplicationControl;
using LeagueOfLegendsBoxer.Application.Client;
using LeagueOfLegendsBoxer.Application.Event;
using LeagueOfLegendsBoxer.Application.Game;
using LeagueOfLegendsBoxer.Application.Request;
using LeagueOfLegendsBoxer.Application.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace LeagueOfLegendsBoxerApplication.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void AddApplicationServices(this IServiceCollection serviceDescriptors) 
        {
            serviceDescriptors.AddSingleton<IRequestService, DefaultRequestService>();
            serviceDescriptors.AddSingleton<IApplicationService,DefaultApplicationService>();
            serviceDescriptors.AddSingleton<IClientService, DefaultClientService>();
            serviceDescriptors.AddSingleton<ISettingsService, DefaultIniSettingsService>();
            serviceDescriptors.AddSingleton<IEventService, DefaultEventService>();
            serviceDescriptors.AddSingleton<IGameService, DefaultGameService>();
            serviceDescriptors.AddSingleton<IAccountService, DefaultAccountService>();
        }
    }
}
