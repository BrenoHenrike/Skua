using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;
using Skua.Core.Utils;

namespace Skua.WPF;
public class TextBoxOnlyNumbersBehavior : Behavior<TextBox>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.PreviewTextInput += AssociatedObject_PreviewTextInput;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.PreviewTextInput -= AssociatedObject_PreviewTextInput;
        base.OnDetaching();
    }

    private void AssociatedObject_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
    {
        if (!e.Text.IsNumber())
            e.Handled = true;
    }
}
