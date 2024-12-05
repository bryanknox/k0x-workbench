using K0x.Benchy.DataStorage.Abstractions;
using K0x.Benchy.DataStorage.Abstractions.Models;

namespace K0x.Benchy.DataStorage.JsonFiles;

public class BenchProvider : IBenchProvider
{
    public Bench Bench { get; set; } = default!;
}
