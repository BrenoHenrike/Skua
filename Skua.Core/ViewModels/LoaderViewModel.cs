using Microsoft.Toolkit.Mvvm.Input;
using Skua.Core.Interfaces;
using Skua.Core.Models.Quests;
using Skua.Core.Utils;

namespace Skua.Core.ViewModels;
public class LoaderViewModel : BotControlViewModelBase, IManagedWindow
{
    public LoaderViewModel(IScriptShop shops, IScriptQuest quests, IQuestDataLoaderService questLoader, IClipboardService clipboardService)
        : base("Loader", 550, 270)
    {
        Shops = shops;
        Quests = quests;
        _questLoader = questLoader;
        _clipboardService = clipboardService;
        LoadCommand = new RelayCommand(Load, AllDigits);
        LoadQuestsCommand = new RelayCommand<IList<object>>(LoadQuests);
        CopyQuestsIDsCommand = new RelayCommand<IList<object>>(CopyQuestIDs);
        CopyQuestsNamesCommand = new RelayCommand<IList<object>>(CopyQuestNames);
        UpdateQuestsCommand = new AsyncRelayCommand<bool>(UpdateQuests);
        GetQuestsCommand = new AsyncRelayCommand(GetQuests);
        CancelQuestLoadCommand = new RelayCommand(() =>
        {
            if(_loaderCTS is not null)
            {
                _loaderCTS.Cancel();
                ProgressReport = "Cancelling task...";
            }
        });
    }

    private CancellationTokenSource? _loaderCTS;
    private readonly IScriptShop Shops;
    private readonly IScriptQuest Quests;
    private readonly IQuestDataLoaderService _questLoader;
    private readonly IClipboardService _clipboardService;
    private string _progressReport = string.Empty;
    public string ProgressReport
    {
        get { return _progressReport; }
        set { SetProperty(ref _progressReport, value); }
    }
    private bool _isLoading;
    public bool IsLoading
    {
        get { return _isLoading; }
        set { SetProperty(ref _isLoading, value); }
    }
    private string _inputIDs = string.Empty;
    public string InputIDs
    {
        get { return _inputIDs; }
        set
        {
            SetProperty(ref _inputIDs, value);
            LoadCommand.NotifyCanExecuteChanged();
        }
    }
    private int _selectedIndex;
    public int SelectedIndex
    {
        get { return _selectedIndex; }
        set { SetProperty(ref _selectedIndex, value); }
    }
    private RangedObservableCollection<QuestData> _questIDs = new();
    public RangedObservableCollection<QuestData> QuestIDs
    {
        get { return _questIDs; }
        set { SetProperty(ref _questIDs, value); }
    }

    public IRelayCommand LoadCommand { get; }
    public IRelayCommand LoadQuestsCommand { get; }
    public IRelayCommand CopyQuestsIDsCommand { get; }
    public IRelayCommand CopyQuestsNamesCommand { get; }
    public IAsyncRelayCommand UpdateQuestsCommand { get; }
    public IAsyncRelayCommand GetQuestsCommand { get; }
    public IRelayCommand CancelQuestLoadCommand { get; }

    private void Load()
    {
        if (SelectedIndex == 0 && int.TryParse(InputIDs, out int id))
        {
            Shops.Load(id);
            return;
        }
        if (SelectedIndex == 1)
        {
            Quests.Load(InputIDs.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToArray());
        }
    }
    private bool AllDigits()
    {
        return InputIDs.Replace(",", "").Replace(" ", "").All(c => int.TryParse(c + "", out int i));
    }
    private void LoadQuests(IList<object>? items)
    {
        if (items is null)
            return;
        IEnumerable<QuestData> quests = items.Cast<QuestData>();
        Quests.Load(quests.Select(q => q.ID).ToArray());
    }
    private void CopyQuestNames(IList<object>? items)
    {
        if (items is null)
            return;

        IEnumerable<QuestData> quests = items.Cast<QuestData>();
        _clipboardService.SetText(string.Join(",", quests.Select(q => q.Name)));
    }

    private void CopyQuestIDs(IList<object>? items)
    {
        if (items is null)
            return;

        IEnumerable<QuestData> quests = items.Cast<QuestData>();
        _clipboardService.SetText(string.Join(",", quests.Select(q => q.ID)));
    }

    private async Task UpdateQuests(bool getAll)
    {
        _loaderCTS = new CancellationTokenSource();
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

    private async Task GetQuests()
    {
        QuestIDs.Clear();
        ProgressReport = "Getting quests";
        QuestIDs.AddRange(await _questLoader.GetFromFileAsync("Quests.txt"));
        ProgressReport = string.Empty;
    }
}
