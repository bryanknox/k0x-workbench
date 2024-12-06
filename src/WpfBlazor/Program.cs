using K0x.DataStorage.JsonFiles;
using K0x.Workbench.DataStorage.Abstractions;
using K0x.Workbench.DataStorage.Abstractions.Models;
using K0x.Workbench.DataStorage.JsonFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using System.Windows;

namespace WpfBlazor;

public class Program
{
    private static ILogger<Program> _logger = null!;

    // The Main method cannot be async for WPF applications.
    // So, we use synchronous methods and block on async methods.
    [STAThread]
    static void Main(string[] args)
    {
        IServiceProvider serviceProvider = SetupProgramConfigurationAndDI();

        // Get the logger for use within this class.
        _logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        InitBenchFilePathProviderFromArgs(serviceProvider, args);

        RunWpfApp(serviceProvider);

        _logger.LogInformation("Done.");
    }

    static IServiceProvider SetupProgramConfigurationAndDI()
    {
        try
        {
            return ProgramConfiguration.Setup();
        }
        catch (Exception ex)
        {
            // NOTE: We can't use the ILogger here because the configuration failed
            // to load, and the logger may not yet be configured.
            // So, we use a MessageBox to show the error message.

            MessageBox.Show(
                "An error occurred setting up and configuring the app.\n"
                + "The application cannot be started.\n"
                + "\n"
                + $"{ex.GetType()}\n"
                + "\n"
                + $"{ex.Message}\n",
                caption: "Error Configuring the App",
                button: MessageBoxButton.OK,
                icon: MessageBoxImage.Error);

            // Rethrow the exception to terminate the program.
            throw;
        }
    }

    static void InitBenchFilePathProviderFromArgs(IServiceProvider serviceProvider, string[] args)
    {
        // Get the file path or use the default.
        string? benchJsonFilePath = null;
        if (args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]))
        {
            benchJsonFilePath = args[0];
        }

        // The IBenchFilePathProvider in DI is a singleton.
        // We set its FilePath here so that it is available to the rest of the app.
        var benchFilePathProvider = serviceProvider.GetRequiredService<IBenchFilePathProvider>();

        benchFilePathProvider.FilePath = benchJsonFilePath;
    }

    // Ensure this method is not inlined, so that no WPF assemblies are loaded
    // before this method is called from the Main method.
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    static void RunWpfApp(IServiceProvider serviceProvider)
    {
        _logger.LogInformation("Starting WPF application.");

        try
        {
            var app = new App(serviceProvider);
            app.InitializeComponent();
            app.Run();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RunWpfApp unhandled exception.");

            MessageBox.Show(
                "An unhandled exeception occurred in the WPF-Blazor app.\n"
                + "\n"
                + $"{ex.GetType()}\n"
                + "\n"
                + $"{ex.Message}\n",
                caption: "Error in WPF-Blazor App",
                button: MessageBoxButton.OK,
                icon: MessageBoxImage.Error);

            // Rethrow the exception to terminate the program.
            throw;
        }
        _logger.LogInformation("WPF application has finished running.");
    }
}
