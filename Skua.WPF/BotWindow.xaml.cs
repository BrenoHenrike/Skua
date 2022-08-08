using Skua.Core.ViewModels;
using System.ComponentModel;
using System.Windows.Data;

namespace Skua.WPF;
/// <summary>
/// Interaction logic for BotWindow.xaml
/// </summary>
public partial class BotWindow : CustomWindow
{
    private ICollectionView? _collectionView;
    public BotWindow()
    {
        InitializeComponent();
        Loaded += BotWindow_Loaded;
    }

    private void BotWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        Loaded -= BotWindow_Loaded;
        var cvs = FindResource("BotViewsSource") as CollectionViewSource;
        _collectionView = cvs?.View ?? null;
        if(_collectionView is not null)
            _collectionView.Filter = Search;
    }

    private bool Search(object obj)
    {
        if (string.IsNullOrEmpty(BotControlsSearchBox.Text))
            return true;

        return obj is BotControlViewModelBase vm && vm.Title.Contains(BotControlsSearchBox.Text);
    }

    private void BotControlsSearchBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        _collectionView?.Refresh();
    }

    private void MenuToggleButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        BotControlsSearchBox.Focus();
    }
}
