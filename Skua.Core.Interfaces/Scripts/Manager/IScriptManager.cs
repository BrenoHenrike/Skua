using System.ComponentModel;

namespace Skua.Core.Interfaces;

public interface IScriptManager : IScriptStatus, INotifyPropertyChanged
{
    CancellationTokenSource? ScriptCTS { get; }

    Task<Exception?> StartScriptAsync();

    object? Compile(string source);

    void LoadScriptConfig(object? script);

    void SetLoadedScript(string path);
}