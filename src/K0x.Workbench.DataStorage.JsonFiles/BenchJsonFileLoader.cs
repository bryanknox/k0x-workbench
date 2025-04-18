using K0x.DataStorage.JsonFiles;
using K0x.Workbench.DataStorage.Abstractions;
using K0x.Workbench.DataStorage.Abstractions.Models;

namespace K0x.Workbench.DataStorage.JsonFiles;

public class BenchJsonFileLoader : IBenchFileLoader
{
    private readonly IJsonFileLoader<BenchJsonFileModel> _jsonFileLoader;

    public BenchJsonFileLoader(IJsonFileLoader<BenchJsonFileModel> jsonFileLoader)
    {
        _jsonFileLoader = jsonFileLoader;
    }

    public async Task<Bench> LoadAsync(string filePath)
    {
        var benchJsonFileModel = await _jsonFileLoader.LoadAsync(filePath);

        return benchJsonFileModel.Bench;
    }
}
