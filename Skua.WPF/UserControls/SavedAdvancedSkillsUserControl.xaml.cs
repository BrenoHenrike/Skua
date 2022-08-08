using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using CommunityToolkit.Mvvm.DependencyInjection;
using Skua.Core.Models.Skills;
using Skua.Core.ViewModels;

namespace Skua.WPF.UserControls;
/// <summary>
/// Interaction logic for SavedAdvancedSkillsUserControl.xaml
/// </summary>
public partial class SavedAdvancedSkillsUserControl : UserControl
{
    private readonly ICollectionView _collectionView;
    public SavedAdvancedSkillsUserControl()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<SavedAdvancedSkillsViewModel>();
        _collectionView = CollectionViewSource.GetDefaultView(((SavedAdvancedSkillsViewModel)DataContext).LoadedSkills);
        _collectionView.Filter = Search;
    }

    private bool Search(object obj)
    {
        if (string.IsNullOrWhiteSpace(SearchBox.Text))
            return true;

        return obj is AdvancedSkill skill && skill.ClassName.Contains(SearchBox.Text);
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        _collectionView.Refresh();
    }
}
