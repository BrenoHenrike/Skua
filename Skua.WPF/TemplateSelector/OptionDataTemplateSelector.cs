using System.Windows;
using System.Windows.Controls;
using Skua.Core.Models;
using Skua.Core.ViewModels;

namespace Skua.WPF;
public class OptionDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate BoolTemplate { get; set; }
    public DataTemplate NumericTemplate { get; set; }
    public DataTemplate ButtonTemplate { get; set; }
    public DataTemplate TextTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        DataTemplate selectedTemplate = BoolTemplate;

        OptionDisplayType displayType = (item as OptionItemViewModel)?.DisplayType ?? OptionDisplayType.CheckBox;

        switch (displayType)
        {
            case OptionDisplayType.CheckBox:
                selectedTemplate = BoolTemplate;
                break;
            case OptionDisplayType.NumericAndButton:
                selectedTemplate = NumericTemplate;
                break;
            case OptionDisplayType.Button:
                selectedTemplate = ButtonTemplate;
                break;
            case OptionDisplayType.Text:
                selectedTemplate = TextTemplate;
                break;
        }

        return selectedTemplate;
    }
}
