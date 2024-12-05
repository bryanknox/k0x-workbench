using K0x.Benchy.DataStorage.Abstractions;
using K0x.Benchy.DataStorage.Abstractions.Models;
using K0x.DataStorage.JsonFiles;

namespace K0x.Benchy.DataStorage.JsonFiles
{
    public class BenchJsonFileSaver : IBenchFileSaver
    {
        private readonly IJsonFileSaver<BenchJsonFileModel> _jsonFileSaver;

        public BenchJsonFileSaver(IJsonFileSaver<BenchJsonFileModel> jsonFileSaver)
        {
            _jsonFileSaver = jsonFileSaver;
        }

        public async Task SaveAsync(Bench bench, string filePath)
        {
            var fileModel = new BenchJsonFileModel
            {
                Bench = bench
            };

            await _jsonFileSaver.SaveAsync(fileModel, filePath);
        }
    }
}
