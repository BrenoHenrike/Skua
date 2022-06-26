using System.Diagnostics;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace Skua.Core.ViewModels;
public class MainMenuItemViewModel : ObservableObject
{
    private string _header = "Default Title";
    public string Header
    {
        get { return _header; }
        set { SetProperty(ref _header, value); }
    }

    public IRelayCommand Click { get; }

    public MainMenuItemViewModel(string header, Action execute)
    {
        Header = header;
        Click = new RelayCommand(execute);
    }

    public MainMenuItemViewModel()
    {
        Click = new RelayCommand(() => Debug.WriteLine("Test"));
    }
}
