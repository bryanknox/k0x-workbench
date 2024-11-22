using System.Text.Json;

namespace K0x.DataStorage.JsonFiles;

public class JsonFileService<T> : IJsonFileService<T>
{
    /// <summary>
    /// Loads data of type <typeparamref name="T"/> from the specified JSON file.
    /// </summary>
    /// <param name="filePath">
    /// The path to the file to load data from.
    /// </param>
    /// <returns>
    /// A new instance of <typeparamref name="T"/> object populated with data from the file.
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
