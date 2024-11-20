﻿namespace K0x.Benchy.DataStorage.Abstractions.Models;

public record Tray
{
    public required string Label { get; init; }
    public IList<Tool> Tools { get; init; } = new List<Tool>();

}
