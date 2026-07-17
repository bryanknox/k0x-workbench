using K0x.Workbench.DataStorage.Abstractions.Models;

namespace K0x.Workbench.DataStorage.Abstractions
{
    public interface IBenchFileSaver
    {
        Task SaveAsync(Kit benchKit, string filePath);
    }
}
