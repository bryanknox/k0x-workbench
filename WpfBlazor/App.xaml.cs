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
        Configuration = InitializeConfiguration();
    }

    private IConfigurationRoot InitializeConfiguration()
    {
        try
        {
            // Load configuration from the required appsettings.json file in the directory
            // where the application is running.
            var builder = new ConfigurationBuilder()
                            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            // Register a callback to be called after a change in the the configuration
            // (appsettings.json) has been detected and the configuration has been reloaded.
            ChangeToken.OnChange(() => configuration.GetReloadToken(), OnConfigurationReloaded);

            return configuration;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"An error occurred:\n\n{ex.GetType()}\n\n{ex.Message}\n\nThe application cannot be strated.",
                "Error Configuring the App",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            // Rethrow the exception to terminate the application.
            throw;
        }
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
