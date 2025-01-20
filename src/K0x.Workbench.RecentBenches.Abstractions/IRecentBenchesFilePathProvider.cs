namespace K0x.Workbench.RecentBenches.Abstractions;

public interface IRecentBenchesFilePathProvider
{
    Task<string> GetFilePathAsync();
}
