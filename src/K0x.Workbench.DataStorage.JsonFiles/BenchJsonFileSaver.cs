using K0x.DataStorage.JsonFiles;
using K0x.Workbench.DataStorage.Abstractions;
using K0x.Workbench.DataStorage.Abstractions.Models;

namespace K0x.Workbench.DataStorage.JsonFiles
{
    public class BenchJsonFileSaver : IBenchFileSaver
    {
        private readonly IJsonFileSaver<BenchJsonFileModel> _jsonFileSaver;

        public BenchJsonFileSaver(IJsonFileSaver<BenchJsonFileModel> jsonFileSaver)
        {
            _jsonFileSaver = jsonFileSaver;
        }

        public async Task SaveAsync(Kit benchKit, string filePath)
        {
            var fileModel = new BenchJsonFileModel
            {
                Bench = benchKit
            };

            await _jsonFileSaver.SaveAsync(fileModel, filePath);
        }
    }
}
