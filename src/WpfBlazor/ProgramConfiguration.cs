using K0x.Workbench.DataStorage.JsonFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WpfBlazor.InternalServices;

namespace WpfBlazor;

internal static class ProgramConfiguration
{
    internal static IServiceProvider Setup()
    {
        IHostBuilder hostBuilder = Host.CreateDefaultBuilder();

        hostBuilder.ConfigureAppConfiguration((context, config) =>
        {
            AppSettingsConfiguration.SetupForJsonFile(config);
        });

        hostBuilder.ConfigureServices((context, services) =>
        {
            // NOTE: IConfiguration is registered above by CreateDefaultBuilder().

            services.AddBenchJsonFiles();

            services.AddAppTitleService();

            services.AddWpfBlazorWebView();

            services.AddTransient<MainWindow>();
        });

        LoggingConfiguration.ConfigureSerilog(hostBuilder);

        IHost host = hostBuilder.Build();

        IServiceProvider serviceProvider = host.Services;

        AppSettingsConfiguration.SetupOnChangeHandler(serviceProvider);

        return serviceProvider;
    }
}

