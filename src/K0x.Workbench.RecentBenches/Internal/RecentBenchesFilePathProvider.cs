using K0x.Workbench.Files.Abstractions;
using K0x.Workbench.RecentBenches.Abstractions;
using Microsoft.Extensions.Configuration;

namespace K0x.Workbench.RecentBenches.Internal;

public class RecentBenchesFilePathProvider : IRecentBenchesFilePathProvider
{
    private readonly ILocalAppDataFolderPathProvider _folderPathProvider;

    public RecentBenchesFilePathProvider(
        ILocalAppDataFolderPathProvider recentBenchesFolderPathProvider
    )
    {
        _folderPathProvider = recentBenchesFolderPathProvider;
    }

    public string GetFilePath()
    {
        string folderPath = _folderPathProvider.GetLocalAppDataFolderPath();
        string filePath = Path.Combine(folderPath, RecentBenchesConstants.DefaultFileName);

        return filePath;
    }
}
