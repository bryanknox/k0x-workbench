using K0x.Workbench.DataStorage.JsonFiles;
using K0x.Workbench.RecentBenches;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using System;
using System.Windows;
using WpfBlazor.InternalServices;

namespace WpfBlazor;

internal static class ProgramConfiguration
{
    internal static IServiceProvider Setup()
    {
        IHostBuilder hostBuilder = Host.CreateDefaultBuilder();

        hostBuilder.ConfigureAppConfiguration((context, config) =>
        {
            // NOTE: Host.CreateDefaultBuilder() already adds appsettings.json and environment variables.
            // We need to add our custom prefix for environment variables AFTER everything else
            // to ensure they can override the appsettings.json settings.
            config.AddEnvironmentVariables("K0xWorkbench_");
        });

        hostBuilder.ConfigureServices((context, services) =>
        {
            // NOTE: IConfiguration is registered above by CreateDefaultBuilder().

            services.AddBenchJsonFiles();

            services.AddAppTitleService();

            services.AddDataFolderPathProvider();

            services.AddRecentBenchesJsonFiles();

            services.AddWpfBlazorWebView();

            services.AddTransient<MainWindow>();
        });

        LoggingConfiguration.ConfigureSerilog(hostBuilder);

        IHost host = hostBuilder.Build();

        IServiceProvider serviceProvider = host.Services;

        SetupOnChangeHandler(serviceProvider);

        return serviceProvider;
    }

    // Called after the host (and IConfiguration) have been built.
    //
    // Subscribes to IConfiguration reload notifications. Any reloadable configuration provider
    // (e.g., appsettings.json or additional JSON files you may add later) that detects a change
    // and reloads will trigger OnConfigurationReloaded. Environment variables do not trigger
    // reloads.
    //
    // ChangeToken.OnChange automatically re-subscribes after each reload because a factory
    // function is provided for the reload token.
    private static void SetupOnChangeHandler(IServiceProvider serviceProvider)
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        ChangeToken.OnChange(() => configuration.GetReloadToken(), OnConfigurationReloaded);
    }

    private static void OnConfigurationReloaded()
    {
        static void ShowMessage()
        {
            MessageBox.Show("Configuration has been reloaded.");
        }

        var app = Application.Current;

        // If there's no current Application (e.g., during shutdown), fall back to direct call.
        if (app is null || app.Dispatcher is null)
        {
            ShowMessage();
            return;
        }

        if (app.Dispatcher.CheckAccess())
        {
            ShowMessage();
        }
        else
        {
            // Marshal back to UI thread to avoid cross-thread access issues.
            app.Dispatcher.BeginInvoke((Action)ShowMessage);
        }
    }
}

