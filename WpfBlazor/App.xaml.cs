using System.Windows;

namespace WpfBlazor;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    // This WPF app's MainWindow grabs this ServiceProvider
    // so it can setup the BlazorWebView to it
    // for dependency injection.
    public IServiceProvider ServiceProvider { get; }

    public App(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }
}
