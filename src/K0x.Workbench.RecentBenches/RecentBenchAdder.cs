using K0x.Workbench.RecentBenches.Abstractions;
using K0x.Workbench.RecentBenches.Abstractions.Models;
using System.Data;

namespace K0x.Workbench.RecentBenches;

public class RecentBenchAdder : IRecentBenchAdder
{
    private readonly IRecentBenchesFilePathProvider _recentBenchesFilePathProvider;
    private readonly RecentBenchesJsonFileLoader _recentBenchesFileLoader;
    private readonly RecentBenchesJsonFileSaver _recentBenchesFileSaver;

    public RecentBenchAdder(
        IRecentBenchesFilePathProvider recentBenchesFilePathProvider,
        RecentBenchesJsonFileLoader recentBenchesFileLoader,
        RecentBenchesJsonFileSaver recentBenchesFileSaver)
    {
        _recentBenchesFilePathProvider = recentBenchesFilePathProvider;
        _recentBenchesFileLoader = recentBenchesFileLoader;
        _recentBenchesFileSaver = recentBenchesFileSaver;
    }

    public async Task AddRecentBenchAsync(string benchFilePath, string benchLabel)
    {
        // We load the recent benches file before adding the new bench
        // because the file may have been updated by another process,
        // and we don't want to overwrite the potentical changes made
        // there.

        string recentBenchesFilePath = _recentBenchesFilePathProvider.GetFilePathOrThrow();

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
            LastOpened = DateTimeOffset.Now // TODO: Use testable time provider
        });
    }
}
