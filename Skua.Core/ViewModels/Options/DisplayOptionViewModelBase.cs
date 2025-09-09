using CommunityToolkit.Mvvm.ComponentModel;

namespace Skua.Core.ViewModels;

public class DisplayOptionItemViewModel<TDisplay> : DisplayOptionItemViewModelBase
{
    public DisplayOptionItemViewModel(string content, TDisplay? defaultValue = default)
        : base(content, typeof(TDisplay))
    {
        Value = defaultValue;
    }

    public DisplayOptionItemViewModel(string content, string tag, TDisplay? defaultValue = default)
        : base(content, tag, typeof(TDisplay))
    {
        Value = defaultValue;
    }

    public DisplayOptionItemViewModel(string content, string description, string tag, TDisplay? defaultValue = default)
        : base(content, description, tag, typeof(TDisplay))
    {
        Value = defaultValue;
    }
}

public partial class DisplayOptionItemViewModelBase : ObservableObject
{
    public DisplayOptionItemViewModelBase(string content, Type displayType)
    {
        Content = content;
        Description = content;
        Tag = content;
        DisplayType = displayType;
    }

    public DisplayOptionItemViewModelBase(string content, string tag, Type displayType)
    {
        Content = content;
        Description = content;
        Tag = tag;
        DisplayType = displayType;
    }

    public DisplayOptionItemViewModelBase(string content, string description, string tag, Type displayType)
    {
        Content = content;
        Description = description;
        Tag = tag;
        DisplayType = displayType;
    }

    public DisplayOptionItemViewModelBase(string content, string description, string tag, string? suffixText, Type displayType)
    {
        Content = content;
        Description = description;
        Tag = tag;
        SuffixText = suffixText;
        DisplayType = displayType;
    }

    public string Content { get; }
    public string Description { get; }
    public string Tag { get; }
    public string? SuffixText { get; set; }

    [ObservableProperty]
    private object? _value;

    public Type DisplayType { get; }
}