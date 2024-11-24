namespace K0x.Benchy.DataStorage.Abstractions.Models;

public record Bench
{
    public required string Label { get; init; }
    public IList<Tray> Trays { get; init; } = new List<Tray>();
}
