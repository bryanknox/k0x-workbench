using Microsoft.Extensions.Configuration;
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
    internal static void Configure(IConfiguration configuration)
    {
        string logDirectoryPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "logs");
        Directory.CreateDirectory(logDirectoryPath);

        string logFilePath = Path.Combine(
            logDirectoryPath,
            $"log_{DateTime.Now:yyyyMMdd_HHmmss}.log");

        // Get the Logging.LogLevel.Default value from the configuration
        var logLevel = configuration.GetValue<string>("Logging:LogLevel:Default");
        var serilogLogLevel = logLevel switch
        {
            "Trace" => LogEventLevel.Verbose,
            "Debug" => LogEventLevel.Debug,
            "Information" => LogEventLevel.Information,
            "Warning" => LogEventLevel.Warning,
            "Error" => LogEventLevel.Error,
            "Critical" => LogEventLevel.Fatal,
            _ => LogEventLevel.Information
        };

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .MinimumLevel.Is(serilogLogLevel)
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
