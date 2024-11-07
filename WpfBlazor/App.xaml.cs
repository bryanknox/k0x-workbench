using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System.Windows;

namespace WpfBlazor;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public IConfiguration Configuration { get; }

    public App()
    {
        // Load configuration from appsettings.json in the directory
        // where the application is running.
        var builder = new ConfigurationBuilder()
                        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        Configuration = builder.Build();

        // Register a callback to be called after a change in the the configuration
        // (appsettings.json) has been detected and the configuration has been reloaded.
        ChangeToken.OnChange(() => Configuration.GetReloadToken(), OnConfigurationReloaded);
    }

    /// <summary>
    /// Event handler called after a change in the the configuration
    /// (appsettings.json) has been detected and the configuration has been reloaded.
    /// </summary>
    private void OnConfigurationReloaded()
    {
        MessageBox.Show("Configuration (appsettings.json) has been reloaded.");
    }
}
