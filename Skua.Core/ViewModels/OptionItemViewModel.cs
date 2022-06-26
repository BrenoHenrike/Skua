using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Skua.Core.Models;
using Skua.Core.Interfaces;
using System.ComponentModel;

namespace Skua.Core.ViewModels;
public class OptionItemViewModel : ObservableObject, IDisposable
{
    public OptionItemViewModel(string binding, string content, IScriptOption options, IRelayCommand command, OptionDisplayType displayType = OptionDisplayType.CheckBox)
    {
        DisplayType = displayType;
        Content = content;
        Command = command;
        Binding = binding;
        Options = options;
        _value = Options.OptionsDictionary[Binding].Invoke();
        Options.PropertyChanged += Option_PropertyChanged;
    }
    public OptionItemViewModel(string content, IRelayCommand command, OptionDisplayType displayType = OptionDisplayType.CheckBox)
    {
        DisplayType = displayType;
        Content = content;
        Command = command;
        switch (displayType)
        {
            case OptionDisplayType.CheckBox:
                _value = false;
                break;
            case OptionDisplayType.NumericAndButton:
                _value = 0;
                break;
            case OptionDisplayType.Button:
                _value = "X";
                break;
        }
    }
    private readonly IScriptOption? Options = null;
    private object? _value = null;
    public object? Value
    {
        get { return _value; }
        set { SetProperty(ref _value, value); }
    }
    public string Binding { get; } = string.Empty;
    public string Content { get; }
    public OptionDisplayType DisplayType { get; }
    public IRelayCommand Command { get; }

    private void Option_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == Binding)
            Value = Options?.OptionsDictionary[Binding].Invoke();
    }

    public void Dispose()
    {
        if(Options is not null)
        {
            GC.SuppressFinalize(this);
            Options.PropertyChanged -= Option_PropertyChanged;
        }
    }
}
