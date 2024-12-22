using System.Windows;
using WpfBlazor.InternalServices;

namespace WpfBlazor;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly IAppTitleGetService _appTitleGetService;

    public MainWindow(IAppTitleGetService appTitleGetService)
    {
        // The InitializeComponent method is automatically generated
        // at app build time and added to the compilation object for the calling class.
        InitializeComponent();

        // Setup dependency injection for the BlazorWebView
        // by setting it to use the WPF app's ServiceProvider.
        blazorWebView.Services = ((App)Application.Current).ServiceProvider;

        // Wire up the AppTitleGetService.
        _appTitleGetService = appTitleGetService;
        _appTitleGetService.OnTitleChanged = OnAppTitleChanged;
        OnAppTitleChanged(null); // Set the default title.
    }

    private void OnAppTitleChanged(string? title)
    {
        Title = string.IsNullOrEmpty(title) ? "K0x Workbench" : $"{title} - K0x Workbench";
    }
}
