using K0x.Benchy.DataStorage.Abstractions;
using K0x.Benchy.DataStorage.Abstractions.Models;
using K0x.Benchy.DataStorage.JsonFiles;
using K0x.DataStorage.JsonFiles;
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
        string? benchJsonFilePath = args.Length > 0 ? args[0] : null;

        IServiceProvider serviceProvider = SetupProgramConfigurationAndDI();

        // Get the logger for use within this class.
        _logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        LoadBenchFromJsonFile(serviceProvider, benchJsonFilePath);

        RunWpfApp(serviceProvider);

        _logger.LogInformation("Done.");
    }

    static async Task CreateBenchFileIfNotExistAsync(
        IServiceProvider serviceProvider,
        string jsonFilePath)
    {
        try
        {
            var isExist = System.IO.File.Exists(jsonFilePath);
            if (!isExist)
            {
                var bench = new Bench
                {
                    Label = "PoC k0x-workbench Bench",
                    Trays = new List<Tray>
                    {
                        new Tray
                        {
                            Label = "Workspace",
                            Tools = new List<Tool>
                            {
                                new Tool { Label = "File Explorer", Command = "C:/_BkGit/bryanknox/k0x-workbench" },
                                new Tool { Label = "Tool 1.2", Command = "cmd2" }
                            }
                        },
                        new Tray
                        {
                            Label = "Tray 2",
                            Tools = new List<Tool>
                            {
                                new Tool { Label = "Tool 2.1", Command = "cmd3" },
                                new Tool { Label = "Tool 2.2", Command = "cmd4" }
                            }
                        }
                    }
                };

                var benchyFileSaver = serviceProvider.GetRequiredService<IBenchyFileSaver>();

                await benchyFileSaver.SaveAsync(bench, "poc-bench.json");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateBenchFileIfNotExistAsync Error.");
            throw;
        }
    }

    static void LoadBenchFromJsonFile(
        IServiceProvider serviceProvider,
        string? benchJsonFilePath)
    {
        try
        {
            string path = !string.IsNullOrWhiteSpace(benchJsonFilePath)
                ? benchJsonFilePath
                : "poc-bench.json";

            CreateBenchFileIfNotExistAsync(serviceProvider, path)
                .GetAwaiter()
                .GetResult();

            var jsonFileLoader = serviceProvider.GetRequiredService<IJsonFileLoader<BenchyJsonFileModel>>();
            var benchJsonFileModel = jsonFileLoader.LoadAsync(path)
                .GetAwaiter()
                .GetResult();

            var benchProvider = serviceProvider.GetRequiredService<IBenchProvider>();
            benchProvider.Bench = benchJsonFileModel.Bench;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading the bench from JSON file.");

            MessageBox.Show(
                "An error occurred loading the bench from JSON file.\n"
                + "The application cannot be started.\n"
                + "\n"
                + $"{ex.GetType()}\n"
                + "\n"
                + $"{ex.Message}\n",
                caption: "Error loading the bench from JSON file.",
                button: MessageBoxButton.OK,
                icon: MessageBoxImage.Error);

            throw;
        }
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
