namespace Skua.Core.ViewModels;

public class CBOBoolOptionItemViewModel : DisplayOptionItemViewModel<bool>
{
    public CBOBoolOptionItemViewModel(string optionTitle, string description, string tag)
        : base(optionTitle, description, tag)
    {
        Value = false;
    }

    public CBOBoolOptionItemViewModel(string optionTitle, string description, string tag, bool value)
        : base(optionTitle, description, tag)
    {
        Value = value;
    }

    public CBOBoolOptionItemViewModel(string optionTitle, string tag, bool value = false)
        : base(optionTitle, tag)
    {
        Value = value;
    }
}