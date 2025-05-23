﻿using K0x.Workbench.Files.Abstractions;

namespace WpfBlazor.InternalServices;

public class LocalAppDataFolderPathProvider : ILocalAppDataFolderPathProvider
{
    public string GetLocalAppDataFolderPath()
    {
        // Get the path to the local app data folder
        string localAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        // Combine it with the application name
        string appLocalAppDataFolder = System.IO.Path.Combine(localAppDataFolder, "K0xWorkbench");

        // Check if the folder exists, if not create it
        if (!System.IO.Directory.Exists(appLocalAppDataFolder))
        {
            System.IO.Directory.CreateDirectory(appLocalAppDataFolder);
        }
        return appLocalAppDataFolder;
    }
}
