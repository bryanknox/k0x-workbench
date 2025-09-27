using K0x.Workbench.Files.Abstractions;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace WpfBlazor.InternalServices;

public class DataFolderPathProvider : IDataFolderPathProvider
{
    private readonly IConfiguration _configuration;

    public DataFolderPathProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetDataFolderPath()
    {
        string? dataFolderPath = _configuration["DataFolderPath"];

        if (string.IsNullOrWhiteSpace(dataFolderPath))
        {
            // Get the path to the user profile folder.
            string baseFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            dataFolderPath = System.IO.Path.Combine(baseFolderPath, ".k0xworkbench");
        }

        Directory.CreateDirectory(dataFolderPath);

        return dataFolderPath;
    }
}
