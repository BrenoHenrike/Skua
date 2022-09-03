
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;

namespace Skua.Core.ViewModels;
public sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string _title = "Skua";

    public MainViewModel()
    {
        _title = $"Skua - {Ioc.Default.GetRequiredService<ISettingsService>().Get("ApplicationVersion", "0.0.0.0")}";
    }

    [RelayCommand]
    private void ShowMainWindow()
    {
        StrongReferenceMessenger.Default.Send<ShowMainWindowMessage>();
    }
}
