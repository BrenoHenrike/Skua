using System.Windows;
using System.Windows.Controls;

namespace Skua.App.WPF.UserControls;
/// <summary>
/// Interaction logic for InputDialog.xaml
/// </summary>
public partial class InputDialog : UserControl
{
    public InputDialog()
    {
        InitializeComponent();
        Loaded += InputDialog_Loaded;
    }

    private void InputDialog_Loaded(object sender, RoutedEventArgs e)
    {
        btnConfirm.Click += BtnConfirm_Click;
        Loaded -= InputDialog_Loaded;
    }

    private void BtnConfirm_Click(object sender, RoutedEventArgs e)
    {
        Window parent = Window.GetWindow(this);
        parent.DialogResult = true;
    }
}
