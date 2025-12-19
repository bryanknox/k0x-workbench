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
        var options = new JsonSerializerOptions
        {
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
        };

        string json = JsonSerializer.Serialize(data, options);

        // Overwrite the file if it exists.
        using var sw = new StreamWriter(filePath, append: false);

        await sw.WriteAsync(json);
    }
}
