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
using Skua.Core.ViewModels;

namespace Skua.App.UserControls;
/// <summary>
/// Interaction logic for GrabberListUserControl.xaml
/// </summary>
public partial class GrabberListUserControl : UserControl
{
    ICollectionView? _collectionView;
    public GrabberListUserControl()
    {
        InitializeComponent();
        Loaded += GrabberListUserControl_Loaded;
    }

    private void GrabberListUserControl_Loaded(object sender, RoutedEventArgs e)
    {
        if(DataContext is not null)
        {
            _collectionView = CollectionViewSource.GetDefaultView(((GrabberListViewModel)DataContext).GrabbedItems);
            _collectionView.Filter = Search;
        }
        Loaded -= GrabberListUserControl_Loaded;
    }

    private bool Search(object obj)
    {
        if(string.IsNullOrWhiteSpace(SearchBox.Text))
            return true;

        return obj.ToString()?.Contains(SearchBox.Text) ?? false;
    }

    private void MenuItem_Click(object sender, RoutedEventArgs e)
    {
        GrabberListBox.UnselectAll();
    }
}
