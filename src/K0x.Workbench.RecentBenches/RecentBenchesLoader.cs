using K0x.Workbench.RecentBenches.Abstractions;
using K0x.Workbench.RecentBenches.Abstractions.Models;
using K0x.Workbench.RecentBenches.Internal;

namespace K0x.Workbench.RecentBenches;

public class RecentBenchesLoader : IRecentBenchesLoader
{
    private readonly IRecentBenchesJsonFileLoader _jsonFileLoader;
    private readonly IRecentBenchesFilePathProvider _filePathProvider;

    public RecentBenchesLoader(
        IRecentBenchesJsonFileLoader jsonFileLoader,
        IRecentBenchesFilePathProvider filePathProvider)
    {
        _jsonFileLoader = jsonFileLoader;
        _filePathProvider = filePathProvider;
    }

    public async Task<List<RecentBench>> GetRecentBenchesAsync()
    {
        var filePath = _filePathProvider.GetFilePath();
        return await _jsonFileLoader.LoadAsync(filePath);
    }
}
