using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WpfBlazor;

internal static class ProgramConfiguration
{
    internal static IServiceProvider Setup()
    {
        IHostBuilder hostBuilder = Host.CreateDefaultBuilder();

        LoggingConfiguration.UseLogger(hostBuilder);

        hostBuilder.ConfigureAppConfiguration((context, config) =>
        {
            AppSettingsConfiguration.SetupForJsonFile(config);
        });

        hostBuilder.ConfigureServices((context, services) =>
        {
            // NOTE: IConfiguration is registered above by CreateDefaultBuilder().

            services.AddWpfBlazorWebView();
        });

        hostBuilder.ConfigureLogging((context, logging) =>
        {
            LoggingConfiguration.Configure(context.Configuration);
        });

        IHost host = hostBuilder.Build();

        IServiceProvider serviceProvider = host.Services;

        AppSettingsConfiguration.SetupOnChangeHandler(serviceProvider);

        return serviceProvider;
    }
}

