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

        var trimmedPath = TrimAndRemoveQuotes(filePath);

        if (trimmedPath is null)
        {
            FilePath = null;
            return;
        }

        var validatedPath = ValidateAndNormalizePath(trimmedPath);

        FilePath = validatedPath;
    }

    private static string? TrimAndRemoveQuotes(string filePath)
    {
        var workingFilePath = filePath.Trim();

        // Remove surrounding double quotes.
        workingFilePath = workingFilePath.Trim('"');

        // Remove surrounding single quotes.
        workingFilePath = workingFilePath.Trim('\'');

        // Trim any remaining whitespace.
        workingFilePath = workingFilePath.Trim();

        // Return null if the path is empty after trimming and removing quotes.
        return string.IsNullOrWhiteSpace(workingFilePath) ? null : workingFilePath;
    }

    private static string ValidateAndNormalizePath(string filePath)
    {
        // Check for invalid characters before attempting to get full path.
        var invalidChars = System.IO.Path.GetInvalidPathChars();
        if (filePath.IndexOfAny(invalidChars) >= 0)
        {
            throw new ArgumentException("File path contains invalid characters.", nameof(filePath));
        }

        // Check path length before calling GetFullPath to provide better error message.
        // Windows typically has a MAX_PATH of 260 characters, but long path support may be enabled.
        if (filePath.Length > 32767) // Maximum path length in Windows with long path support
        {
            throw new PathTooLongException($"File path is too long. Maximum length is 32767 characters, but path is {filePath.Length} characters.");
        }

        try
        {
            // Make sure we have an absolute path.
            var fullPath = System.IO.Path.GetFullPath(filePath);

            // Additional validation: check the resulting full path length.
            if (fullPath.Length > 32767)
            {
                throw new PathTooLongException($"Resolved file path is too long. Maximum length is 32767 characters, but path is {fullPath.Length} characters.");
            }

            return fullPath;
        }
        catch (ArgumentException ex)
        {
            throw new ArgumentException($"Invalid file path: {filePath}", nameof(filePath), ex);
        }
        catch (NotSupportedException ex)
        {
            throw new ArgumentException($"File path format is not supported: {filePath}", nameof(filePath), ex);
        }
        catch (PathTooLongException)
        {
            // Re-throw PathTooLongException as-is
            throw;
        }
    }
}
