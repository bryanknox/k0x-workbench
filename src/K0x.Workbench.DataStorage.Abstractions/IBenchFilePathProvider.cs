namespace K0x.Workbench.DataStorage.Abstractions;

public interface IBenchFilePathProvider
{
    string? FilePath { get; }

    void SetFilePath(string? filePath);
}
