using Skua.Core.Interfaces;

namespace Skua.Core.Scripts;
public class ScriptManager : IScriptManager
{
    public bool ScriptRunning { get; set; }
    public string LoadedScript { get; set; } = string.Empty;
    public string CompiledScript { get; set; } = string.Empty;

    public event Action? ScriptStarted;
    public event Action<bool>? ScriptStopped;
    public event Action<Exception>? ScriptError;

    public object? Compile(string script)
    {
        throw new NotImplementedException();
    }

    public Task<Exception> RestartScriptAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Exception> StartScriptAsync()
    {
        throw new NotImplementedException();
    }

    public void StopScript()
    {
        throw new NotImplementedException();
    }
}