using K0x.Benchy.DataStorage.Abstractions.Models;

namespace K0x.Benchy.DataStorage.Abstractions;

public interface IBenchProvider
{
    Bench Bench { get; set; }
}
