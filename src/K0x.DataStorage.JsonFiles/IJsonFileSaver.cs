namespace K0x.DataStorage.JsonFiles;

public interface IJsonFileSaver<T>
{
    Task SaveAsync(T data, string filePath);
}
