﻿
using Microsoft.Extensions.DependencyInjection;

namespace WpfBlazor.InternalServices;

public static class ServicesConfigurationExtensions
{
    public static void AddAppTitleService(this IServiceCollection services)
    {
        // A single instance of TitleService implements multiple interfaces.
        services.AddSingleton<AppTitleService>();
        services.AddSingleton<IAppTitleGetService>(s => s.GetRequiredService<AppTitleService>());
        services.AddSingleton<IAppTitleSetService>(s => s.GetRequiredService<AppTitleService>());
    }
}
