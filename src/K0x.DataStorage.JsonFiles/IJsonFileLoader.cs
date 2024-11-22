namespace K0x.DataStorage.JsonFiles
{
    public interface IJsonFileLoader<T>
    {
        Task<T> LoadAsync(string jsonFilePath);
    }
}
