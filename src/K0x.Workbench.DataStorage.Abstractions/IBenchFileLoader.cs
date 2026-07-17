using K0x.Workbench.DataStorage.Abstractions.Models;

namespace K0x.Workbench.DataStorage.Abstractions
{
    public interface IBenchFileLoader
    {
        Task<Kit> LoadAsync(string filePath);
    }
}
