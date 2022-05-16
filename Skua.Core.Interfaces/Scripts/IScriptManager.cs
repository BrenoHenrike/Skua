namespace Skua.Core.Interfaces;
public interface IScriptManager
{
    /// <summary>
    /// Whether the script is running.
    /// </summary>
    bool ScriptRunning { get; set; }
    /// <summary>
    /// Path to the currently loaded script
    /// </summary>
    string LoadedScript { get; set; }
    /// <summary>
    /// The last script compiled.
    /// </summary>
    string CompiledScript { get; set; }

    event Action? ScriptStarted;
    event Action<bool>? ScriptStopped;
    event Action<Exception>? ScriptError;

    Task<Exception> StartScriptAsync();
    Task<Exception> RestartScriptAsync();
    void StopScript();
    object? Compile(string script);
}
