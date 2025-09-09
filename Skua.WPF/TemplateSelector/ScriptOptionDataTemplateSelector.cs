using Skua.Core.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Skua.WPF;

public class OptionContainerDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate BoolTemplate { get; set; }
    public DataTemplate EnumTemplate { get; set; }
    public DataTemplate StringTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        DataTemplate selectedTemplate = StringTemplate;

        Type? displayType = (item as OptionContainerItemViewModel)?.Type;

        if (displayType is null)
            return selectedTemplate;

        if (displayType.IsEnum)
            selectedTemplate = EnumTemplate;

        if (displayType == typeof(bool))
            selectedTemplate = BoolTemplate;

        return selectedTemplate;
    }
}