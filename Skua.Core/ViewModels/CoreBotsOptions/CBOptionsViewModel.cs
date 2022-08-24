using CommunityToolkit.Mvvm.ComponentModel;
using Skua.Core.Interfaces;
using System.Text;

namespace Skua.Core.ViewModels;
public class CBOptionsViewModel : ObservableObject, IManageCBOptions
{
    private readonly IDialogService _dialogService;

    public CBOptionsViewModel(List<DisplayOptionItemViewModelBase> options, IDialogService dialogService)
    {
        Options = options;
        _dialogService = dialogService;
        DefaultValues = new();
        foreach (DisplayOptionItemViewModelBase option in Options)
            DefaultValues.Add(option.Tag, option.Value!);
    }

    public List<DisplayOptionItemViewModelBase> Options { get; }

    private Dictionary<string, object> DefaultValues { get; }

    public StringBuilder Save(StringBuilder builder)
    {
        foreach(DisplayOptionItemViewModelBase option in Options)
        {
            if(option.Tag == "PrivateRooms" && (bool)option.Value == false && _dialogService.ShowMessageBox("Whilst we do offer the option, we highly recommend staying in private rooms while botting. Bot in public at your own risk.\r\n Confirm the use of Public Rooms?", "Public Room Warning", true) == false)
            {
                builder.AppendLine($"{option.Tag}: {true}");
                continue;
            }
            builder.AppendLine($"{option.Tag}: {option.Value}");
        }

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
