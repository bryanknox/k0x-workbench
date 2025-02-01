using K0x.DataStorage.JsonFiles;
using K0x.Workbench.RecentBenches.Abstractions.Models;

namespace K0x.Workbench.RecentBenches;

public class RecentBenchesJsonFileSaver : IRecentBenchesJsonFileSaver
{
    private readonly IJsonFileSaver<RecentBenchesFileModel> _jsonFileSaver;

    public RecentBenchesJsonFileSaver(IJsonFileSaver<RecentBenchesFileModel> jsonFileSaver)
    {
        _jsonFileSaver = jsonFileSaver;
    }

    public async Task SaveAsync(List<RecentBench> recentBenches, string filePath)
    {
        var fileModel = new RecentBenchesFileModel
        {
            RecentBenches = recentBenches
        };

        await _jsonFileSaver.SaveAsync(fileModel, filePath);
    }
}
