using K0x.Workbench.DataStorage.Abstractions;
using K0x.Workbench.DataStorage.Abstractions.Models;
using Microsoft.AspNetCore.Components;
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

        // AppSetting1 = Configuration["AppSettings:Setting1"] ?? "Setting1 Not Found";

        await LoadBenchFromJsonFileAsync();

        Logger.LogTrace("OnInitialized END.");
    }

    private async Task LoadBenchFromJsonFileAsync()
    {
        try
        {
            await CreateBenchFileIfNotExistAsync(BenchFilePathProvider.FilePath);

            Bench = await BenchFileLoader.LoadAsync(BenchFilePathProvider.FilePath);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"{nameof(LoadBenchFromJsonFileAsync)} Error");

            MessageBox.Show(
                "An error occurred loading the bench from JSON file.\n"
                + $"Path: {BenchFilePathProvider.FilePath}\n"
                + "\n"
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

    private async Task CreateBenchFileIfNotExistAsync(string jsonFilePath)
    {
        try
        {
            var isExist = System.IO.File.Exists(jsonFilePath);
            if (!isExist)
            {
                var bench = new Bench
                {
                    Label = "PoC k0x-workbench Bench",
                    Kits = new List<Kit>
                    {
                        new Kit
                        {
                            Label = "Workspace",
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

                await BenchFileSaver.SaveAsync(bench, "poc-bench.json");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"{nameof(CreateBenchFileIfNotExistAsync)} Error.");
            throw;
        }

    }
}
