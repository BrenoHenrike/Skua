using CommunityToolkit.Mvvm.ComponentModel;
using System.Text;

namespace Skua.Core.ViewModels;
public class CBOptionsViewModel : ObservableObject, IManageCBOptions
{
    public CBOptionsViewModel(List<DisplayOptionItemViewModelBase> options)
    {
        Options = options;
        DefaultValues = new();
        foreach (DisplayOptionItemViewModelBase option in Options)
            DefaultValues.Add(option.Tag, option.Value!);
    }

    public List<DisplayOptionItemViewModelBase> Options { get; }

    private Dictionary<string, object> DefaultValues { get; }

    public StringBuilder Save(StringBuilder builder)
    {
        foreach(DisplayOptionItemViewModelBase option in Options)
            builder.AppendLine($"{option.Tag}: {option.Value}");

        return builder;
    }

    public void SetValues(Dictionary<string, string> values)
    {
        foreach(DisplayOptionItemViewModelBase option in Options)
        {
            if(values.TryGetValue(option.Tag, out string? value) && !string.IsNullOrWhiteSpace(value))
            {
                option.Value = Convert.ChangeType(value, option.DisplayType);
                continue;
            }
            option.Value = DefaultValues[option.Tag];
        }
    }
}
