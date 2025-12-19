namespace K0x.Workbench.DataStorage.Abstractions.Models;

public record Kit
{
    public required string Label { get; init; }
    public IList<Tool> Tools { get; init; } = new List<Tool>();
    public IList<Kit> Kits { get; init; } = new List<Kit>();
}
