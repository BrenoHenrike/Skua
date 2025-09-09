namespace Skua.Core.Interfaces;

public interface IPluginManager
{
    /// <summary>
    /// Gets a list of currently loaded plugins' containers.
    /// </summary>
    List<IPluginContainer> Containers { get; }

    /// <summary>
    /// Loads all the plugins in the plugins folder.
    /// </summary>
    void Initialize();

    /// <summary>
    /// Loads the plugin at the given path.
    /// </summary>
    /// <param name="path">The path to the plugin assembly.</param>
    /// <returns>Whether the plugin was loaded successfully or not.</returns>
    Exception? Load(string path);

    /// <summary>
    /// Unloads the given plugin.
    /// </summary>
    /// <param name="plugin">The plugin to unload.</param>
    void Unload(ISkuaPlugin plugin);

    /// <summary>
    /// Unloads the plugin by it's given name.
    /// </summary>
    /// <param name="pluginName">The name o the plugin to unload.</param>
    void Unload(string pluginName);

    /// <summary>
    /// Gets the container for the given plugin.
    /// </summary>
    /// <param name="plugin">The plugin to get the container for.</param>
    /// <returns>The plugin's container.</returns>
    public IPluginContainer? GetContainer(ISkuaPlugin plugin);

    /// <summary>
    /// Gets the container for the given plugin name.
    /// </summary>
    /// <param name="pluginName">Name of the plugin to get the container for.</param>
    /// <returns>The plugin's container</returns>
    public IPluginContainer? GetContainer(string pluginName);

    /// <summary>
    /// Gets the container for the plugin with the given type.
    /// </summary>
    /// <typeparam name="T">The type of the plugin to get the container for.</typeparam>
    /// <returns>The container for the plugin with the given type.</returns>
    IPluginContainer GetContainer<T>() where T : ISkuaPlugin;
}