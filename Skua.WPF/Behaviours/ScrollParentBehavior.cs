using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace Skua.WPF;
public sealed class ScrollParentBehavior : Behavior<UIElement>
{

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.PreviewMouseWheel += AssociatedObject_PreviewMouseWheel;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.PreviewMouseWheel -= AssociatedObject_PreviewMouseWheel;
        base.OnDetaching();
    }

    private void AssociatedObject_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        e.Handled = true;

        MouseWheelEventArgs e2 = new(e.MouseDevice, e.Timestamp, e.Delta)
        {
            RoutedEvent = UIElement.MouseWheelEvent
        };
        AssociatedObject.RaiseEvent(e2);

    }
}
