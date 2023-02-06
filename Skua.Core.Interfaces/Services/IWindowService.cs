namespace Skua.Core.Interfaces;
public interface IWindowService
{
    void ShowHostWindow<TViewModel>(int width, int height) where TViewModel : class;
    void ShowWindow<TViewModel>() where TViewModel : class;
    void ShowWindow<TViewModel>(TViewModel viewModel) where TViewModel : class;
    void ShowManagedWindow(string key);
    void RegisterManagedWindow<TViewModel>(string key, TViewModel viewModel) where TViewModel : class, IManagedWindow;
}
