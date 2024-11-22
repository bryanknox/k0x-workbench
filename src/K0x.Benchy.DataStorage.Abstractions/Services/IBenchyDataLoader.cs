﻿using K0x.Benchy.DataStorage.Abstractions.Models;

namespace K0x.Benchy.DataStorage.Abstractions.Services
{
    public interface IBenchyDataLoader
    {
        Task<Bench> LoadBenchAsync(string jsonFilePath);
    }
}
