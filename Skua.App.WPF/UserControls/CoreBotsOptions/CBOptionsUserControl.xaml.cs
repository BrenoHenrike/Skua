using Microsoft.Toolkit.Mvvm.DependencyInjection;
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

namespace Skua.App.WPF.UserControls;
/// <summary>
/// Interaction logic for CBOptionsUserControl.xaml
/// </summary>
public partial class CBOptionsUserControl : UserControl
{
    private ICollectionView _collectionView;
    public CBOptionsUserControl()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<CBOptionsViewModel>();
        _collectionView = CollectionViewSource.GetDefaultView(((CBOptionsViewModel)DataContext).Options);
        _collectionView.GroupDescriptions.Add(new PropertyGroupDescription("DisplayType"));
        //_collectionView.SortDescriptions.Add(new SortDescription("DisplayType", ListSortDirection.Ascending));
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
