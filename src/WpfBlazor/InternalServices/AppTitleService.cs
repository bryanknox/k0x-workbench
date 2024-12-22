namespace WpfBlazor.InternalServices;

public class AppTitleService : IAppTitleSetService, IAppTitleGetService
{
    // ITitleGetService members

    public Action<string?>? OnTitleChanged { get; set; }

    // ITitleSetService members

    public void SetTitle(string? title)
    {
        if (OnTitleChanged is not null)
        {
            OnTitleChanged.Invoke(title);
        }
    }
}
