using CommunityToolkit.Mvvm.ComponentModel;
using Skua.Core.Models;

namespace Skua.Core.ViewModels;

public partial class AssignHotKeyDialogViewModel : DialogViewModelBase
{
    public AssignHotKeyDialogViewModel(string title, HotKey hotKey)
        : base(title)
    {
        _keyInput = hotKey.Key;
        _ctrlCheck = hotKey.Ctrl;
        _altCheck = hotKey.Alt;
        _shiftCheck = hotKey.Shift;
    }

    public AssignHotKeyDialogViewModel(string title)
        : base(title)
    {
        _keyInput = string.Empty;
    }

    [ObservableProperty]
    private bool _ctrlCheck;

    [ObservableProperty]
    private bool _altCheck;

    [ObservableProperty]
    private bool _shiftCheck;

    [ObservableProperty]
    private string _keyInput;

    public string KeyGesture => $"{(CtrlCheck ? "Ctrl+" : string.Empty)}{(ShiftCheck ? "Shift+" : string.Empty)}{(AltCheck ? "Alt+" : string.Empty)}{KeyInput}";
}