using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Skua.Core.Interfaces;

namespace Skua.Core.ViewModels;
public partial class RegisteredQuestsViewModel : ObservableObject
{
    private readonly char[] _questsSeparator = { '|', ',', ' ' };
    public RegisteredQuestsViewModel(IScriptQuest quests)
    {
        WeakReferenceMessenger.Default.Register<RegisteredQuestsViewModel, PropertyChangedMessage<IEnumerable<int>>>(this, RegisteredChanged);

        _quests = quests;
        RemoveAllQuestsCommand = new RelayCommand(_quests.UnregisterAllQuests);
    }

    private readonly IScriptQuest _quests;
    [ObservableProperty]
    private string _addQuestInput = string.Empty;

    public List<int> CurrentAutoQuests => _quests.Registered.ToList();
    public IRelayCommand RemoveAllQuestsCommand { get; }

    [RelayCommand]
    private void RemoveQuests(IList<object>? items)
    {
        if (items is null)
            return;
        IEnumerable<int> quests = items.Cast<int>();
        if (quests.Any())
            _quests.UnregisterQuests(quests.ToArray());
    }

    [RelayCommand]
    private void AddQuest()
    {
        if (string.IsNullOrWhiteSpace(AddQuestInput))
            return;
        if (!AddQuestInput.Replace(",", "").Replace("|", "").Replace(" ", "").All(char.IsDigit))
            return;

        IEnumerable<int> quests = AddQuestInput.Split(_questsSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(s => int.Parse(s));
        if (quests.Any())
            _quests.RegisterQuests(quests.ToArray());

        AddQuestInput = string.Empty;
    }

    private void RegisteredChanged(RegisteredQuestsViewModel recipient, PropertyChangedMessage<IEnumerable<int>> message)
    {
        if (message.PropertyName == nameof(IScriptQuest.Registered))
            recipient.OnPropertyChanged(nameof(recipient.CurrentAutoQuests));
    }
}
