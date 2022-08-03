using Skua.Core.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Skua.WPF.UserControls;
/// <summary>
/// Interaction logic for CustomMessageBoxDialog.xaml
/// </summary>
public partial class CustomMessageBoxDialog : UserControl
{
    public CustomMessageBoxDialog()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;
        Window parent = Window.GetWindow(this);
        CustomDialogViewModel vm = (CustomDialogViewModel)parent.DataContext;
        string text = button.Content.ToString() ?? string.Empty;
        vm.Result = new(text, vm.Buttons.IndexOf(text));
        parent.DialogResult = true;
    }
}
