namespace K0x.Workbench.RecentBenches.Abstractions.Models;

public record RecentBench
{
    public required string BenchLabel { get; init; }
    public required string FilePath { get; init; }
    public required DateTimeOffset LastOpened { get; init; }
}
