using System.Windows;
using System.Windows.Controls;

namespace Skua.WPF.UserControls;
/// <summary>
/// Interaction logic for InputDialog.xaml
/// </summary>
public partial class InputDialog : UserControl
{
    public InputDialog()
    {
        InitializeComponent();
    }

    private void BtnConfirm_Click(object sender, RoutedEventArgs e)
    {
        Window parent = Window.GetWindow(this);
        parent.DialogResult = true;
    }
}
