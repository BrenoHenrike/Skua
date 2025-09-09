using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;

namespace Skua.WPF;

public class ListBoxScrollToSelectedIndexBehavior : Behavior<ListBox>
{
    private void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not ListBox listBox || listBox.SelectedItem is null)
            return;

        listBox.Dispatcher.BeginInvoke(() =>
        {
            listBox.UpdateLayout();
            if (listBox.SelectedItem != null)
                listBox.ScrollIntoView(listBox.SelectedItem);
        });
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.SelectionChanged -= AssociatedObject_SelectionChanged;
        base.OnDetaching();
    }
}