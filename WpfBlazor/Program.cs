﻿using System.Runtime.CompilerServices;
using System.Windows;

namespace WpfBlazor;

public class Program
{
    [STAThread]
    static void Main()
    {
        IServiceProvider serviceProvider;
        try
        {
            serviceProvider = DependencyInjection.Configure();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"An error occurred:\n\n{ex.GetType()}\n\n{ex.Message}\n\nThe application cannot be started.",
                "Error Configuring the App",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            // Rethrow the exception to terminate the application.
            throw;
        }

        RunWpfApp(serviceProvider);
    }

    // Ensure this method is not inlined, so that no WPF assemblies are loaded
    // before this method is called from the Main method.
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    static void RunWpfApp(IServiceProvider serviceProvider)
    {
        var app = new App(serviceProvider);
        app.InitializeComponent();
        app.Run();
    }
}
