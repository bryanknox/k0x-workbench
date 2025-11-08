using K0x.Workbench.DataStorage.Abstractions;
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

        benchFilePathProvider.SetFilePath(benchJsonFilePath);
    }

    static string GetExceptionInfo(Exception ex)
    {
        var lines = new List<string>();
        Exception? current = ex;

        while (current != null)
        {
            lines.Add(current.GetType().FullName ?? current.GetType().Name + ":");
            lines.Add(current.Message);
            lines.Add(string.Empty);
            current = current.InnerException;
        }

        return string.Join(Environment.NewLine, lines);
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

            // Resolve MainWindow with dependencies
            var mainWindow = serviceProvider.GetRequiredService<MainWindow>();

            int exitCode = app.Run(mainWindow);

            _logger.LogInformation("WPF application has finished running with exit code {ExitCode}.", exitCode);
        }
        catch (System.Reflection.TargetInvocationException ex) when (ex.InnerException is Microsoft.Web.WebView2.Core.WebView2RuntimeNotFoundException)
        {
            var webView2Ex = (Microsoft.Web.WebView2.Core.WebView2RuntimeNotFoundException)ex.InnerException;
            _logger.LogError(webView2Ex, "WebView2 Runtime not found.");

            string exceptionInfo = GetExceptionInfo(ex);

            _logger.LogError("Exception Details:\n" + exceptionInfo);

            MessageBox.Show(
                "WebView2 Runtime is required but not installed on this system.\n"
                + "\n"
                + "Please install the Microsoft Edge WebView2 Runtime from:\n"
                + "https://developer.microsoft.com/en-us/microsoft-edge/webview2/?form=MA13LH\n"
                + "\n"
                + "The application cannot continue without the WebView2 Runtime.\n"
                + "\n"
                + "Exception Details:\n"
                + exceptionInfo,
                caption: "WebView2 Runtime Required",
                button: MessageBoxButton.OK,
                icon: MessageBoxImage.Warning);

            // Don't rethrow - exit gracefully
            return;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RunWpfApp unhandled exception.");

            string exceptionInfo = GetExceptionInfo(ex);

            _logger.LogError("Exception Details:\n" + exceptionInfo);

            MessageBox.Show(
                "An unhandled exception occurred in the WPF-Blazor app.\n"
                + "\n"
                + "Exception Details:\n"
                + exceptionInfo,
                caption: "Error in WPF-Blazor App",
                button: MessageBoxButton.OK,
                icon: MessageBoxImage.Error);

            // Rethrow the exception to terminate the program.
            throw;
        }
        _logger.LogInformation("WPF application has finished running.");
    }
}
