using System.Windows;
using System.Windows.Controls;

namespace Skua.WPF.UserControls;

/// <summary>
/// Interaction logic for MessageBoxDialog.xaml
/// </summary>
public partial class MessageBoxDialog : UserControl
{
    public MessageBoxDialog()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Window parent = Window.GetWindow(this);
        parent.DialogResult = true;
    }
}