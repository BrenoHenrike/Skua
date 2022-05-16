using Skua.Core.Interfaces;

namespace Skua.Core.Scripts;

public class ScriptHandlers : IScriptHandlers
{
    public List<IHandler> CurrentHandlers { get; private set; } = new();
    private volatile int _iHandler;
    public IHandler RegisterHandler(int ticks, Func<IScriptInterface, bool> function, string name = null!)
    {
        string hName = name ?? _iHandler++.ToString();
        Handler handler = new(hName, ticks, function);
        CurrentHandlers.Add(handler);
        return handler;
    }
}
