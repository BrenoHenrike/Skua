using Skua.Core.Models.Skills;
using System;
using System.Windows.Controls;
using System.Windows.Data;

namespace Skua.WPF.UserControls;

/// <summary>
/// Interaction logic for SavedAdvancedSkillsUserControl.xaml
/// </summary>
public partial class SavedAdvancedSkillsUserControl : UserControl
{
    public SavedAdvancedSkillsUserControl()
    {
        InitializeComponent();
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var view = CollectionViewSource.GetDefaultView(SkillsList.ItemsSource);
        view.Filter = o =>
        {
            if (o is AdvancedSkill skill)
            {
                return skill.ClassName.Contains(SearchBox.Text, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        };
    }
}