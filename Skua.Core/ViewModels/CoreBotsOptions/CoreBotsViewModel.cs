using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skua.Core.Interfaces;
using System.Text;

namespace Skua.Core.ViewModels;
public partial class CoreBotsViewModel : BotControlViewModelBase
{
    public CoreBotsViewModel(List<TabItemViewModel> tabs, IScriptPlayer player, IDialogService dialogService)
        : base("CoreBots Options")
    {
        CoreBotsTabs = tabs;
        _selectedTab = CoreBotsTabs[0];
        _player = player;
        _dialogService = dialogService;
    }

    private readonly IScriptPlayer _player;
    private readonly IDialogService _dialogService;
    private Dictionary<string, Dictionary<string, string>> _readValues = new(); 
    [ObservableProperty]
    private TabItemViewModel _selectedTab;

    public List<TabItemViewModel> CoreBotsTabs { get; }

    [RelayCommand]
    private void Save()
    {
        if (string.IsNullOrEmpty(_player.Username))
        {
            _dialogService.ShowMessageBox("Login first so that we can fetch your username for the save file", "Save");
            return;
        }

        StringBuilder bob = new();
        foreach(TabItemViewModel tab in CoreBotsTabs)
        {
            if (tab.Content is IManageCBOptions cbo)
                cbo.Save(bob);
        }
        Directory.CreateDirectory(AppContext.BaseDirectory + @"\options\");
        File.WriteAllText(AppContext.BaseDirectory + $@"\options\CBO_Storage({_player.Username}).txt", bob.ToString());
        _dialogService.ShowMessageBox($@"Saved to \options\CBO_Storage({_player.Username}).txt", "Save Successful!");
        _readValues[_player.Username] = ReadValues(File.ReadAllLines(AppContext.BaseDirectory + $@"\options\CBO_Storage({_player.Username}).txt"));
    }

    [RelayCommand]
    private void Load()
    {
        if (string.IsNullOrEmpty(_player.Username))
        {
            _dialogService.ShowMessageBox("Login first so that we can fetch your username to load file", "Load");
            return;
        }

        if (_readValues.ContainsKey(_player.Username))
        {
            SetValues(_readValues[_player.Username]);
            return;
        }

        if (!File.Exists(AppContext.BaseDirectory + $@"\options\CBO_Storage({_player.Username}).txt"))
            return;

        Dictionary<string, string> optionsDict = ReadValues(File.ReadAllLines(AppContext.BaseDirectory + $@"\options\CBO_Storage({_player.Username}).txt"));

        SetValues(optionsDict);

        _readValues.Add(_player.Username, optionsDict);
    }

    private Dictionary<string, string> ReadValues(IEnumerable<string> lines)
    {
        Dictionary<string, string> optionsDict = new();
        foreach (string option in lines)
        {
            ReadOnlySpan<string> value = option.Split(_separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (value.Length == 2)
                optionsDict.Add(value[0], value[1]);
        }
        return optionsDict;
    }

    private void SetValues(Dictionary<string, string> options)
    {
        foreach (TabItemViewModel tab in CoreBotsTabs)
        {
            if (tab.Content is IManageCBOptions setable)
                setable.SetValues(options);
        }
    }

    private readonly char _separator = ':';
}

internal interface IManageCBOptions
{
    StringBuilder Save(StringBuilder builder);
    void SetValues(Dictionary<string, string> values);
}
