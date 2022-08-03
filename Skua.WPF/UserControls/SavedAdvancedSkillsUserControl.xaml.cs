using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Skua.Core.Models.Skills;
using Skua.Core.ViewModels;

namespace Skua.WPF.UserControls;
/// <summary>
/// Interaction logic for SavedAdvancedSkillsUserControl.xaml
/// </summary>
public partial class SavedAdvancedSkillsUserControl : UserControl
{
    ICollectionView _collectionView;
    public SavedAdvancedSkillsUserControl()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetService<SavedAdvancedSkillsViewModel>();
        _collectionView = CollectionViewSource.GetDefaultView(((SavedAdvancedSkillsViewModel)DataContext!).LoadedSkills);
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
