using System;
using System.ComponentModel;
using System.Windows;

namespace Skua.WPF;

public class PropertyGridWindowManager
{
    public static readonly DependencyProperty OptionsProperty =
        DependencyProperty.RegisterAttached("Options", typeof(PropertyGridWindowOptions), typeof(PropertyGridWindowManager), new PropertyMetadata(PropertyGridWindowOptions.None, OnOptionsChanged));

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public static PropertyGridWindowOptions GetOptions(DependencyObject element)
    {
        if (element is null)
            throw new ArgumentNullException(nameof(element));

        return (PropertyGridWindowOptions)element.GetValue(OptionsProperty);
    }

    public static void SetOptions(DependencyObject element, PropertyGridWindowOptions value)
    {
        if (element == null)
            throw new ArgumentNullException("element");

        element.SetValue(OptionsProperty, value);
    }

    private static void OnOptionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
    }
}

[Flags]
public enum PropertyGridWindowOptions
{
    None = 0x0,
    UseDefinedSize = 0x1,
}