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
    internal static void Configure(IConfiguration configuration)
    {
        string logDirectoryPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "logs");
        Directory.CreateDirectory(logDirectoryPath);

        string logFilePath = Path.Combine(
            logDirectoryPath,
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
        var logFiles = Directory.GetFiles(logDirectoryPath, "*.log");
        foreach (var logFile in logFiles)
        {
            if (File.GetCreationTime(logFile) < expireDate)
            {
                File.Delete(logFile);
            }
        }
    }

    internal static void UseLogger(IHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog();
    }
}
