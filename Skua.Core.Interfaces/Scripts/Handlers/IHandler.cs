namespace Skua.Core.Interfaces;
public interface IHandler
{
    Func<IScriptInterface, bool> Function { get; }
    string Name { get; }
    int Ticks { get; }
}
