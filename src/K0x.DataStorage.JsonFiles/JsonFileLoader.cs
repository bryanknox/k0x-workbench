using System.Text.Json;

namespace K0x.DataStorage.JsonFiles;

public class JsonFileLoader<T> : IJsonFileLoader<T>
{
    /// <summary>
    /// Loads data of type <typeparamref name="T"/> from the specified JSON file.
    /// </summary>
    /// <param name="filePath">
    /// The path to the file to load data from.
    /// </param>
    /// <returns>
    /// A new instance of <typeparamref name="T"/> object populated with data read from the file.
    /// </returns>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="JsonException"></exception>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Performance",
        "CA1822:Mark members as static",
        Justification = "Instances of this class are used in dependency injection.")]
    public async Task<T> LoadAsync(string filePath)
    {
        using var sr = File.OpenText(filePath);

        string json = await sr.ReadToEndAsync();

        T? data = JsonSerializer.Deserialize<T>(json);

        // data is not null because an exception will be thrown if the JSON is invalid.
        return data!;
    }
}
