using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;

namespace Skua.WPF;
public class ListBoxScrollToCaretBehavior : Behavior<ListBox>
{
    private ScrollViewer? scrollViewer;
    private bool isScrollDownEnabled;

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.Loaded += OnLoaded;
        AssociatedObject.Unloaded += OnUnLoaded;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.Loaded -= OnLoaded;
        AssociatedObject.Unloaded -= OnUnLoaded;
        base.OnDetaching();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (AssociatedObject.ItemsSource is not INotifyCollectionChanged incc)
            return;
        incc.CollectionChanged += OnCollectionChanged;
        if (VisualTreeHelper.GetChildrenCount(AssociatedObject) > 0)
        {
            Border border = (Border)VisualTreeHelper.GetChild(AssociatedObject, 0);
            scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
        }
    }

    private void OnUnLoaded(object? sender, RoutedEventArgs e)
    {
        if (AssociatedObject.ItemsSource is not INotifyCollectionChanged incc)
            return;

        incc.CollectionChanged -= OnCollectionChanged;
    }

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (scrollViewer is null)
            return;

        isScrollDownEnabled = scrollViewer.ScrollableHeight > 0 && scrollViewer.VerticalOffset + scrollViewer.ViewportHeight < scrollViewer.ExtentHeight;

        if (e.Action == NotifyCollectionChangedAction.Add && !isScrollDownEnabled)
            scrollViewer.ScrollToBottom();
    }
}
