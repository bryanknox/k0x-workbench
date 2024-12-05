using K0x.Benchy.DataStorage.Abstractions.Models;

namespace K0x.Benchy.DataStorage.JsonFiles;

public record BenchJsonFileModel
{
    public required Bench Bench { get; init; }
}
