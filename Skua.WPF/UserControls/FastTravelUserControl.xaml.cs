﻿using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using CommunityToolkit.Mvvm.DependencyInjection;
using Skua.Core.Utils;
using Skua.Core.ViewModels;

namespace Skua.WPF.UserControls;
/// <summary>
/// Interaction logic for FastTravelUserControl.xaml
/// </summary>
public partial class FastTravelUserControl : UserControl
{
    ICollectionView _collectionView;
    public FastTravelUserControl()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetService<FastTravelViewModel>()!;
        _collectionView = CollectionViewSource.GetDefaultView(((FastTravelViewModel)DataContext).FastTravelItems);
        _collectionView.Filter = Search;
    }

    private bool Search(object obj)
    {
        if (string.IsNullOrEmpty(SearchBox.Text))
            return true;

        return obj is FastTravelItemViewModel item && item.DescriptionName.Contains(SearchBox.Text);
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        _collectionView.Refresh();
    }
}
