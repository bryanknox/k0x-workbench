using K0x.Workbench.DataStorage.Abstractions;

namespace K0x.Workbench.DataStorage.JsonFiles;

public class BenchFilePathProvider : IBenchFilePathProvider
{
    public string FilePath { get; set; } = string.Empty;
}
