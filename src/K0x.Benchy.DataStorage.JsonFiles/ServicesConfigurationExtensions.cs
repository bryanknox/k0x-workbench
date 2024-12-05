using K0x.Benchy.DataStorage.Abstractions;
using K0x.DataStorage.JsonFiles;
using Microsoft.Extensions.DependencyInjection;

namespace K0x.Benchy.DataStorage.JsonFiles;

public static class ServicesConfigurationExtensions
{
    public static void AddBenchyJsonFiles(this IServiceCollection services)
    {
        services.AddSingleton<IBenchProvider, BenchProvider>();

        services.AddScoped<IJsonFileLoader<BenchyJsonFileModel>, JsonFileLoader<BenchyJsonFileModel>>();

        services.AddSingleton<IBenchyFileLoader, BenchyJsonFileLoader>();

        services.AddScoped<IJsonFileSaver<BenchyJsonFileModel>, JsonFileSaver<BenchyJsonFileModel>>();
        services.AddScoped<IBenchyFileSaver, BenchyJsonFileSaver>();
    }
}
