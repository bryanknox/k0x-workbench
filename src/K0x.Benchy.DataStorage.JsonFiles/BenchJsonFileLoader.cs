using K0x.Benchy.DataStorage.Abstractions;
using K0x.Benchy.DataStorage.Abstractions.Models;
using K0x.DataStorage.JsonFiles;

namespace K0x.Benchy.DataStorage.JsonFiles;

public class BenchJsonFileLoader : IBenchFileLoader
{
    private readonly IJsonFileLoader<BenchJsonFileModel> _jsonFileLoader;

    private readonly string _filePath;

    public BenchJsonFileLoader(
        IJsonFileLoader<BenchJsonFileModel> jsonFileLoader,
        string filePath)
    {
        _jsonFileLoader = jsonFileLoader;
        _filePath = filePath;
    }

    public async Task<Bench> LoadAsync()
    {
        var benchJsonFileModel = await _jsonFileLoader.LoadAsync(_filePath);

        return benchJsonFileModel.Bench;
    }
}
