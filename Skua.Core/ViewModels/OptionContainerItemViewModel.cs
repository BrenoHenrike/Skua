using Microsoft.Toolkit.Mvvm.ComponentModel;
using Skua.Core.Interfaces;
using Skua.Core.Options;

namespace Skua.Core.ViewModels;
public class OptionContainerItemViewModel : ObservableObject
{
    public OptionContainerItemViewModel(IOptionContainer container, IOption option)
    {
        Container = container;
        Option = option;
        Type = option.Type;
        if (Type.IsEnum)
        {
            EnumValues = Enum.GetNames(Type).Select(s => s.Replace('_', ' ')).ToList();
            SelectedValue = GetValue().ToString()!.Replace('_', ' ');
            return;
        }
        _value = GetValue();
        Category = option.Category;
    }

    public IOptionContainer Container { get; }
    public IOption Option { get; }
    public Type Type { get; }
    public string Category { get; }

    private object _value;
    public object Value
    {
        get { return _value; }
        set { SetProperty(ref _value, value); }
    }
    private List<string>? _enumValues;
    public List<string>? EnumValues
    {
        get { return _enumValues; }
        set { SetProperty(ref _enumValues, value); }
    }

    private string? _selectedValue;
    public string? SelectedValue
    {
        get { return _selectedValue; }
        set { SetProperty(ref _selectedValue, value); }
    }

    private object GetValue()
    {
        object value = typeof(OptionContainer).GetMethod("Get", new Type[] { typeof(IOption) })?
                .MakeGenericMethod(new Type[] { Option.Type })
                .Invoke(Container, new object[] { Option }) ?? string.Empty;
        return value;
    }
}
