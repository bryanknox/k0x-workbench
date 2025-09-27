using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.IO;

namespace WpfBlazor;

/// <remarks>
/// Uses Serilog. See:
/// https://serilog.net/
/// https://github.com/serilog/serilog
/// </remarks>
internal static class LoggingConfiguration
{
    internal static void ConfigureSerilog(IHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog();

        hostBuilder.ConfigureLogging((context, logging) =>
        {
            Configure(context.Configuration);
        });
    }

    private static void Configure(IConfiguration configuration)
    {
        string logsFolderPath = GetLogsFolderPath(configuration);

        Directory.CreateDirectory(logsFolderPath);

        string logFilePath = Path.Combine(
            logsFolderPath,
            $"log_{DateTime.Now:yyyyMMdd_HHmmss}.log");

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .WriteTo.Console()
            .WriteTo.File(
                logFilePath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 10)
            .CreateLogger();

        // Delete log files older than 5 days
        var expireDate = DateTime.Now.AddDays(-5);
        var logFiles = Directory.GetFiles(logsFolderPath, "*.log");
        foreach (var logFile in logFiles)
        {
            if (File.GetCreationTime(logFile) < expireDate)
            {
                File.Delete(logFile);
            }
        }
    }

    private static string GetLogsFolderPath(IConfiguration configuration)
    {
        string? logsFolderPath = configuration["LogsFolderPath"];

        if (string.IsNullOrWhiteSpace(logsFolderPath))
        {
            // Default to a folder under user's LocalApplicationData folder.

            string baseFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            logsFolderPath = Path.Combine(baseFolderPath, "K0xWorkbench", "logs");
        }

        // Create the folder if it does not exist.
        Directory.CreateDirectory(logsFolderPath);

        return logsFolderPath;
    }
}
