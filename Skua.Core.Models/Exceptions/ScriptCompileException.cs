namespace Skua.Core.Models;
public class ScriptCompileException : Exception
{
    public string CompiledScript { get; }
    public ScriptCompileException(string? message, string compiledScript)
        : base(message)
    {
        CompiledScript = compiledScript;
    }
}
