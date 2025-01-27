namespace K0x.Workbench.RecentBenches;

public class AppExeFolderPathProvider : IAppExeFolderPathProvider
{
    public string GetAppExeFolderPath()
    {
        return AppDomain.CurrentDomain.BaseDirectory;
    }
}
