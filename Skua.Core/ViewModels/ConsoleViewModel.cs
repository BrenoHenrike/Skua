using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skua.Core.Interfaces;

namespace Skua.Core.ViewModels;

public partial class ConsoleViewModel : BotControlViewModelBase
{
    private const string _source =
        @"using Skua.Core;
using Skua.Core.Interfaces;
using Skua.Core.Utils;
using Skua.Core.Models;
using Skua.Core.Models.Items;
using Skua.Core.Models.Monsters;
using Skua.Core.Models.Players;
using Skua.Core.Models.Quests;
using Skua.Core.Models.Servers;
using Skua.Core.Models.Shops;
using Skua.Core.Models.Skills;
using Newtonsoft.Json;
public class Script{ public void ScriptMain(IScriptInterface bot){";

    public ConsoleViewModel(IDialogService dialogService, IScriptManager scriptManager)
        : base("Console", 700, 400)
    {
        _dialogService = dialogService;
        _scriptManager = scriptManager;
    }

    private readonly IDialogService _dialogService;
    private readonly IScriptManager _scriptManager;

    [ObservableProperty]
    private string _snippetText = "bot.Log(\"Test\");";

    [RelayCommand]
    private async Task Run()
    {
        await Task.Run(() =>
        {
            try
            {
                object? o = _scriptManager.Compile($"{_source}{_snippetText}}}}}");
                o!.GetType().GetMethod("ScriptMain")!.Invoke(o, new[] { IScriptInterface.Instance });
            }
            catch (Exception e)
            {
                _dialogService.ShowMessageBox($"Error running snippet:\r\n{e.InnerException?.Message ?? e.Message}\r\nStackTrace: {e.InnerException?.StackTrace ?? e.StackTrace}", "Error");
            }
        });
    }
}