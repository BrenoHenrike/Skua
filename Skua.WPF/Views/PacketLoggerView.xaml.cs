using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CommunityToolkit.Mvvm.DependencyInjection;
using Skua.Core.ViewModels;

namespace Skua.WPF.Views;
/// <summary>
/// Interaction logic for PacketLoggerView.xaml
/// </summary>
public partial class PacketLoggerView : UserControl
{
    ICollectionView _logsCollectionView;
    ICollectionView _filtersCollectionView;
    public PacketLoggerView()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetService<PacketLoggerViewModel>()!;
        _logsCollectionView = CollectionViewSource.GetDefaultView(((PacketLoggerViewModel)DataContext!).PacketLogs);
        _logsCollectionView.Filter = SearchPackets;
        _filtersCollectionView = CollectionViewSource.GetDefaultView(((PacketLoggerViewModel)DataContext!).PacketFilters);
        _filtersCollectionView.Filter = SearchFilters;
    }

    private bool SearchPackets(object obj)
    {
        if (string.IsNullOrEmpty(txtLogSearchBox.Text))
            return true;

        return obj is string packet && packet.Contains(txtLogSearchBox.Text);
    }
    
    private bool SearchFilters(object obj)
    {
        if (string.IsNullOrEmpty(txtFilterSearchBox.Text))
            return true;

        return obj is PacketLogFilterViewModel filter && filter.Content.Contains(txtFilterSearchBox.Text);
    }

    private void UnselectAllLogs(object sender, RoutedEventArgs e)
    {
        lstPackets.UnselectAll();
    }

    private void TxtLogSearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        _logsCollectionView.Refresh();
    }

    private void TxtFilterSearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        _filtersCollectionView.Refresh();
    }
}
