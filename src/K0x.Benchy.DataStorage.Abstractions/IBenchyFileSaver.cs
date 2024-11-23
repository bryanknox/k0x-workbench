using K0x.Benchy.DataStorage.Abstractions.Models;

namespace K0x.Benchy.DataStorage.Abstractions
{
    public interface IBenchyFileSaver
    {
        Task SaveAsync(Bench bench, string filePath);
    }
}
