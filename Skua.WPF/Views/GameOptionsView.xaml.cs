using CommunityToolkit.Mvvm.DependencyInjection;
using Skua.Core.ViewModels;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;

namespace Skua.WPF.Views;
/// <summary>
/// Interaction logic for GameOptionsView.xaml
/// </summary>
public partial class GameOptionsView : UserControl
{
    private readonly ICollectionView _collectionView;
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
