using K0x.Benchy.DataStorage.Abstractions;
using K0x.DataStorage.JsonFiles;
using Microsoft.Extensions.DependencyInjection;

namespace K0x.Benchy.DataStorage.JsonFiles;

public static class ServicesConfigurationExtensions
{
    public static void AddBenchJsonFiles(this IServiceCollection services)
    {
        services.AddSingleton<IBenchProvider, BenchProvider>();

        services.AddScoped<IJsonFileLoader<BenchJsonFileModel>, JsonFileLoader<BenchJsonFileModel>>();

        services.AddSingleton<IBenchFileLoader, BenchJsonFileLoader>();

        services.AddScoped<IJsonFileSaver<BenchJsonFileModel>, JsonFileSaver<BenchJsonFileModel>>();
        services.AddScoped<IBenchFileSaver, BenchJsonFileSaver>();
    }
}
