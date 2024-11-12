using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using System.Windows;

namespace WpfBlazor;

public static class DependencyInjection
{
    public static IServiceProvider Configure()
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

        IHost host = hostBuilder.Build();

        IServiceProvider serviceProvider = host.Services;

        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        RegisterConfigurationChangeReloadHandler(configuration);

        return serviceProvider;
    }

    private static void RegisterConfigurationChangeReloadHandler(IConfiguration configuration)
    {
        // Register a callback to be called after a change in the configuration
        // (appsettings.json) has been detected and the configuration has been reloaded.
        ChangeToken.OnChange(() => configuration.GetReloadToken(), OnConfigurationReloaded);
    }

    private static void OnConfigurationReloaded()
    {
        MessageBox.Show("Configuration (appsettings.json) has been reloaded.");
    }

}

