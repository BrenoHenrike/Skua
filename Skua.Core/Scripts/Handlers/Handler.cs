using Skua.Core.Interfaces;

namespace Skua.Core.Scripts;

public class Handler : IHandler
{
    public Handler(string name, int ticks, Func<IScriptInterface, bool> function)
    {
        Name = name;
        Ticks = ticks;
        Function = function;
    }
    public string Name { get; }
    public int Ticks { get; }
    public Func<IScriptInterface, bool> Function { get; }
}
