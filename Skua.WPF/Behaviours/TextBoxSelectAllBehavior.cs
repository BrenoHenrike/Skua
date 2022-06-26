using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace Skua.WPF;
public class TextBoxSelectAllBehavior : Behavior<TextBox>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.PreviewMouseLeftButtonDown += AssociatedObject_PreviewMouseLeftButtonDown;
        AssociatedObject.GotKeyboardFocus += AssociatedObject_GotKeyboardFocus;
        AssociatedObject.MouseDoubleClick += AssociatedObject_MouseDoubleClick;
    }

    private void AssociatedObject_MouseDoubleClick(object sender, MouseButtonEventArgs e) => SelectAll();

    private void AssociatedObject_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) => SelectAll();

    private void SelectAll()
    {
        AssociatedObject.SelectAll();
    }

    private void AssociatedObject_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if(!AssociatedObject.IsKeyboardFocusWithin)
        {
            AssociatedObject.Focus();
            e.Handled = true;
        }
    }

    protected override void OnDetaching()
    {
        AssociatedObject.PreviewMouseLeftButtonDown -= AssociatedObject_PreviewMouseLeftButtonDown;
        AssociatedObject.GotKeyboardFocus -= AssociatedObject_GotKeyboardFocus;
        AssociatedObject.MouseDoubleClick -= AssociatedObject_MouseDoubleClick;
        base.OnDetaching();
    }
}
