using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System.Windows;

namespace WpfBlazor;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public IServiceProvider ServiceProvider { get; }

    public App(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }
}
