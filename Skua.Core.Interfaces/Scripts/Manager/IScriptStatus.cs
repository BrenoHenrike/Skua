namespace Skua.Core.Interfaces;
public interface IScriptStatus
{
    /// <summary>
    /// Whether the current script should terminate.
    /// </summary>
    bool ShouldExit { get; }
    /// <summary>
    /// Whether the script is running.
    /// </summary>
    bool ScriptRunning { get; }
    /// <summary>
    /// Path to the currently loaded script
    /// </summary>
    string LoadedScript { get; }
    /// <summary>
    /// The last script compiled.
    /// </summary>
    string CompiledScript { get; }
    IScriptOptionContainer? Config { get; set; }

    event Action? ScriptStarted;
    event Action<bool>? ScriptStopped;
    event Action<Exception>? ScriptError;

    Task RestartScriptAsync();
    void StopScript(bool runScriptStoppingEvent = true);
    ValueTask StopScriptAsync(bool runScriptStoppingEvent = true);
}
