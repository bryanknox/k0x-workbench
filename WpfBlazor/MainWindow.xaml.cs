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

        // Setup dependency injection for the BlazorWebView
        // by setting it to use the WPF app's ServiceProvider.
        blazorWebView.Services = ((App)Application.Current).ServiceProvider;
    }
}
