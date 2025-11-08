using K0x.Workbench.DataStorage.Abstractions;

namespace K0x.Workbench.DataStorage.JsonFiles;

public class BenchFilePathProvider : IBenchFilePathProvider
{
    private readonly object _lock = new();
    private string? _filePath;

    public string? FilePath
    {
        get
        {
            lock (_lock)
            {
                return _filePath;
            }
        }
        private set
        {
            lock (_lock)
            {
                _filePath = value;
            }
        }
    }

    public void SetFilePath(string? filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            FilePath = null;
            return;
        }

        var trimmedPath = TrimAndRemoveQuotes(filePath);
        var validatedPath = ValidateAndNormalizePath(trimmedPath);

        FilePath = validatedPath;
    }

    private static string TrimAndRemoveQuotes(string filePath)
    {
        var workingFilePath = filePath.Trim();

        // Check for and remove surrounding quotes.
        if (workingFilePath.StartsWith('"') && workingFilePath.EndsWith('"'))
        {
            workingFilePath = workingFilePath[1..^1].Trim();
        }

        return workingFilePath;
    }

    private static string ValidateAndNormalizePath(string filePath)
    {
        // Make sure we have an absolute path.
        return System.IO.Path.GetFullPath(filePath);
    }
}
