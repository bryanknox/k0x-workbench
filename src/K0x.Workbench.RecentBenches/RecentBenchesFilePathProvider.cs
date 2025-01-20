using K0x.Workbench.RecentBenches.Abstractions;

namespace K0x.Workbench.RecentBenches;

public class RecentBenchesFilePathProvider : IRecentBenchesFilePathProvider
{
    public Task<string> GetFilePathAsync()
    {
        // TODO: Load the file path from the configuration

        throw new NotImplementedException();
    }
}
