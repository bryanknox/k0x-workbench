using K0x.DataStorage.JsonFiles;
using K0x.Workbench.RecentBenches.Abstractions.Models;
using System.IO.Abstractions;

namespace K0x.Workbench.RecentBenches.Internal;

public class RecentBenchesJsonFileLoader : IRecentBenchesJsonFileLoader
{
    private readonly IFileSystem _fileSystem;
    private readonly IJsonFileLoader<RecentBenchesFileModel> _jsonFileLoader;

    public RecentBenchesJsonFileLoader(
        IFileSystem fileSystem,
        IJsonFileLoader<RecentBenchesFileModel> jsonFileLoader)
    {
        _fileSystem = fileSystem;
        _jsonFileLoader = jsonFileLoader;
    }

    public async Task<List<RecentBench>> LoadAsync(string filePath)
    {
        if (!_fileSystem.File.Exists(filePath))
        {
            return new List<RecentBench>();
        }

        RecentBenchesFileModel fileModel = await _jsonFileLoader.LoadAsync(filePath);

        return fileModel.RecentBenches.ToList();
    }
}
