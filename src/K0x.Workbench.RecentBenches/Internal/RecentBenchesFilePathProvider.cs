using K0x.Workbench.Files.Abstractions;
using K0x.Workbench.RecentBenches.Abstractions;
using Microsoft.Extensions.Configuration;

namespace K0x.Workbench.RecentBenches.Internal;

public class RecentBenchesFilePathProvider : IRecentBenchesFilePathProvider
{
    private readonly IDataFolderPathProvider _folderPathProvider;

    public RecentBenchesFilePathProvider(
        IDataFolderPathProvider recentBenchesFolderPathProvider
    )
    {
        _folderPathProvider = recentBenchesFolderPathProvider;
    }

    public string GetFilePath()
    {
        string folderPath = _folderPathProvider.GetDataFolderPath();
        string filePath = Path.Combine(folderPath, RecentBenchesConstants.DefaultFileName);

        return filePath;
    }
}
