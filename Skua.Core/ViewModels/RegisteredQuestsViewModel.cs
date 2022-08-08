using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skua.Core.Interfaces;

namespace Skua.Core.ViewModels;
public class RegisteredQuestsViewModel : ObservableObject, IDisposable
{
    private readonly char[] _questsSeparator = { '|', ',', ' ' };
    public RegisteredQuestsViewModel(IScriptQuest quests)
    {
        _quests = quests;
        _quests.PropertyChanged += Quests_PropertyChanged;
        AddQuestCommand = new RelayCommand(AddQuest);
        RemoveQuestsCommand = new RelayCommand<IList<object>>(RemoveQuests);
        RemoveAllQuestsCommand = new RelayCommand(_quests.UnregisterAllQuests);
    }
    private readonly IScriptQuest _quests;
    private string _addQuestInput = string.Empty;
    public string AddQuestInput
    {
        get { return _addQuestInput; }
        set { SetProperty(ref _addQuestInput, value); }
    }
    public List<int> CurrentAutoQuests => _quests.Registered.ToList();
    public IRelayCommand AddQuestCommand { get; }
    public IRelayCommand RemoveAllQuestsCommand { get; }
    public IRelayCommand RemoveQuestsCommand { get; }
    private void RemoveQuests(IList<object>? items)
    {
        if (items is null)
            return;
        IEnumerable<int> quests = items.Cast<int>();
        if (quests.Any())
            _quests.UnregisterQuests(quests.ToArray());
    }

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

    private void Quests_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IScriptQuest.Registered))
            OnPropertyChanged(nameof(CurrentAutoQuests));
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _quests.PropertyChanged -= Quests_PropertyChanged;
    }
}
