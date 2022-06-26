using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Skua.Core.ViewModels;

namespace Skua.App.Views;
/// <summary>
/// Interaction logic for OptionsView.xaml
/// </summary>
public partial class OptionsView : UserControl
{
    ICollectionView _collectionView;
    public OptionsView()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetService<OptionsViewModel>();
        Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        _collectionView = CollectionViewSource.GetDefaultView(((OptionsViewModel)DataContext!).OptionItems);
        _collectionView.SortDescriptions.Add(new SortDescription("DisplayType", ListSortDirection.Ascending));
        _collectionView.Filter = Search;
    }

    private bool Search(object obj)
    {
        if (string.IsNullOrEmpty(SearchBox.Text))
            return true;

        return obj is OptionItemViewModel optionItem && optionItem.Content.Contains(SearchBox.Text);
    }

    private void Dispatcher_ShutdownStarted(object? sender, System.EventArgs e)
    {
        ((OptionsViewModel)DataContext).Dispose();
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        _collectionView.Refresh();
    }
}
