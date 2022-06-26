using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Skua.Core.ViewModels;
public class BotControlViewModelBase : ObservableRecipient
{
    public string Title { get; }

    public BotControlViewModelBase(string title)
    {
        Title = title;
    }
}
