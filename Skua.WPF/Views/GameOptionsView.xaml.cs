using CommunityToolkit.Mvvm.DependencyInjection;
using Skua.Core.ViewModels;
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

namespace Skua.WPF.Views;
/// <summary>
/// Interaction logic for GameOptionsView.xaml
/// </summary>
public partial class GameOptionsView : UserControl
{
    private ICollectionView _collectionView;
    public GameOptionsView()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetService<GameOptionsViewModel>();
        _collectionView = CollectionViewSource.GetDefaultView(((GameOptionsViewModel)DataContext!).GameOptions);
        _collectionView.SortDescriptions.Add(new SortDescription("Tag", ListSortDirection.Ascending));
        _collectionView.SortDescriptions.Add(new SortDescription("Content", ListSortDirection.Ascending));
        _collectionView.Filter = Search;
    }

    private bool Search(object obj)
    {
        if (string.IsNullOrEmpty(SearchBox.Text))
            return true;

        return obj is DisplayOptionItemViewModelBase optionItem && optionItem.Content.Contains(SearchBox.Text);
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        _collectionView.Refresh();
    }
}
