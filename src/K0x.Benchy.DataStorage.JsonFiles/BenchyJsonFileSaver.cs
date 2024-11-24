using K0x.Benchy.DataStorage.Abstractions;
using K0x.Benchy.DataStorage.Abstractions.Models;
using K0x.DataStorage.JsonFiles;

namespace K0x.Benchy.DataStorage.JsonFiles
{
    public class BenchyJsonFileSaver : IBenchyFileSaver
    {
        private readonly IJsonFileSaver<BenchyJsonFileModel> _jsonFileSaver;

        public BenchyJsonFileSaver(IJsonFileSaver<BenchyJsonFileModel> jsonFileSaver)
        {
            _jsonFileSaver = jsonFileSaver;
        }

        public async Task SaveAsync(Bench bench, string filePath)
        {
            var fileModel = new BenchyJsonFileModel {
                Bench = bench
            };

            await _jsonFileSaver.SaveAsync(fileModel, filePath);
        }
    }
}
