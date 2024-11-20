using K0x.Benchy.DataStorage.Abstractions.Models;
using K0x.Benchy.DataStorage.Abstractions.Services;

namespace K0x.Benchy.DataStorage.JsonFiles.Services;

public class BenchyJsonFileDataLoader : IBenchyDataLoader
{
    public Task<IList<Bench>> LoadBenchesAsync()
    {
        throw new NotImplementedException();
    }
}
