namespace Skua.Core.Interfaces;
public interface ISkuaPlugin
{
    /// <summary>
    /// The name of the plugin.
    /// </summary>
    string Name { get; }
    /// <summary>
    /// The author of the plugin.
    /// </summary>
    string Author { get; }
    /// <summary>
    /// The description of the plugin.
    /// </summary>
    string Description { get; }
    /// <summary>
    /// Indicates what file name the options of this plugin should be stored under. This needs to be unique to your plugin.
    /// </summary>
    string OptionsStorage => $"{Author}_{Name}".Replace(' ', '_').Trim();

    /// <summary>
    /// Called when the plugin is loaded.
    /// </summary>
    void Load(IServiceProvider provider, IPluginHelper helper);
    /// <summary>
    /// Called when the plugin is unloaded.
    /// </summary>
    void Unload();
    /// <summary>
    /// A list of options this plugin uses. This is only queried once, before Load is called.
    /// </summary>
    List<IOption>? Options { get; }
}
