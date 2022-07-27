using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Skua.Core.Interfaces;

namespace Skua.Core.ViewModels;
public partial class MainMenuItemViewModel : ObservableObject
{
    public MainMenuItemViewModel(string header, IEnumerable<MainMenuItemViewModel> subItems)
    {
        Header = header;
        SubItems = subItems.ToList();
        Command = new RelayCommand(OpenManagedWindow);
    }

    public MainMenuItemViewModel(string header)
    {
        Header = header;
        Command = new RelayCommand(OpenManagedWindow);
    }

    public MainMenuItemViewModel(string header, IRelayCommand command)
    {
        Header = header;
        Command = command;
    }

    [ObservableProperty]
    private string _header = "Default Title";
    [ObservableProperty]
    private List<MainMenuItemViewModel>? _subItems = null;
    public IRelayCommand Command { get; }

    private void OpenManagedWindow()
    {
        Ioc.Default.GetRequiredService<IWindowService>().ShowManagedWindow(Header);
    }
}
