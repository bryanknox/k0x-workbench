using K0x.Benchy.DataStorage.Abstractions.Models;

namespace K0x.Benchy.DataStorage.JsonFiles.FileModels;

public record BenchyJsonFileModel
{
    public required Bench Bench { get; init; }
}
