﻿using K0x.DataStorage.JsonFiles;
using K0x.Workbench.DataStorage.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace K0x.Workbench.DataStorage.JsonFiles;

public static class ServicesConfigurationExtensions
{
    public static void AddBenchJsonFiles(this IServiceCollection services)
    {
        services.AddSingleton<IBenchFilePathProvider, BenchFilePathProvider>();

        services.AddSingleton<IBenchProvider, BenchProvider>();

        services.AddScoped<IJsonFileLoader<BenchJsonFileModel>, JsonFileLoader<BenchJsonFileModel>>();

        services.AddSingleton<IBenchFileLoader, BenchJsonFileLoader>();

        services.AddScoped<IJsonFileSaver<BenchJsonFileModel>, JsonFileSaver<BenchJsonFileModel>>();
        services.AddScoped<IBenchFileSaver, BenchJsonFileSaver>();
    }
}
