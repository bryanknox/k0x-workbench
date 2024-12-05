namespace K0x.Workbench.DataStorage.Abstractions.Models;

public record Tray
{
    public required string Label { get; init; }
    public IList<Tool> Tools { get; init; } = new List<Tool>();

}
