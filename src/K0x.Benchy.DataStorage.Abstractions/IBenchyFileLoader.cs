using K0x.Benchy.DataStorage.Abstractions.Models;

namespace K0x.Benchy.DataStorage.Abstractions
{
    public interface IBenchyFileLoader
    {
        Task<Bench> LoadBenchAsync(string jsonFilePath);
    }
}
