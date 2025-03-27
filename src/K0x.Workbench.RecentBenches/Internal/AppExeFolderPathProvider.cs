namespace K0x.Workbench.RecentBenches.Internal;

public class AppExeFolderPathProvider : IAppExeFolderPathProvider
{
    public string GetAppExeFolderPath()
    {
        return AppDomain.CurrentDomain.BaseDirectory;
    }
}
