namespace K0x.Benchy.DataStorage.Abstractions.Models;

public record Tool
{
    public string Label { get; init; } = string.Empty;
    public required string Command { get; init; }
    public string? WorkingDirectory { get; init; } = null;
}
