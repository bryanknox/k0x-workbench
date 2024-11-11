namespace WpfBlazor;

public class Program
{
    [STAThread]
    static void Main()
    {
        var app = new App();
        app.InitializeComponent();
        app.Run();
    }
}
