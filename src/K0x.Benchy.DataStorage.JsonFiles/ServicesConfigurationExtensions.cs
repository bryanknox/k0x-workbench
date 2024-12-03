using K0x.Benchy.DataStorage.Abstractions;
using K0x.DataStorage.JsonFiles;
using Microsoft.Extensions.DependencyInjection;

namespace K0x.Benchy.DataStorage.JsonFiles;

public static class ServicesConfigurationExtensions
{
    public static void AddBenchyJsonFiles(this IServiceCollection services, string benchJsonFilePath)
    {
        services.AddScoped<IJsonFileLoader<BenchyJsonFileModel>, JsonFileLoader<BenchyJsonFileModel>>();

        services.AddSingleton<IBenchyFileLoader>(provider =>
        {
            var jsonFileLoader = provider.GetRequiredService<IJsonFileLoader<BenchyJsonFileModel>>();
            return new BenchyJsonFileLoader(jsonFileLoader, benchJsonFilePath);
        });

        services.AddScoped<IJsonFileSaver<BenchyJsonFileModel>, JsonFileSaver<BenchyJsonFileModel>>();
        services.AddScoped<IBenchyFileSaver, BenchyJsonFileSaver>();
    }
}
