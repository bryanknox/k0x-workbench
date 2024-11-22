using K0x.Benchy.DataStorage.Abstractions.Models;
using K0x.Benchy.DataStorage.Abstractions.Services;
using K0x.Benchy.DataStorage.JsonFiles.FileModels;
using K0x.DataStorage.JsonFiles;

namespace K0x.Benchy.DataStorage.JsonFiles.Services;

public class BenchyJsonFileDataLoader : IBenchyDataLoader
{
    private readonly IJsonFileLoader<BenchyJsonFileModel> _jsonFileLoader;

    public BenchyJsonFileDataLoader(IJsonFileLoader<BenchyJsonFileModel> jsonFileLoader)
    {
        _jsonFileLoader = jsonFileLoader;
    }

    public async Task<Bench> LoadBenchAsync(string jsonFilePath)
    {
        var benchyJsonFileModel = await _jsonFileLoader.LoadAsync(jsonFilePath);

        return benchyJsonFileModel.Bench;
    }
}
