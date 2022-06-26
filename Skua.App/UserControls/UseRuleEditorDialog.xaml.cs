using System.Windows;
using System.Windows.Controls;

namespace Skua.App.UserControls;
/// <summary>
/// Interaction logic for EditAdvancedSkillDialog.xaml
/// </summary>
public partial class UseRuleEditorDialog : UserControl
{
    public UseRuleEditorDialog()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Window parent = Window.GetWindow(this);
        parent.DialogResult = true;
    }
}
