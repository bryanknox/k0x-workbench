using K0x.Workbench.RecentBenches.Abstractions;
using K0x.Workbench.RecentBenches.Abstractions.Models;

namespace K0x.Workbench.RecentBenches;

public class RecentBenchAdder : IRecentBenchAdder
{
    private readonly IRecentBenchesFilePathProvider _recentBenchesFilePathProvider;
    private readonly IRecentBenchesJsonFileLoader _recentBenchesFileLoader;
    private readonly IRecentBenchesJsonFileSaver _recentBenchesFileSaver;
    private readonly TimeProvider _timeProvider;

    public RecentBenchAdder(
        IRecentBenchesFilePathProvider recentBenchesFilePathProvider,
        IRecentBenchesJsonFileLoader recentBenchesFileLoader,
        IRecentBenchesJsonFileSaver recentBenchesFileSaver,
        TimeProvider timeProvider)
    {
        _recentBenchesFilePathProvider = recentBenchesFilePathProvider;
        _recentBenchesFileLoader = recentBenchesFileLoader;
        _recentBenchesFileSaver = recentBenchesFileSaver;
        _timeProvider = timeProvider;
    }

    public async Task AddRecentBenchAsync(string benchFilePath, string benchLabel)
    {
        // We load the recent benches file before adding the new bench
        // because the file may have been updated by another process,
        // and we don't want to overwrite the potentical changes made
        // there.

        string recentBenchesFilePath = _recentBenchesFilePathProvider.GetFilePath();

        List<RecentBench> recentBenches = await _recentBenchesFileLoader.LoadAsync(recentBenchesFilePath);

        Upsert(recentBenches, benchFilePath, benchLabel);

        await _recentBenchesFileSaver.SaveAsync(recentBenches, recentBenchesFilePath);
    }

    private void Upsert(List<RecentBench> recentBenches, string benchFilePath, string benchLabel)
    {
        RecentBench? existingBench = recentBenches.FirstOrDefault(b => b.FilePath == benchFilePath);
        if (existingBench != null) {
            recentBenches.Remove(existingBench);
        }
        recentBenches.Add(new RecentBench
        {
            FilePath = benchFilePath,
            BenchLabel = benchLabel,
            LastOpened = _timeProvider.GetLocalNow()
        });
    }
}
