using Skua.Core.Models;

namespace Skua.Core.Interfaces;
public interface IPluginContainer
{
    /// <summary>
    /// A container managing this plugin's options.
    /// </summary>
    IOptionContainer OptionContainer { get; }
    /// <summary>
    /// This container's plugin.
    /// </summary>
    ISkuaPlugin Plugin { get; }
    /// <summary>
    /// The file at which the plugin's options are saved.
    /// </summary>
    string OptionsFile => Path.Combine(ClientFileSources.SkuaPluginsDIR, "options", $"{Plugin.OptionsStorage}.cfg");
}
