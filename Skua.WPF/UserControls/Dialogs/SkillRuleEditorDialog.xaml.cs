using System.Windows;
using System.Windows.Controls;

namespace Skua.WPF.UserControls;
/// <summary>
/// Interaction logic for SkillRuleEditorDialog.xaml
/// </summary>
public partial class SkillRuleEditorDialog : UserControl
{
    public SkillRuleEditorDialog()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Window parent = Window.GetWindow(this);
        parent.DialogResult = true;
    }
}
