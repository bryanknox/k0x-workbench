using K0x.Workbench.RecentBenches.Abstractions;
using K0x.Workbench.RecentBenches.Abstractions.Models;
using K0x.Workbench.DataStorage.Abstractions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Components;

namespace WpfBlazor.Pages;

public partial class RecentPage : ComponentBase
{
    [Inject]
    private ILogger<RecentPage> Logger { get; set; } = default!;
    [Inject]
    private IRecentBenchesLoader RecentBenchesLoader { get; set; } = default!;
    [Inject]
    private IBenchFilePathProvider BenchFilePathProvider { get; set; } = default!;
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

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

    private async Task RefreshRecentBenches()
    {
        await LoadRecentBenchesAsync();
    }

    private void OnRecentBenchClick(RecentBench bench)
    {
        BenchFilePathProvider.FilePath = bench.FilePath;
        NavigationManager.NavigateTo("/bench");
    }
}
