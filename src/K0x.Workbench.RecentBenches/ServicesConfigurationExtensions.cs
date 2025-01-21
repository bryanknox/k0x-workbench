using K0x.Workbench.RecentBenches.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace K0x.Workbench.RecentBenches;

public static class ServicesConfigurationExtensions
{
    public static void AddRecentBenchesJsonFiles(this IServiceCollection services)
    {
        services.AddSingleton<IRecentBenchesFilePathProvider, RecentBenchesFilePathProvider>();
    }
}
