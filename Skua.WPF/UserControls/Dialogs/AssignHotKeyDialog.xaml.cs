using Skua.Core.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Skua.WPF.UserControls;

/// <summary>
/// Interaction logic for AssignHotKeyDialog.xaml
/// </summary>
public partial class AssignHotKeyDialog : UserControl
{
    private Window? _window;
    private AssignHotKeyDialogViewModel? _vm;

    public AssignHotKeyDialog()
    {
        InitializeComponent();
        Loaded += AssignHotKeyDialog_Loaded;
    }

    private void AssignHotKeyDialog_Loaded(object sender, RoutedEventArgs e)
    {
        _window = Window.GetWindow(this);
        _vm = DataContext as AssignHotKeyDialogViewModel;
        Loaded -= AssignHotKeyDialog_Loaded;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var window = Window.GetWindow(this);
        window.DialogResult = true;
    }

    private string backupKey = string.Empty;

    private void AssignKey(object sender, RoutedEventArgs e)
    {
        if (_window is null || _vm is null)
            return;
        _window.KeyDown += _window_KeyDown;
        backupKey = _vm.KeyInput;
        _vm.KeyInput = "Waiting input...";
    }

    private void _window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            _vm!.KeyInput = backupKey;
            _window!.KeyDown -= _window_KeyDown;
            e.Handled = true;
            return;
        }

        if (e.Key is Key.LeftCtrl or Key.RightCtrl or Key.LeftShift or Key.RightShift or Key.LeftAlt or Key.RightAlt)
            return;

        _vm!.KeyInput = e.Key.ToString();

        _window!.KeyDown -= _window_KeyDown;
        e.Handled = true;
    }
}