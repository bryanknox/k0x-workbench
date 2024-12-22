namespace WpfBlazor.InternalServices;

public interface IAppTitleGetService
{
    Action<string?>? OnTitleChanged { get; set; }
}
