using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using CommunityToolkit.Mvvm.DependencyInjection;
using Skua.Core.Models.Quests;
using Skua.Core.ViewModels;

namespace Skua.WPF.Views;
/// <summary>
/// Interaction logic for LoaderUserControl.xaml
/// </summary>
public partial class LoaderView : UserControl
{
    private readonly ICollectionView _collectionView;
    public LoaderView()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<LoaderViewModel>();
        _collectionView = CollectionViewSource.GetDefaultView(((LoaderViewModel)DataContext).QuestIDs);
        _collectionView.Filter = Search;
        Loaded += LoaderUserControl_Loaded;
    }

    private async void LoaderUserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        await ((LoaderViewModel)DataContext).GetQuestsCommand.ExecuteAsync(false);
        Loaded -= LoaderUserControl_Loaded;
    }

    private bool Search(object obj)
    {
        if (string.IsNullOrEmpty(SearchBox.Text))
            return true;

        return obj is QuestData item && item.ToString().Contains(SearchBox.Text);
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        _collectionView.Refresh();
    }
}
