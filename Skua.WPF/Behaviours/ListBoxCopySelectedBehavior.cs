using Microsoft.Xaml.Behaviors;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Skua.WPF;

public class ListBoxCopySelectedBehavior : Behavior<ListBox>
{
    private readonly CommandBinding CopyCommand;

    public ListBoxCopySelectedBehavior()
    {
        static void handler(object sender, ExecutedRoutedEventArgs arg)
        {
            if (sender is not ListBox listBox)
                return;
            if (listBox.SelectedItems is not null)
            {
                StringBuilder bob = new();
                foreach (object item in listBox.SelectedItems)
                {
                    bob.AppendLine(item.ToString());
                }
                Clipboard.SetDataObject(bob.ToString());
            }
        }
        CopyCommand = new CommandBinding(ApplicationCommands.Copy, handler);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.CommandBindings.Add(CopyCommand);
    }

    protected override void OnDetaching()
    {
        AssociatedObject.CommandBindings.Remove(CopyCommand);
        base.OnDetaching();
    }
}