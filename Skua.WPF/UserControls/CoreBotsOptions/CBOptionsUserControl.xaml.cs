using CommunityToolkit.Mvvm.DependencyInjection;
using Skua.Core.ViewModels;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;

namespace Skua.WPF.UserControls;
/// <summary>
/// Interaction logic for CBOptionsUserControl.xaml
/// </summary>
public partial class CBOptionsUserControl : UserControl
{
    private readonly ICollectionView _collectionView;
    public CBOptionsUserControl()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<CBOptionsViewModel>();
        _collectionView = CollectionViewSource.GetDefaultView(((CBOptionsViewModel)DataContext).Options);
        _collectionView.GroupDescriptions.Add(new PropertyGroupDescription("DisplayType"));
        _collectionView.SortDescriptions.Add(new SortDescription("Content", ListSortDirection.Ascending));
        _collectionView.Filter = Search;
    }

    private bool Search(object obj)
    {
        if (string.IsNullOrWhiteSpace(SearchBox.Text))
            return true;

        return obj is DisplayOptionItemViewModelBase opt && opt.Content.Contains(SearchBox.Text);
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        _collectionView.Refresh();
    }
}
