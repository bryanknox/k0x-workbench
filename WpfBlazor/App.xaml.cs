using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System.Windows;

namespace WpfBlazor;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private IConfiguration Configuration { get; }

    public IServiceProvider ServiceProvider { get; }

    public App()
    {
        try
        {
            var host = CreateHostBuilder().Build();
            ServiceProvider = host.Services;
            Configuration = ServiceProvider.GetRequiredService<IConfiguration>();
            RegisterConfigurationChangeReloadHandler();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"An error occurred:\n\n{ex.GetType()}\n\n{ex.Message}\n\nThe application cannot be started.",
                "Error Configuring the App",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            // Rethrow the exception to terminate the application.
            throw;
        }
    }

    private IHostBuilder CreateHostBuilder()
    {
        IHostBuilder hostBuilder = Host.CreateDefaultBuilder();

        hostBuilder.ConfigureAppConfiguration((context, config) =>
        {
            config.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                  .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        });

        hostBuilder.ConfigureServices((context, services) =>
        {
            // NOTE: IConfiguration is registered above by CreateDefaultBuilder().

            services.AddWpfBlazorWebView();
        });

        return hostBuilder;
    }

    private void RegisterConfigurationChangeReloadHandler()
    {
        // Register a callback to be called after a change in the configuration
        // (appsettings.json) has been detected and the configuration has been reloaded.
        ChangeToken.OnChange(() => Configuration.GetReloadToken(), OnConfigurationReloaded);
    }

    private void OnConfigurationReloaded()
    {
        MessageBox.Show("Configuration (appsettings.json) has been reloaded.");
    }
}
