using CommunityToolkit.Mvvm.DependencyInjection;
using Skua.Core.ViewModels;
using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;

namespace Skua.WPF.Views;

/// <summary>
/// Interaction logic for PacketInterceptorView.xaml
/// </summary>
public partial class PacketInterceptorView : UserControl
{
    private readonly ICollectionView _collectionView;

    public PacketInterceptorView()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<PacketInterceptorViewModel>();
        _collectionView = CollectionViewSource.GetDefaultView(((PacketInterceptorViewModel)DataContext).Packets);
        _collectionView.Filter = Search;
    }

    private bool Search(object obj)
    {
        if (string.IsNullOrWhiteSpace(SearchBox.Text))
            return true;

        return obj is InterceptedPacketViewModel pkt && pkt.Packet.Contains(SearchBox.Text, StringComparison.OrdinalIgnoreCase);
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        _collectionView.Refresh();
    }
}
