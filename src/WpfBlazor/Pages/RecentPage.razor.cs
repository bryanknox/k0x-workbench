using K0x.Workbench.DataStorage.Abstractions;
using K0x.Workbench.RecentBenches.Abstractions;
using K0x.Workbench.RecentBenches.Abstractions.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using System.Windows;

namespace WpfBlazor.Pages;

public partial class RecentPage : ComponentBase
{
    [Inject]
    IBenchFilePathProvider BenchFilePathProvider { get; set; } = default!;
    [Inject]
    private ILogger<RecentPage> Logger { get; set; } = default!;
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;
    [Inject]
    private IRecentBenchesFilePathProvider RecentBenchesFilePathProvider { get; set; } = default!;
    [Inject]
    private IRecentBenchesLoader RecentBenchesLoader { get; set; } = default!;

    private List<RecentBench> RecentBenches { get; set; } = new();
    private bool IsLoading { get; set; }
    private string? ErrorMessage { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Logger.LogTrace("OnInitializedAsync START.");

        await LoadRecentBenchesAsync();

        Logger.LogTrace("OnInitializedAsync END.");
    }

    private async Task LoadRecentBenchesAsync()
    {
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            var benches = await RecentBenchesLoader.GetRecentBenchesAsync();
            RecentBenches = benches.OrderByDescending(b => b.LastOpened).ToList();

            if (RecentBenches.Count == 0)
            {
                Logger.LogInformation("No recent benches available");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading recent benches");
            ErrorMessage = "An error occurred while loading recent benches.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task ReloadRecentBenches()
    {
        await LoadRecentBenchesAsync();
    }

    private void OnRecentBenchClick(RecentBench bench)
    {
        BenchFilePathProvider.FilePath = bench.FilePath;
        NavigationManager.NavigateTo("/bench");
    }
        
   private void EditRecentBenchesFile(MouseEventArgs e)
    {
        string? recentBenchesFilePath = RecentBenchesFilePathProvider.GetFilePath();
        if (!string.IsNullOrEmpty(recentBenchesFilePath))
        {
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = recentBenchesFilePath,
                UseShellExecute = true
            };
            try
            {
                System.Diagnostics.Process.Start(psi);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error opening file for editing: {recentBenchesFilePath}");

                MessageBox.Show(
                    $"Error opening file for editing: {recentBenchesFilePath}\n"
                    + $"{ex.GetType()}\n"
                    + "\n"
                    + $"{ex.Message}\n",
                    caption: "Error opening file for editing",
                    button: MessageBoxButton.OK,
                    icon: MessageBoxImage.Error);
            }
        }
    }
}
