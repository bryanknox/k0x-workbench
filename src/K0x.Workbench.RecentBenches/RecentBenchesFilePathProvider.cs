using K0x.Workbench.RecentBenches.Abstractions;
using Microsoft.Extensions.Configuration;

namespace K0x.Workbench.RecentBenches;

public class RecentBenchesFilePathProvider : IRecentBenchesFilePathProvider
{
    private readonly IConfiguration _configuration;

    public RecentBenchesFilePathProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetFilePathOrThrow()
    {
        string? filePath = _configuration["RecentBenchesFilePath"];

        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new InvalidOperationException("RecentBenchesFilePath is not set in appsettings.json.");
        }
        return filePath;
    }
}
