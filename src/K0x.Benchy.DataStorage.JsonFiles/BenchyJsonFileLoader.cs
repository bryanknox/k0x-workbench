﻿using K0x.Benchy.DataStorage.Abstractions;
using K0x.Benchy.DataStorage.Abstractions.Models;
using K0x.DataStorage.JsonFiles;

namespace K0x.Benchy.DataStorage.JsonFiles;

public class BenchyJsonFileLoader : IBenchyFileLoader
{
    private readonly IJsonFileLoader<BenchyJsonFileModel> _jsonFileLoader;

    public BenchyJsonFileLoader(IJsonFileLoader<BenchyJsonFileModel> jsonFileLoader)
    {
        _jsonFileLoader = jsonFileLoader;
    }

    public async Task<Bench> LoadAsync(string filePath)
    {
        var benchyJsonFileModel = await _jsonFileLoader.LoadAsync(filePath);

        return benchyJsonFileModel.Bench;
    }
}
