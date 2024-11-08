using Microsoft.AspNetCore.Components.WebView.Wpf;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace WpfBlazor;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        // The InitializeComponent method is automatically generated
        // at app build time and added to the compilation object for the calling class.
        InitializeComponent();

        // Get the service provider from the WPF app's App class.
        var serviceProvider = ((App)Application.Current).ServiceProvider;

        // Set the BlazorWebView's Services property to that service provider.
        blazorWebView.Services = serviceProvider;
    }
}
