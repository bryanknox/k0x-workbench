using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System.IO;

namespace WpfBlazor;

/// <remarks>
/// Uses Serilog. See:
/// https://serilog.net/
/// https://github.com/serilog/serilog
/// </remarks>
internal static class LoggingConfiguration
{
    internal static void Configure(IHostBuilder hostBuilder)
    {
        string logDirectoryPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "logs");
        Directory.CreateDirectory(logDirectoryPath);

        string logFilePath = Path.Combine(
            logDirectoryPath,
            $"log_{DateTime.Now:yyyyMMdd_HHmmss}.log");

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(
                logFilePath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 10)
            .CreateLogger();

        hostBuilder.UseSerilog();

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
}
