using K0x.Benchy.DataStorage.Abstractions.Models;
using K0x.Benchy.DataStorage.Abstractions.Services;
using K0x.Benchy.DataStorage.JsonFiles.FileModels;
using K0x.DataStorage.JsonFiles;

namespace K0x.Benchy.DataStorage.JsonFiles.Services;

public class BenchyJsonFileDataLoader : IBenchyDataLoader
{
    private string _jsonFilePath = null!;

    private readonly IJsonFileService<BenchyJsonFileModel> _jsonFileService;

    public BenchyJsonFileDataLoader(IJsonFileService<BenchyJsonFileModel> jsonFileService)
    {
        _jsonFileService = jsonFileService;
    }

    public void Initialize(string jsonFilePath)
    {
        _jsonFilePath = jsonFilePath;
    }

    public async Task<Bench> LoadBenchAsync()
    {
        ThrowIfJsonFilePathNotInitialized();

        var benchyJsonFileModel = await _jsonFileService.LoadAsync(_jsonFilePath);

        return benchyJsonFileModel.Bench;
    }

    private void ThrowIfJsonFilePathNotInitialized()
    {
        if (string.IsNullOrWhiteSpace(_jsonFilePath))
        {
            throw new InvalidOperationException(
                "Can't access Benchy file. Json file path was not properly initialized."
                + "Program logic error."
                + $" {nameof(BenchyJsonFileDataLoader)}.Initialize(..) was not called.");
        }
    }
}
