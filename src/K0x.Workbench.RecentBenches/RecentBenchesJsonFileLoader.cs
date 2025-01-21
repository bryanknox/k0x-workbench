using K0x.DataStorage.JsonFiles;
using K0x.Workbench.RecentBenches.Abstractions.Models;

namespace K0x.Workbench.RecentBenches;

public class RecentBenchesJsonFileLoader
{
    private readonly IJsonFileLoader<RecentBenchesFileModel> _jsonFileLoader;

    public RecentBenchesJsonFileLoader(IJsonFileLoader<RecentBenchesFileModel> jsonFileLoader)
    {
        _jsonFileLoader = jsonFileLoader;
    }

    public async Task<List<RecentBench>> LoadAsync(string filePath)
    {
        RecentBenchesFileModel fileModel = await _jsonFileLoader.LoadAsync(filePath);

        return fileModel.RecentBenches.ToList();
    }
}
