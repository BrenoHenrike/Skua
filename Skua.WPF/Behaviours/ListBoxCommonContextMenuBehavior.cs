using System.Windows.Controls;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Xaml.Behaviors;

namespace Skua.WPF;
public class ListBoxUnselectAllMenuBehavior : Behavior<ListBox>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        MenuItem unselectAll = new()
        {
            Header = "Unselect All",
            Command = new RelayCommand(AssociatedObject.UnselectAll)
        };
        if(AssociatedObject.ContextMenu is null)
            AssociatedObject.ContextMenu = new ContextMenu();
        AssociatedObject.ContextMenu.Items.Add(unselectAll);
    }
}
