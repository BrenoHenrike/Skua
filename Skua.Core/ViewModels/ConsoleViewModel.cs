using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Input;
using Skua.Core.Interfaces;
using Skua.Core.Models;
using Westwind.Scripting;

namespace Skua.Core.ViewModels;
public class ConsoleViewModel : BotControlViewModelBase
{
    public ConsoleViewModel(CSharpScriptExecution compiler, IDialogService dialogService, IScriptManager scriptManager, IScriptInterface bot)
        : base("Console")
    {
        _compiler = compiler;
        _dialogService = dialogService;
        ScriptManager = scriptManager;
        Bot = bot;

        RunCommand = new AsyncRelayCommand(Run);
    }
    private readonly CSharpScriptExecution _compiler;
    private readonly IDialogService _dialogService;
    private readonly IScriptManager ScriptManager;
    private readonly IScriptInterface Bot;
    private string _snippetText = "bot.Log(\"Test\");";

    public string SnippetText
    {
        get { return _snippetText; }
        set { SetProperty(ref _snippetText, value); }
    }

    public IAsyncRelayCommand RunCommand { get; }
    private Task Run()
    {
        try
        {
            //_compiler.ExecuteMethod($"public object Snippet(IScriptInterface bot){{\n{SnippetText}\nreturn null;}}", "Snippet", Bot);
            string source = $"using Skua.Core; using Skua.Core.Interfaces; public class Script{{ public void ScriptMain(IScriptInterface bot){{{_snippetText}}}";
            object? o = ScriptManager.Compile(source);
            o!.GetType().GetMethod("ScriptMain")!.Invoke(o, new[] { Bot });
        }
        catch (Exception e)
        {
            _dialogService.ShowMessageBox($"Error running snippet:\r\n{e.InnerException?.Message ?? e.Message}\r\nStackTrace: {e.InnerException?.StackTrace ?? e.StackTrace}", "Error");
        }
        return Task.CompletedTask;
    }
}
