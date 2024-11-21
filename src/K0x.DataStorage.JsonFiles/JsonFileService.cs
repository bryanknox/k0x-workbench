using System.Text.Json;

namespace K0x.DataStorage.JsonFiles;

public class JsonFileService<T> : IJsonFileService<T>
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Performance",
        "CA1822:Mark members as static",
        Justification = "Instances of this class are used in dependency injection.")]
    public async Task<T> LoadAsync(string filePath)
    {
        using var sr = File.OpenText(filePath);

        string json = await sr.ReadToEndAsync();

        T? data = JsonSerializer.Deserialize<T>(json);

        if (data is null)
        {
            throw new InvalidDataException($"File contains no data. FilePath: {filePath}");
        }

        return data;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Performance",
        "CA1822:Mark members as static",
        Justification = "Instances of this class are used in dependency injection.")]
    public async Task SaveAsync(T data, string filePath)
    {
        string json = JsonSerializer.Serialize(data);

        using var sw = new StreamWriter(filePath);

        await sw.WriteAsync(json);
    }
}
