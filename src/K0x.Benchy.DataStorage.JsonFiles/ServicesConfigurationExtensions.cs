using K0x.Benchy.DataStorage.Abstractions;
using K0x.DataStorage.JsonFiles;
using Microsoft.Extensions.DependencyInjection;

namespace K0x.Benchy.DataStorage.JsonFiles;

public static class ServicesConfigurationExtensions
{
    public static void AddBenchyJsonFileLoader(this IServiceCollection services)
    {
        services.AddSingleton<IJsonFileLoader<BenchyJsonFileModel>, JsonFileLoader<BenchyJsonFileModel>>();

        // A single instance of BenchyJsonFileDataLoader can provide the implementation of multiple interfaces.
        services.AddSingleton<BenchyJsonFileLoader>();
        services.AddSingleton<IBenchyFileLoader>(s => s.GetRequiredService<BenchyJsonFileLoader>());
    }
}
