using K0x.Workbench.DataStorage.Abstractions;
using K0x.Workbench.DataStorage.Abstractions.Models;

namespace K0x.Workbench.DataStorage.JsonFiles;

public class BenchProvider : IBenchProvider
{
    public Bench Bench { get; set; } = default!;
}
