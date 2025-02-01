using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace WpfBlazor.Pages;

public partial class RecentPage : ComponentBase
{
    [Inject]
    private ILogger<RecentPage> Logger { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        Logger.LogTrace("OnInitializedAsync START.");

        await base.OnInitializedAsync();

        Logger.LogTrace("OnInitializedAsync END.");
    }

}
