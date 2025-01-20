using K0x.Workbench.RecentBenches.Abstractions.Models;

namespace K0x.Workbench.RecentBenches;

public class RecentBenchesFileModel
{
    public IList<RecentBench> RecentBenches { get; init; } = new List<RecentBench>();
}
