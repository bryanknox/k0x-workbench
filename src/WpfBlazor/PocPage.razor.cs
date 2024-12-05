using K0x.DataStorage.JsonFiles;
using K0x.Workbench.DataStorage.Abstractions;
using K0x.Workbench.DataStorage.Abstractions.Models;
using K0x.Workbench.DataStorage.JsonFiles;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace WpfBlazor;

public partial class PocPage : ComponentBase
{
    [Inject] private IBenchFilePathProvider BenchFilePathProvider { get; set; } = default!;
    [Inject] private IConfiguration Configuration { get; set; } = default!;
    [Inject] private ILogger<PocPage> Logger { get; set; } = default!;

    protected string AppSetting1 { get; set; } = string.Empty;
    protected string Path { get; set; } = "C:/Users/bryan/OneDrive/BK Shortcuts";
    protected bool? IsPathExist { get; set; } = null;
    protected Bench? Bench { get; set; }

    protected override void OnInitialized()
    {
        Logger.LogTrace("OnInitialized START.");

        Logger.LogTrace("OnInitialized trace.");
        Logger.LogDebug("OnInitialized debug.");
        Logger.LogInformation("OnInitialized information.");
        Logger.LogWarning("OnInitialized warning.");
        Logger.LogError("OnInitialized error.");
        Logger.LogCritical("OnInitialized critical.");

        AppSetting1 = Configuration["AppSettings:Setting1"] ?? "Setting1 Not Found";
        CheckPathExistence();

        Bench = LoadBenchOrThrow();
        Logger.LogDebug($"bench: {Bench}");

        Logger.LogTrace("OnInitialized END.");
    }

    private void CheckPathExistence()
    {
        IsPathExist = Directory.Exists(Path);
    }

    private void OpenDirectory()
    {
        if (IsPathExist == true)
        {
            var psi = new ProcessStartInfo
            {
                FileName = Path,
                UseShellExecute = true
            };
            Process.Start(psi);
        }
    }

    private Bench LoadBenchOrThrow()
    {
        if (BenchProvider.Bench is null)
        {
            Logger.LogError("BenchProvider.Bench is null.");
            throw new InvalidOperationException(
                "BenchProvider.Bench is null."
                + " This is an implementation error."
                + " BenchProvider.Bench should be initialized"
                + $" before the {nameof(PocPage)} is created."
            );
        }

        return BenchProvider.Bench;
    }


    private void LoadBenchFromJsonFile()
    {
        try
        {
            CreateBenchFileIfNotExistAsync()
                .GetAwaiter()
                .GetResult();


            var benchJsonFileModel = jsonFileLoader.LoadAsync(BenchFilePathProvider.FilePath)
                .GetAwaiter()
                .GetResult();

            Bench = benchJsonFileModel.Bench;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading the bench from JSON file.");

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
}
