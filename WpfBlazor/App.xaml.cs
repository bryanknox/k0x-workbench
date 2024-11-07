using Microsoft.Extensions.Configuration;
using System.IO;
using System.Windows;

namespace WpfBlazor;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
        public IConfiguration Configuration { get; }

        public App()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();
        }
}
