using K0x.Workbench.DataStorage.Abstractions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace WpfBlazor.Pages;

/// <summary>
/// The index page that determines the startup route based on whether a bench file
/// is specified.
/// If a bench file is specified, it navigates to the BenchPage;
/// otherwise, it navigates to the RecentPage.
/// </summary>
public partial class Index : ComponentBase
{
    [Inject]
    private IBenchFilePathProvider BenchFilePathProvider { get; set; } = default!;
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;
    [Inject]
    private ILogger<Index> Logger { get; set; } = default!;

    protected override void OnInitialized()
    {
        Logger.LogTrace("Index page OnInitialized START.");

        // Determine the startup route based on whether a bench file is specified
        string startupRoute = DetermineStartupRoute();

        Logger.LogInformation($"Navigating from index to startup route: {startupRoute}");

        // Navigate to the determined startup route
        NavigationManager.NavigateTo(startupRoute);

        Logger.LogTrace("Index page OnInitialized END.");
    }

    private string DetermineStartupRoute()
    {
        // If a bench file is specified, open BenchPage, otherwise open RecentPage
        if (!string.IsNullOrWhiteSpace(BenchFilePathProvider.FilePath))
        {
            Logger.LogDebug($"Bench file specified: '{BenchFilePathProvider.FilePath}'. Navigating to BenchPage.");
            return "/bench";
        }
        else
        {
            Logger.LogDebug("No bench file specified. Navigating to RecentPage.");
            return "/recent";
        }
    }
}
