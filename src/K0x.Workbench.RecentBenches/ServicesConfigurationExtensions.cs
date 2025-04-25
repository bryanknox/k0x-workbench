using K0x.DataStorage.JsonFiles;
using K0x.Workbench.RecentBenches.Abstractions;
using K0x.Workbench.RecentBenches.Internal;
using Microsoft.Extensions.DependencyInjection;
using System.IO.Abstractions;

namespace K0x.Workbench.RecentBenches;

public static class ServicesConfigurationExtensions
{
    public static void AddRecentBenchesJsonFiles(this IServiceCollection services)
    {
        services.AddScoped<IJsonFileLoader<RecentBenchesFileModel>, JsonFileLoader<RecentBenchesFileModel>>();
        services.AddScoped<IJsonFileSaver<RecentBenchesFileModel>, JsonFileSaver<RecentBenchesFileModel>>();
        services.AddScoped<IRecentBenchAdder, RecentBenchAdder>();
        services.AddScoped<IRecentBenchesFilePathProvider, RecentBenchesFilePathProvider>();
        services.AddScoped<IRecentBenchesJsonFileLoader, RecentBenchesJsonFileLoader>();
        services.AddScoped<IRecentBenchesJsonFileSaver, RecentBenchesJsonFileSaver>();
        services.AddScoped<IRecentBenchesLoader, RecentBenchesLoader>();

        // Third-party services
        services.AddScoped<IFileSystem, FileSystem>(); // From TestableIO.System.IO.Abstractions.Wrappers NuGet.
        services.AddSingleton(TimeProvider.System);
    }
}
