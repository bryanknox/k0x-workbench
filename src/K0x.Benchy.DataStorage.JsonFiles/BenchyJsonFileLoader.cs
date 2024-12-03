using K0x.Benchy.DataStorage.Abstractions;
using K0x.Benchy.DataStorage.Abstractions.Models;
using K0x.DataStorage.JsonFiles;

namespace K0x.Benchy.DataStorage.JsonFiles;

public class BenchyJsonFileLoader : IBenchyFileLoader
{
    private readonly IJsonFileLoader<BenchyJsonFileModel> _jsonFileLoader;

    private readonly string _filePath;

    public BenchyJsonFileLoader(
        IJsonFileLoader<BenchyJsonFileModel> jsonFileLoader,
        string filePath)
    {
        _jsonFileLoader = jsonFileLoader;
        _filePath = filePath;
    }

    public async Task<Bench> LoadAsync()
    {
        var benchyJsonFileModel = await _jsonFileLoader.LoadAsync(_filePath);

        return benchyJsonFileModel.Bench;
    }
}
