using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Linq;

namespace WpfBlazor;

internal static class LoggingConfiguration
{
    internal static void Configure(IHostBuilder hostBuilder)
    {
        string logDirectory = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "logs");
        Directory.CreateDirectory(logDirectory);

        string logFilePath = Path.Combine(
            logDirectory,
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
        var logFiles = Directory.GetFiles(logDirectory, "*.log");
        foreach (var logFile in logFiles)
        {
            if (File.GetCreationTime(logFile) < DateTime.Now.AddDays(-5))
            {
                File.Delete(logFile);
            }
        }
    }
}
