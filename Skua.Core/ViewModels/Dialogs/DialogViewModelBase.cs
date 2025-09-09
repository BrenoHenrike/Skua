using CommunityToolkit.Mvvm.ComponentModel;

namespace Skua.Core.ViewModels;

public class DialogViewModelBase : ObservableRecipient
{
    public string Title { get; }

    public DialogViewModelBase(string title)
    {
        Title = title;
    }
}