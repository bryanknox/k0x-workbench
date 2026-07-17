using K0x.Workbench.DataStorage.Abstractions.Models;

namespace K0x.Workbench.DataStorage.JsonFiles;

public record BenchJsonFileModel
{
    public required Kit Bench { get; init; }
}
