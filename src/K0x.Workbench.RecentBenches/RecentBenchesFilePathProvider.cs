using K0x.Workbench.RecentBenches.Abstractions;
using Microsoft.Extensions.Configuration;

namespace K0x.Workbench.RecentBenches;

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
        string? filePath = _configuration[RecentBenchesConstants.FilePathSettingName];

        if (string.IsNullOrWhiteSpace(filePath))
        {
            string folderPath = _appExeFolderPathProvider.GetAppExeFolderPath();
            filePath = Path.Combine(folderPath, RecentBenchesConstants.DefaultFileName);
        }
        return filePath;
    }
}
