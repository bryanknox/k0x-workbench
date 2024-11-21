using K0x.Benchy.DataStorage.Abstractions.Services;
using K0x.Benchy.DataStorage.JsonFiles.FileModels;
using K0x.Benchy.DataStorage.JsonFiles.Services;
using K0x.DataStorage.JsonFiles;
using Microsoft.Extensions.DependencyInjection;

namespace K0x.Benchy.DataStorage.JsonFiles;

public static class ServicesConfigurationExtensions
{
    public static void AddBenchyJsonFileDataLoader(this IServiceCollection services)
    {
        services.AddSingleton<IJsonFileService<BenchyJsonFileModel>, JsonFileService<BenchyJsonFileModel>>();

        // A single instance of BenchyJsonFileDataLoader can provide the implementation of multiple interfaces.
        services.AddSingleton<BenchyJsonFileDataLoader>();
        services.AddSingleton<IBenchyDataLoader>(s => s.GetRequiredService<BenchyJsonFileDataLoader>());
    }
}
