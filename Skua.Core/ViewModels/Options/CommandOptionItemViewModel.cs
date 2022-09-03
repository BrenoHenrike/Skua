using CommunityToolkit.Mvvm.Input;

namespace Skua.Core.ViewModels;
public class CommandOptionItemViewModel<TDisplay> : CommandOptionItemViewModel
{
    public CommandOptionItemViewModel(string content, IRelayCommand command) : base(content, command, typeof(TDisplay))
    {
        Value = default(TDisplay);
    }

    public CommandOptionItemViewModel(string content, string tag, IRelayCommand command) : base(content, tag, command, typeof(TDisplay))
    {
        Value = default(TDisplay);
    }

    public CommandOptionItemViewModel(string content, string tag, IRelayCommand command, TDisplay? defaultValue) : base(content, tag, command, typeof(TDisplay))
    {
        Value = defaultValue;
    }

    public CommandOptionItemViewModel(string content, string description, string tag, IRelayCommand command, TDisplay? defaultValue) : base(content, description, tag, command, typeof(TDisplay))
    {
        Value = defaultValue;
    }

    public CommandOptionItemViewModel(string content, string description, string tag, IRelayCommand command) : base(content, description, tag, command, typeof(TDisplay))
    {
        Value = default(TDisplay);
    }
}

public class CommandOptionItemViewModel : DisplayOptionItemViewModelBase
{
    public CommandOptionItemViewModel(string content, IRelayCommand command, Type displayType) : base(content, displayType)
    {
        Command = command;
    }

    public CommandOptionItemViewModel(string content, string tag, IRelayCommand command, Type displayType) : base(content, tag, displayType)
    {
        Command = command;
    }

    public CommandOptionItemViewModel(string content, string description, string tag, IRelayCommand command, Type displayType) : base(content, description, tag, displayType)
    {
        Command = command;
    }

    public IRelayCommand Command { get; }
}
