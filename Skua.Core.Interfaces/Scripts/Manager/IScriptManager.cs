using System.ComponentModel;

namespace Skua.Core.Interfaces;
public interface IScriptManager : IScriptStatus, INotifyPropertyChanged
{
    public CancellationTokenSource? ScriptCTS { get; }
    Task<Exception?> StartScriptAsync();
    object? Compile(string source);
    void LoadScriptConfig(object? script);
    void SetLoadedScript(string path);
}
