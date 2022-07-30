using Microsoft.Toolkit.Mvvm.Input;
using Skua.Core.Interfaces;

namespace Skua.Core.ViewModels;
public class ConsoleViewModel : BotControlViewModelBase
{
    public ConsoleViewModel(IDialogService dialogService, IScriptManager scriptManager)
        : base("Console", 700, 400)
    {
        _dialogService = dialogService;
        _scriptManager = scriptManager;

        RunCommand = new AsyncRelayCommand(Run);
    }
    private readonly IDialogService _dialogService;
    private readonly IScriptManager _scriptManager;
    private string _snippetText = "bot.Log(\"Test\");";

    public string SnippetText
    {
        get { return _snippetText; }
        set { SetProperty(ref _snippetText, value); }
    }

    public IAsyncRelayCommand RunCommand { get; }

    private async Task Run()
    {
        await Task.Run(() =>
        {
            try
            {
                string source = 
                $@"
using Skua.Core;
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
public class Script{{ public void ScriptMain(IScriptInterface bot){{{_snippetText}}}}}";
                object? o = _scriptManager.Compile(source);
                o!.GetType().GetMethod("ScriptMain")!.Invoke(o, new[] { IScriptInterface.Instance });
            }
            catch (Exception e)
            {
                _dialogService.ShowMessageBox($"Error running snippet:\r\n{e.InnerException?.Message ?? e.Message}\r\nStackTrace: {e.InnerException?.StackTrace ?? e.StackTrace}", "Error");
            }
        });
    }
}
