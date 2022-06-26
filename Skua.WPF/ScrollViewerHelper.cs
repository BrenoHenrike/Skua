using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System;

namespace Skua.WPF;

public static class ScrollViewerHelper
{
    public static readonly DependencyProperty ShiftScrollHorizontallyProperty
        = DependencyProperty.RegisterAttached("ShiftWheelScrollsHorizontally",
            typeof(bool),
            typeof(ScrollViewerHelper),
            new PropertyMetadata(false, ShiftScrollChangedCallback));

    public static readonly DependencyProperty ScrollHorizontallyProperty
        = DependencyProperty.RegisterAttached("ScrollHorizontally",
            typeof(bool),
            typeof(ScrollViewerHelper),
            new PropertyMetadata(false, HorizontalScrollChangedCallback));

    private static void ShiftScrollChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var element = d as UIElement;

        if (element == null)
            throw new Exception("Attached property must be used with UIElement.");

        if ((bool)e.NewValue)
            element.PreviewMouseWheel += ShiftScroll_OnPreviewMouseWheel;
        else
            element.PreviewMouseWheel -= ShiftScroll_OnPreviewMouseWheel;
    }

    private static void ShiftScroll_OnPreviewMouseWheel(object sender, MouseWheelEventArgs args)
    {
        var scrollViewer = ((UIElement)sender).FindDescendant<ScrollViewer>();

        if (scrollViewer == null)
            return;

        if (Keyboard.Modifiers != ModifierKeys.Shift)
            return;

        if (args.Delta < 0)
            scrollViewer.LineRight();
        else
            scrollViewer.LineLeft();

        args.Handled = true;
    }

    private static void HorizontalScrollChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var element = d as UIElement;

        if (element == null)
            throw new Exception("Attached property must be used with UIElement.");

        if ((bool)e.NewValue)
            element.PreviewMouseWheel += HorizontalScrool_OnPreviewMouseWheel;
        else
            element.PreviewMouseWheel -= HorizontalScrool_OnPreviewMouseWheel;
    }




    private static void HorizontalScrool_OnPreviewMouseWheel(object sender, MouseWheelEventArgs args)
    {
        var scrollViewer = ((UIElement)sender).FindDescendant<ScrollViewer>();

        if (scrollViewer == null)
            return;

        if (args.Delta < 0)
            scrollViewer.LineRight();
        else
            scrollViewer.LineLeft();

        args.Handled = true;
    }

    public static void SetShiftScrollHorizontally(UIElement element, bool value) => element.SetValue(ShiftScrollHorizontallyProperty, value);
    public static bool GetShiftScrollHorizontally(UIElement element) => (bool)element.GetValue(ShiftScrollHorizontallyProperty);

    public static void SetScrollHorizontally(UIElement element, bool value) => element.SetValue(ScrollHorizontallyProperty, value);
    public static bool GetScrollHorizontally(UIElement element) => (bool)element.GetValue(ScrollHorizontallyProperty);

    private static T FindDescendant<T>(this DependencyObject d) where T : DependencyObject
    {
        if (d == null)
            return null;

        var childCount = VisualTreeHelper.GetChildrenCount(d);

        for (var i = 0; i < childCount; i++)
        {
            var child = VisualTreeHelper.GetChild(d, i);

            var result = child as T ?? FindDescendant<T>(child);

            if (result != null)
                return result;
        }

        return null;
    }
}