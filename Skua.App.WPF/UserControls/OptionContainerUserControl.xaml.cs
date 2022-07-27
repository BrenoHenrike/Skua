using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Skua.App.WPF.UserControls;
/// <summary>
/// Interaction logic for OptionContainerUserControl.xaml
/// </summary>
public partial class OptionContainerUserControl : UserControl
{
    public OptionContainerUserControl()
    {
        InitializeComponent();
    }

    private void DataGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        var dep = (DependencyObject)e.OriginalSource;
        while (dep is not null and not DataGridCell)
        {
            dep = VisualTreeHelper.GetParent(dep);
        }

        if (dep == null)
            return;

        if (dep is not DataGridCell cell)
            return;
        if (cell is null)
            return;


        while (dep is not null and not DataGridRow)
        {
            dep = VisualTreeHelper.GetParent(dep);
        }

        if (dep is not DataGridRow row)
            return;

        OptionsDataGrid.SelectedItem = row.DataContext;
    }
}
