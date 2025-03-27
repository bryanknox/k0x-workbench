using K0x.Workbench.RecentBenches.Abstractions.Models;

namespace K0x.Workbench.RecentBenches.Internal;

public interface IRecentBenchesJsonFileSaver
{
    Task SaveAsync(List<RecentBench> recentBenches, string filePath);
}
