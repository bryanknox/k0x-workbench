using K0x.Workbench.DataStorage.Abstractions;
using K0x.Workbench.DataStorage.Abstractions.Models;
using K0x.Workbench.RecentBenches.Abstractions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Windows;
using WpfBlazor.InternalServices;

namespace WpfBlazor.Pages;

public partial class BenchPage : ComponentBase
{
    [Inject]
    private IBenchFilePathProvider BenchFilePathProvider { get; set; } = default!;
    [Inject]
    private IBenchFileLoader BenchFileLoader { get; set; } = default!;
    [Inject]
    private IBenchFileSaver BenchFileSaver { get; set; } = default!;
    [Inject]
    private IConfiguration Configuration { get; set; } = default!;
    [Inject]
    private ILogger<BenchPage> Logger { get; set; } = default!;
    [Inject]
    private IRecentBenchAdder RecentBenchAdder { get; set; } = default!;
    [Inject]
    private IAppTitleSetService TitleSetService { get; set; } = default!;

    protected Bench? Bench { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Logger.LogTrace("OnInitializedAsync START.");

        if (!string.IsNullOrEmpty(BenchFilePathProvider.FilePath))
        {
            Bench? bench = await LoadBenchFromJsonFileAsync(BenchFilePathProvider.FilePath);

            await UpdateBenchAndInfoAsync(bench, BenchFilePathProvider.FilePath);
        }

        Logger.LogTrace("OnInitializedAsync END.");
    }

    private async Task OpenBenchFileAsync(MouseEventArgs e)
    {
        Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
        dialog.FileName = BenchFilePathProvider.FilePath; // Default file name to current file path.
        dialog.DefaultExt = ".json"; // Default file extension
        dialog.Filter = "JSON documents (.json)|*.json"; // Filter files by extension

        bool? result = dialog.ShowDialog();

        if (result is true)
        {
            Bench? bench = await LoadBenchFromJsonFileAsync(dialog.FileName!);

            // Only update if successful. Keep the current bench if not.
            if (bench is not null)
            {
                await UpdateBenchAndInfoAsync(bench, dialog.FileName);
            }
        }
    }

    private async Task NewBenchFileAsync(MouseEventArgs e)
    {
        var dialog = new Microsoft.Win32.SaveFileDialog();
        dialog.FileName = "Sample_1__K0xBench"; // Default file name
        dialog.DefaultExt = ".json"; // Default file extension
        dialog.Filter = "JSON documents (.json)|*.json"; // Filter files by extension

        bool? result = dialog.ShowDialog();

        if (result is true)
        {
            Bench = null;

            Bench sampleBench = CreateSampleBench(dialog.FileName);

            await SaveBenchFileAsync(sampleBench, dialog.FileName);

            await UpdateBenchAndInfoAsync(sampleBench, dialog.FileName);
        }
    }

    private async Task SaveAsBenchFileAsync(MouseEventArgs e)
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
            await SaveBenchFileAsync(Bench, dialog.FileName!);

            await UpdateBenchAndInfoAsync(Bench, dialog.FileName);
        }
    }

    private void EditBenchFile(MouseEventArgs e)
    {
        if (!string.IsNullOrEmpty(BenchFilePathProvider.FilePath))
        {
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = BenchFilePathProvider.FilePath,
                UseShellExecute = true
            };
            try
            {
                System.Diagnostics.Process.Start(psi);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error opening file for editing: {BenchFilePathProvider.FilePath}");

                MessageBox.Show(
                    $"Error opening file for editing: {BenchFilePathProvider.FilePath}\n"
                    + $"{ex.GetType()}\n"
                    + "\n"
                    + $"{ex.Message}\n",
                    caption: "Error opening file for editing",
                    button: MessageBoxButton.OK,
                    icon: MessageBoxImage.Error);
            }
        }
    }

    private async Task ReloadBenchFileAsync(MouseEventArgs e)
    {
        if (!string.IsNullOrEmpty(BenchFilePathProvider.FilePath))
        {
            Bench? bench = await LoadBenchFromJsonFileAsync(BenchFilePathProvider.FilePath);

            // Only update if successful. Keep the current bench if not.
            if (bench is not null)
            {
                await UpdateBenchAndInfoAsync(bench, BenchFilePathProvider.FilePath);
            }
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

    private static Bench CreateSampleBench(string jsonFilePath)
    {
        string absoluteFilePath = System.IO.Path.GetFullPath(jsonFilePath);
        string folderPath = System.IO.Path.GetDirectoryName(absoluteFilePath)
            ?? string.Empty;
        string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(absoluteFilePath);

        return new Bench
        {
            Label = fileNameWithoutExtension,
            Kits = new List<Kit>
            {
                new Kit
                {
                    Label = "Kit 1",
                    Tools = new List<Tool>
                    {
                        new Tool { Label = "File Explorer", Command = folderPath },
                        new Tool { Label = "Edit File", Command = absoluteFilePath },
                    }
                },
                new Kit
                {
                    Label = "Kit 2",
                    Tools = new List<Tool>
                    {
                        new Tool { Label = "Notepad", Command = "notepad.exe" },
                        new Tool { Label = "Windows Terminal", Command = "wt" },
                        new Tool { Label = "VS Code", Command = "code" }
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

    private async Task UpdateBenchAndInfoAsync(Bench? bench, string? filePath)
    {
        Bench = bench;
        BenchFilePathProvider.SetFilePath(filePath);
        TitleSetService.SetTitle(bench?.Label);

        if (bench != null
            && !string.IsNullOrWhiteSpace(filePath))
        {
            await RecentBenchAdder.AddRecentBenchAsync(filePath, bench.Label);
        }
    }
}
