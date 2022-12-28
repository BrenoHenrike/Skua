using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace Skua.Core.ViewModels;
public partial class AccountItemViewModel : ObservableObject
{
    [ObservableProperty]
    private string _displayName;
    [ObservableProperty]
    private string _username;
    [ObservableProperty]
    private string _password;
    [ObservableProperty]
    private bool _useCheck;

    [RelayCommand]
    private void Remove()
    {
        // TODO remove account
    }
}
