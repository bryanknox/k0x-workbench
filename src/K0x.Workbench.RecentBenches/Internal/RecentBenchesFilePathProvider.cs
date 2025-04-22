using K0x.Workbench.RecentBenches.Abstractions;
using Microsoft.Extensions.Configuration;

namespace K0x.Workbench.RecentBenches.Internal;

public class RecentBenchesFilePathProvider : IRecentBenchesFilePathProvider
{
    private readonly IConfiguration _configuration;
    private readonly IAppExeFolderPathProvider _appExeFolderPathProvider;

    public RecentBenchesFilePathProvider(
        IAppExeFolderPathProvider appExeFolderPathProvider,
        IConfiguration configuration
    )
    {
        _appExeFolderPathProvider = appExeFolderPathProvider;
        _configuration = configuration;
    }

    public string GetFilePath()
    {
        string folderPath = _appExeFolderPathProvider.GetAppExeFolderPath();
        string filePath = Path.Combine(folderPath, RecentBenchesConstants.DefaultFileName);

        return filePath;
    }
}
