namespace Skua.Core.Interfaces;
public interface IWindowService
{
    void ShowWindow<TViewModel>() where TViewModel : class;
    void ShowWindow<TViewModel>(TViewModel viewModel) where TViewModel : class;
    void ShowManagedWindow(string key);
    void RegisterManagedWindow<TViewModel>(string key, TViewModel viewModel) where TViewModel : class, IManagedWindow;
}
