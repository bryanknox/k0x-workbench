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

    protected Kit? BenchKit { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Logger.LogTrace("OnInitializedAsync START.");

        if (!string.IsNullOrEmpty(BenchFilePathProvider.FilePath))
        {
            Kit? benchKit = await LoadBenchKitFromJsonFileAsync(BenchFilePathProvider.FilePath);

            await UpdateBenchKitAndInfoAsync(benchKit, BenchFilePathProvider.FilePath);
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
            Kit? benchKit = await LoadBenchKitFromJsonFileAsync(dialog.FileName!);

            // Only update if successful. Keep the current bench if not.
            if (benchKit is not null)
            {
                await UpdateBenchKitAndInfoAsync(benchKit, dialog.FileName);
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
            BenchKit = null;

            Kit sampleBenchKit = CreateSampleBenchKit(dialog.FileName);

            await SaveBenchFileAsync(sampleBenchKit, dialog.FileName);

            await UpdateBenchKitAndInfoAsync(sampleBenchKit, dialog.FileName);
        }
    }

    private async Task SaveAsBenchFileAsync(MouseEventArgs e)
    {
        if (BenchKit is null)
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
            await SaveBenchFileAsync(BenchKit, dialog.FileName!);

            await UpdateBenchKitAndInfoAsync(BenchKit, dialog.FileName);
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
            Kit? benchKit = await LoadBenchKitFromJsonFileAsync(BenchFilePathProvider.FilePath);

            // Only update if successful. Keep the current bench if not.
            if (benchKit is not null)
            {
                await UpdateBenchKitAndInfoAsync(benchKit, BenchFilePathProvider.FilePath);
            }
        }
    }

    private async Task<Kit?> LoadBenchKitFromJsonFileAsync(string jsonFilePath)
    {
        try
        {
            Kit benchKit = await BenchFileLoader.LoadAsync(jsonFilePath);

            return benchKit;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"{nameof(LoadBenchKitFromJsonFileAsync)} Error");

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

    private static Kit CreateSampleBenchKit(string jsonFilePath)
    {
        string absoluteFilePath = System.IO.Path.GetFullPath(jsonFilePath);
        string folderPath = System.IO.Path.GetDirectoryName(absoluteFilePath)
            ?? string.Empty;
        string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(absoluteFilePath);

        return new Kit
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

    private async Task SaveBenchFileAsync(Kit benchKit, string jsonFilePath)
    {
        try
        {
            await BenchFileSaver.SaveAsync(benchKit, jsonFilePath);
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

    private async Task UpdateBenchKitAndInfoAsync(Kit? benchKit, string? filePath)
    {
        BenchKit = benchKit;
        BenchFilePathProvider.SetFilePath(filePath);
        TitleSetService.SetTitle(benchKit?.Label);

        if (benchKit != null
            && !string.IsNullOrWhiteSpace(filePath))
        {
            await RecentBenchAdder.AddRecentBenchAsync(filePath, benchKit.Label);
        }
    }
}
