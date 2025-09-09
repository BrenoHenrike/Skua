namespace Skua.Core.ViewModels;

public class CBOBoolChoiceOptionItemViewModel : DisplayOptionItemViewModel<bool>
{
    public CBOBoolChoiceOptionItemViewModel(string optionTitle, string description, string tag, string firstChoice, string secondChoice)
        : base(optionTitle, description, tag)
    {
        FirstChoice = firstChoice;
        SecondChoice = secondChoice;
        Value = false;
    }

    public CBOBoolChoiceOptionItemViewModel(string optionTitle, string description, string tag, string firstChoice, string secondChoice, bool value)
        : base(optionTitle, description, tag)
    {
        FirstChoice = firstChoice;
        SecondChoice = secondChoice;
        Value = value;
    }

    public string FirstChoice { get; }
    public string SecondChoice { get; }
}