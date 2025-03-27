using K0x.Workbench.RecentBenches.Abstractions.Models;

namespace K0x.Workbench.RecentBenches.Abstractions;

public interface IRecentBenchesLoader
{
    Task<List<RecentBench>> GetRecentBenchesAsync();
}
