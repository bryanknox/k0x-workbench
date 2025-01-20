using K0x.Workbench.RecentBenches.Abstractions;
using K0x.Workbench.RecentBenches.Abstractions.Models;

namespace K0x.Workbench.RecentBenches;

public class RecentBenchAdder : IRecentBenchAdder
{
    private readonly RecentBenchesFileLoader _recentBenchesFileLoader;
    private readonly RecentBenchesFileSaver _recentBenchesFileSaver;

    public RecentBenchAdder(
        RecentBenchesFileLoader recentBenchesFileLoader,
        RecentBenchesFileSaver recentBenchesFileSaver)
    {
        _recentBenchesFileLoader = recentBenchesFileLoader;
        _recentBenchesFileSaver = recentBenchesFileSaver;
    }

    public Task AddRecentBenchAsync(string filePath, string benchLabel)
    {
        // We load the recent benches file before adding the new bench
        // because the file may have been updated by another process,
        // and we don't want to overwrite the potentical changes made
        // there.

                
        throw new NotImplementedException();
    }
}
