using System.Windows;
using System.Windows.Controls;

namespace Skua.WPF.UserControls;

/// <summary>
/// Interaction logic for FastTravelEditorDialog.xaml
/// </summary>
public partial class FastTravelEditorDialog : UserControl
{
    public FastTravelEditorDialog()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Window parent = Window.GetWindow(this);
        parent.DialogResult = true;
    }
}