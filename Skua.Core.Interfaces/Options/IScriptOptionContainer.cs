namespace Skua.Core.Interfaces;
public interface IScriptOptionContainer : IOptionContainer
{

    /// <summary>
    /// The name of the file used to store this scripts options. This should be unique to your script to prevent option name clashes.
    /// </summary>
    /// <remarks>Transient options are reset when the script is restarted (including auto-relogins).</remarks>
    string Storage { get; set; }
}
