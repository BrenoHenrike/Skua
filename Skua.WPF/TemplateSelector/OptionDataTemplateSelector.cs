using System;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Input;
using Skua.Core.Models;
using Skua.Core.ViewModels;

namespace Skua.WPF;
public class OptionDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate BoolTemplate { get; set; }
    public DataTemplate IntTemplate { get; set; }
    public DataTemplate StringTemplate { get; set; }
    public DataTemplate EnumTemplate { get; set; }
    public DataTemplate ActionTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        DataTemplate selectedTemplate = BoolTemplate;

        if (item is not DisplayOptionItemViewModelBase vm)
            return selectedTemplate;

        if (vm.DisplayType == typeof(bool))
            selectedTemplate = BoolTemplate;
        else if (vm.DisplayType == typeof(string))
            selectedTemplate = StringTemplate;
        else if(vm.DisplayType == typeof(int))
            selectedTemplate = IntTemplate;
        else if(vm.DisplayType.IsEnum)
            selectedTemplate = EnumTemplate;
        else if(vm.DisplayType == typeof(IRelayCommand))
            selectedTemplate = ActionTemplate;

        //switch(item)
        //{
        //    case BoolOptionItemViewModel:
        //    case BoolBindingOptionItemViewModel:
        //    case BoolSettingsOptionItemViewModel:
        //        selectedTemplate = BoolTemplate;
        //        break;
        //    case IntSettingsOptionItemViewModel:
        //    case IntBindingOptionItemViewModel:
        //        selectedTemplate = IntTemplate;
        //        break;
        //    case StringSettingsOptionItemViewModel:
        //    case StringBindingOptionItemViewModel:
        //        selectedTemplate = StringTemplate;
        //        break;
        //    case CommandOptionItemViewModel:
        //        selectedTemplate = ActionTemplate;
        //        break;
        //}

        //OptionDisplayType displayType = (item as OptionItemViewModel)?.DisplayType ?? OptionDisplayType.CheckBox;

        //switch (displayType)
        //{
        //    case OptionDisplayType.CheckBox:
        //        selectedTemplate = BoolTemplate;
        //        break;
        //    case OptionDisplayType.NumericAndButton:
        //        selectedTemplate = NumericTemplate;
        //        break;
        //    case OptionDisplayType.Button:
        //        selectedTemplate = ButtonTemplate;
        //        break;
        //    case OptionDisplayType.Text:
        //        selectedTemplate = TextTemplate;
        //        break;
        //}

        return selectedTemplate;
    }
}
