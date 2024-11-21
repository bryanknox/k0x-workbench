namespace K0x.DataStorage.JsonFiles;

public interface IJsonFileService<T>
{
    Task<T> LoadAsync(string filePath);
    Task SaveAsync(T data, string filePath);
}
