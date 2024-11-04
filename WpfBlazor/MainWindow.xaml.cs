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

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddWpfBlazorWebView();
        Resources.Add("services", serviceCollection.BuildServiceProvider());
    }
}
