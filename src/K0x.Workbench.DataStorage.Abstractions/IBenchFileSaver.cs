using K0x.Workbench.DataStorage.Abstractions.Models;

namespace K0x.Workbench.DataStorage.Abstractions
{
    public interface IBenchFileSaver
    {
        Task SaveAsync(Bench bench, string filePath);
    }
}
