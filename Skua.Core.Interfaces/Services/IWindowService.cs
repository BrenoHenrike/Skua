namespace Skua.Core.Interfaces.Services;
public interface IWindowService
{
    void ShowWindow<TViewModel>() where TViewModel : class;
}
