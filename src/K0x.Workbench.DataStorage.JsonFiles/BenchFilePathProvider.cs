using K0x.Workbench.DataStorage.Abstractions;

namespace K0x.Workbench.DataStorage.JsonFiles;

public class BenchFilePathProvider : IBenchFilePathProvider
{
    public string? FilePath { get; private set; }

    public void SetFilePath(string? filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            FilePath = null;
            return;
        }

        var workingFilePath = filePath.Trim();

        // Check for and remove surrounding quotes.
        if (workingFilePath.StartsWith('"') && workingFilePath.EndsWith('"'))
        {
            workingFilePath = workingFilePath[1..^1].Trim();
        }

        // Make sure we have an absolute path.
        workingFilePath = System.IO.Path.GetFullPath(workingFilePath);

        FilePath = workingFilePath;
    }
}
