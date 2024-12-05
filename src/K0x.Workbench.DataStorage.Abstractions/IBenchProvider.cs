using K0x.Workbench.DataStorage.Abstractions.Models;

namespace K0x.Workbench.DataStorage.Abstractions;

public interface IBenchProvider
{
    Bench Bench { get; set; }
}
