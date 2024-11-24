using System.Text.Json;

namespace K0x.DataStorage.JsonFiles;

public class JsonFileSaver<T> : IJsonFileSaver<T>
{
    /// <summary>
    /// Saves the specified data of type <typeparamref name="T"/> to the specified JSON file.
    /// Overwrites the file if it already exists.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="filePath"></param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Performance",
        "CA1822:Mark members as static",
        Justification = "Instances of this class are used in dependency injection.")]
    public async Task SaveAsync(T data, string filePath)
    {
        string json = JsonSerializer.Serialize(data);

        // Overwrite the file if it exists.
        using var sw = new StreamWriter(filePath, append: false);

        await sw.WriteAsync(json);
    }
}
