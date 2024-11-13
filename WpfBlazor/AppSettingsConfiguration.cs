using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System.Windows;

namespace WpfBlazor
{
    internal static class AppSettingsConfiguration
    {
        // This method is called before the configuration is built (hostBuilder.Build()).
        internal static void SetupForJsonFile(IConfigurationBuilder config)
        {
            // Setup to use the appsettings.json file.
            // And reload the configuration when the file changes.
            // See class AppSettingsChangeHandler.
            config.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                  .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        }

        // This method is called after the configuration has been reloaded (hostBuilder.Build()).
        internal static void SetupOnChangeHandler(IServiceProvider serviceProvider)
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            // Register a callback to be called after a change in the configuration
            // (appsettings.json) has been detected and the configuration has been reloaded.
            ChangeToken.OnChange(() => configuration.GetReloadToken(), OnConfigurationReloaded);
        }

        private static void OnConfigurationReloaded()
        {
            MessageBox.Show("Configuration (appsettings.json) has been reloaded.");
        }
    }
}
