using System.ComponentModel;

namespace Skua.Core.Interfaces;
public interface IScriptManager : IScriptStatus, INotifyPropertyChanged
{
    bool ScriptRunning { get; }
    bool ScriptPaused { get; }
    void PauseScript();
    void ResumeScript();
    CancellationTokenSource? ScriptCTS { get; }
    Task<Exception?> StartScriptAsync();
    object? Compile(string source);
    void LoadScriptConfig(object? script);
    void SetLoadedScript(string path);
    void CheckPause();
}
