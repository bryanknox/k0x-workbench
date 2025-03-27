using K0x.Workbench.RecentBenches.Abstractions.Models;

namespace K0x.Workbench.RecentBenches.Internal;

public interface IRecentBenchesJsonFileLoader
{
    Task<List<RecentBench>> LoadAsync(string filePath);
}
