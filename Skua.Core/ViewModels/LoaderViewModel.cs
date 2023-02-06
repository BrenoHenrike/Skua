using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skua.Core.Interfaces;
using Skua.Core.Models.Quests;
using Skua.Core.Utils;

namespace Skua.Core.ViewModels;
public partial class LoaderViewModel : BotControlViewModelBase, IManagedWindow
{
    public LoaderViewModel(IScriptShop shops, IScriptQuest quests, IQuestDataLoaderService questLoader, IClipboardService clipboardService)
        : base("Loader", 550, 270)
    {
        _shops = shops;
        _quests = quests;
        _questLoader = questLoader;
        _clipboardService = clipboardService;
    }

    private CancellationTokenSource? _loaderCTS;
    private readonly IScriptShop _shops;
    private readonly IScriptQuest _quests;
    private readonly IQuestDataLoaderService _questLoader;
    private readonly IClipboardService _clipboardService;
    [ObservableProperty]
    private string _progressReport = string.Empty;
    [ObservableProperty]
    private bool _isLoading;
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoadCommand))]
    private string _inputIDs = string.Empty;
    [ObservableProperty]
    private int _selectedIndex;
    [ObservableProperty]
    private RangedObservableCollection<QuestData> _questIDs = new();

    [RelayCommand(CanExecute = nameof(AllDigits))]
    private void Load()
    {
        if (SelectedIndex == 0 && int.TryParse(InputIDs, out int id))
        {
            Task.Factory.StartNew(() => _shops.Load(id));
            return;
        }
        if (SelectedIndex == 1)
        {
            Task.Factory.StartNew(() => _quests.Load(InputIDs.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToArray()));
        }
    }
    private bool AllDigits()
    {
        return InputIDs.Replace(",", "").Replace(" ", "").All(c => int.TryParse(c + "", out int i));
    }

    [RelayCommand]
    private void LoadQuests(IList<object>? items)
    {
        if (items is null)
            return;
        IEnumerable<QuestData> quests = items.Cast<QuestData>();
        _quests.Load(quests.Select(q => q.ID).ToArray());
    }

    [RelayCommand]
    private void CopyQuestsNames(IList<object>? items)
    {
        if (items is null)
            return;

        IEnumerable<QuestData> quests = items.Cast<QuestData>();
        _clipboardService.SetText(string.Join(",", quests.Select(q => q.Name)));
    }

    [RelayCommand]
    private void CopyQuestsIDs(IList<object>? items)
    {
        if (items is null)
            return;

        IEnumerable<QuestData> quests = items.Cast<QuestData>();
        _clipboardService.SetText(string.Join(",", quests.Select(q => q.ID)));
    }

    [RelayCommand]
    private async Task UpdateQuests(bool getAll)
    {
        _loaderCTS = new();
        QuestIDs.Clear();
        Progress<string> progress = new(progress =>
        {
            IsLoading = true;
            ProgressReport = progress;
        });
        List<QuestData> questData = await _questLoader.UpdateAsync("Quests.txt", getAll, progress, _loaderCTS.Token);
        QuestIDs.Clear();
        QuestIDs.AddRange(questData);
        IsLoading = false;
        ProgressReport = string.Empty;
        _loaderCTS.Dispose();
        _loaderCTS = null;
    }

    [RelayCommand]
    private async Task GetQuests()
    {
        IsLoading = true;
        ProgressReport = "Getting quests";
        QuestIDs.Clear();
        QuestIDs.AddRange(await _questLoader.GetFromFileAsync("Quests.txt"));
        ProgressReport = string.Empty;
        IsLoading = false;
    }

    [RelayCommand]
    private void CancelQuestLoad()
    {
        if (_loaderCTS is not null)
        {
            _loaderCTS.Cancel();
            ProgressReport = "Cancelling task...";
        }
    }
}
