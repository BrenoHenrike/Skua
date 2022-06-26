using Skua.Core.Interfaces;

namespace Skua.Core.Scripts;

public class ScriptHandlers : IScriptHandlers
{
    private List<IHandler> _currentHandlers = new();
    public IEnumerable<IHandler> CurrentHandlers => _currentHandlers;
    private volatile int _iHandler;
    public IHandler RegisterHandler(int ticks, Func<IScriptInterface, bool> function, string name = null!)
    {
        string hName = name ?? _iHandler++.ToString();
        Handler handler = new(hName, ticks, function);
        _currentHandlers.Add(handler);
        return handler;
    }

    public bool Remove(string name)
    {
        return _currentHandlers.RemoveAll(h => h.Name == name) > 0;
    }

    public bool Remove(IHandler handler)
    {
        return _currentHandlers.Remove(handler);
    }

    public bool Remove(List<IHandler> handlers)
    {
        return _currentHandlers.RemoveAll(h => handlers.Contains(h)) == handlers.Count;
    }

    public void Clear()
    {
        _currentHandlers.Clear();
    }
}
