﻿using K0x.DataStorage.JsonFiles;
using K0x.Workbench.RecentBenches.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace K0x.Workbench.RecentBenches;

public static class ServicesConfigurationExtensions
{
    public static void AddRecentBenchesJsonFiles(this IServiceCollection services)
    {
        services.AddScoped<IAppExeFolderPathProvider, AppExeFolderPathProvider>();
        services.AddScoped<IJsonFileLoader<RecentBenchesFileModel>, JsonFileLoader<RecentBenchesFileModel>>();
        services.AddScoped<IJsonFileSaver<RecentBenchesFileModel>, JsonFileSaver<RecentBenchesFileModel>>();
        services.AddScoped<IRecentBenchAdder, RecentBenchAdder>();
        services.AddScoped<IRecentBenchesFilePathProvider, RecentBenchesFilePathProvider>();
        services.AddScoped<IRecentBenchesJsonFileLoader, RecentBenchesJsonFileLoader>();
        services.AddScoped<IRecentBenchesJsonFileSaver, RecentBenchesJsonFileSaver>();

        services.AddSingleton(TimeProvider.System);
    }
}
