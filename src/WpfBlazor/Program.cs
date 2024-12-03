using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System.Runtime.CompilerServices;
using System.Windows;

namespace WpfBlazor;

public class Program
{
    private static ILogger<Program> _logger = null!;

    [STAThread]
    static void Main(string[] args)
    {
        string benchJsonFilePath = args.Length > 0 ? args[0] : "poc-bench.json";

        IServiceProvider serviceProvider = SetupProgramConfigurationAndDI(benchJsonFilePath);

        // Get the logger for use within this class.
        _logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        RunWpfApp(serviceProvider);

        _logger.LogInformation("Done.");
    }

    static IServiceProvider SetupProgramConfigurationAndDI(string benchJsonFilePath)
    {
        try
        {
            return ProgramConfiguration.Setup(benchJsonFilePath);
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
