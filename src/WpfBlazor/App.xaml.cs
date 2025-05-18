using System.Windows;

namespace WpfBlazor;

/// <summary>
/// Interaction logic (code-behind) for App.xaml
/// </summary>
public partial class App : Application
{
    // This WPF app's MainWindow grabs this ServiceProvider
    // so it can setup the BlazorWebView to it
    // for dependency injection.
    public IServiceProvider ServiceProvider { get; }

//make build fail.    public App(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }
}
