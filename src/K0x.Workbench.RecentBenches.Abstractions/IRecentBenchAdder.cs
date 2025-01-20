namespace K0x.Workbench.RecentBenches.Abstractions;

public interface IRecentBenchAdder
{
    Task AddRecentBenchAsync(string filePath, string benchLabel);
}
