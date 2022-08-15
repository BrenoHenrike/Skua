using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;

namespace Skua.Core.ViewModels;
public partial class HotKeyItemViewModel : ObservableObject, IHotKey
{
    public HotKeyItemViewModel(string title, string keyGesture)
    {
        Title = title;
        _keyGesture = keyGesture;
    }

    public HotKeyItemViewModel() { }

    public string Binding { get; set; }
    public string Title { get; set; } = string.Empty;

    [ObservableProperty]
    private string _keyGesture;

    [RelayCommand]
    private void EditHotKey()
    {
        StrongReferenceMessenger.Default.Send<EditHotKeyMessage>(new(Title, KeyGesture));
    }
}
