using K0x.Workbench.Files.Abstractions;

namespace WpfBlazor.InternalServices;

public class DataFolderPathProvider : IDataFolderPathProvider
{
    public string GetDataFolderPath()
    {
        // Get the path to the local app data folder
        string baseFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        // Combine it with the application name
        string dataFolderPath = System.IO.Path.Combine(baseFolderPath, ".k0xworkbench");

        // Check if the folder exists, if not create it
        if (!System.IO.Directory.Exists(dataFolderPath))
        {
            System.IO.Directory.CreateDirectory(dataFolderPath);
        }
        return dataFolderPath;
    }
}
