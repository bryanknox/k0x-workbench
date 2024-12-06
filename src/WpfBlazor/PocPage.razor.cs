using K0x.Workbench.DataStorage.Abstractions;
using K0x.Workbench.DataStorage.Abstractions.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Windows;

namespace WpfBlazor;

public partial class PocPage : ComponentBase
{
    [Inject] private IBenchFilePathProvider BenchFilePathProvider { get; set; } = default!;
    [Inject] private IBenchFileLoader BenchFileLoader { get; set; } = default!;
    [Inject] private IBenchFileSaver BenchFileSaver { get; set; } = default!;
    [Inject] private IConfiguration Configuration { get; set; } = default!;
    [Inject] private ILogger<PocPage> Logger { get; set; } = default!;

    protected Bench? Bench { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Logger.LogTrace("OnInitialized START.");

        if (!string.IsNullOrEmpty(BenchFilePathProvider.FilePath))
        {
            Bench = await LoadBenchFromJsonFileAsync(BenchFilePathProvider.FilePath);
        }

        Logger.LogTrace("OnInitialized END.");
    }

    private async Task OpenBenchFileAsnyc(MouseEventArgs e)
    {
        Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
        dialog.FileName = "K0x Bench Sample 1"; // Default file name
        dialog.DefaultExt = ".json"; // Default file extension
        dialog.Filter = "JSON documents (.json)|*.json"; // Filter files by extension

        bool? result = dialog.ShowDialog();

        if (result is true)
        {
            Bench? bench = await LoadBenchFromJsonFileAsync(dialog.FileName);

            if (bench is not null)
            {
                BenchFilePathProvider.FilePath = dialog.FileName;

                Bench = bench;
            }
        }
    }
    private async Task NewSampleBenchFileAsnyc(MouseEventArgs e)
    {
        var dialog = new Microsoft.Win32.SaveFileDialog();
        dialog.FileName = "K0x Bench Sample 1"; // Default file name
        dialog.DefaultExt = ".json"; // Default file extension
        dialog.Filter = "JSON documents (.json)|*.json"; // Filter files by extension

        bool? result = dialog.ShowDialog();

        if (result is true)
        {
            BenchFilePathProvider.FilePath = dialog.FileName;

            Bench sampleBench = CreateSampleBench();

            await SaveBenchFileAsync(sampleBench, dialog.FileName);

            BenchFilePathProvider.FilePath = dialog.FileName;

            Bench = sampleBench;
        }
    }

    private async Task SaveAsBenchFileAsnyc(MouseEventArgs e)
    {
        if (Bench is null)
        {
            MessageBox.Show(
                "No bench to save.",
                caption: "No bench to save.",
                button: MessageBoxButton.OK,
                icon: MessageBoxImage.Information);
            return;
        }

        var dialog = new Microsoft.Win32.SaveFileDialog();
        dialog.FileName = BenchFilePathProvider.FilePath; // Default file name to current file path.
        dialog.DefaultExt = ".json"; // Default file extension
        dialog.Filter = "JSON documents (.json)|*.json"; // Filter files by extension

        bool? result = dialog.ShowDialog();

        if (result is true)
        {
            await SaveBenchFileAsync(Bench, dialog.FileName);

            BenchFilePathProvider.FilePath = dialog.FileName;
        }
    }

    private async Task<Bench?> LoadBenchFromJsonFileAsync(string jsonFilePath)
    {
        try
        {
            Bench bench = await BenchFileLoader.LoadAsync(jsonFilePath);

            return bench;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"{nameof(LoadBenchFromJsonFileAsync)} Error");

            MessageBox.Show(
                "An error occurred loading the bench from JSON file.\n"
                + $"Path: {jsonFilePath}\n"
                + "\n"
                + $"{ex.GetType()}\n"
                + "\n"
                + $"{ex.Message}\n",
                caption: "Error loading the bench from JSON file.",
                button: MessageBoxButton.OK,
                icon: MessageBoxImage.Error);

            return null;
        }
    }

    private static Bench CreateSampleBench()
    {
        return new Bench
        {
            Label = "K0x-workbench Sample Bench",
            Kits = new List<Kit>
                {
                    new Kit
                    {
                        Label = "Workspace Kit",
                        Tools = new List<Tool>
                        {
                            new Tool { Label = "File Explorer", Command = "C:/_BkGit/bryanknox/k0x-workbench" },
                            new Tool { Label = "Tool 1.2", Command = "cmd2" }
                        }
                    },
                    new Kit
                    {
                        Label = "Kit 2",
                        Tools = new List<Tool>
                        {
                            new Tool { Label = "Tool 2.1", Command = "cmd3" },
                            new Tool { Label = "Tool 2.2", Command = "cmd4" }
                        }
                    }
                }
        };
    }

    private async Task SaveBenchFileAsync(Bench bench, string jsonFilePath)
    {
        try
        {
            await BenchFileSaver.SaveAsync(bench, jsonFilePath);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"{nameof(SaveBenchFileAsync)} Error.");

            MessageBox.Show(
                "An error occurred saving the bench to JSON file.\n"
                + $"Path: {jsonFilePath}\n"
                + "\n"
                + $"{ex.GetType()}\n"
                + "\n"
                + $"{ex.Message}\n",
                caption: "Error saving the bench to JSON file.",
                button: MessageBoxButton.OK,
                icon: MessageBoxImage.Error);
        }
    }

}
