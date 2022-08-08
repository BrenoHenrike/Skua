using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Skua.Core.ViewModels;

namespace Skua.WPF.UserControls;
/// <summary>
/// Interaction logic for GrabberListUserControl.xaml
/// </summary>
public partial class GrabberListUserControl : UserControl
{
    private ICollectionView? _collectionView;
    public GrabberListUserControl()
    {
        InitializeComponent();
        DataContextChanged += GrabberListUserControl_DataContextChanged;
        Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
    }

    private void Dispatcher_ShutdownStarted(object? sender, EventArgs e)
    {
        DataContextChanged -= GrabberListUserControl_DataContextChanged;
    }

    private void GrabberListUserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (DataContext is not null)
        {
            _collectionView = CollectionViewSource.GetDefaultView(((GrabberListViewModel)DataContext).GrabbedItems);
            _collectionView.Filter = Search;
        }
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

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        _collectionView?.Refresh();
    }
}
