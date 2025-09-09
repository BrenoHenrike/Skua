using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using Skua.Core.Models;
using Skua.Core.Utils;
using System.Diagnostics;
using System.Reflection;

namespace Skua.Core.Plugins;

public delegate void PluginLoadDelegate(IPluginContainer container);

public class PluginManager : IPluginManager
{
    private Dictionary<ISkuaPlugin, IPluginContainer> _plugins = new();

    public List<IPluginContainer> Containers => _plugins.Values.ToList();

    public void Initialize()
    {
        Directory.CreateDirectory(Path.Combine(ClientFileSources.SkuaPluginsDIR, "options"));
        Directory.GetFiles(ClientFileSources.SkuaPluginsDIR)
            .Where(f => f.EndsWith(".dll"))
            .ForEach(f =>
            {
                Exception? e = Load(f);
                if (e is not null)
                    Trace.WriteLine($"Error while loading plugin '{f}': {e.Message}");
            });
    }

    public Exception? Load(string path)
    {
        ISkuaPlugin? plugin = null;
        PluginContainer? container = null;

        try
        {
            Assembly asm = Assembly.LoadFrom(path);
            TypeInfo? type = asm.DefinedTypes.FirstOrDefault(t => typeof(ISkuaPlugin).IsAssignableFrom(t));
            if (type is null)
                return new Exception("Plugin class not found.");

            if (Activator.CreateInstance(type) is not ISkuaPlugin pluginInstance)
                return new Exception("Failed to create plugin instance.");

            plugin = pluginInstance;
            container = new PluginContainer(plugin);

            // Add to dictionary first to ensure proper tracking
            _plugins.Add(plugin, container);

            try
            {
                plugin.Load(Ioc.Default, Ioc.Default.GetRequiredService<IPluginHelper>());
                container.OptionContainer.SetDefaults();
                container.OptionContainer.Load();
                StrongReferenceMessenger.Default.Send<PluginLoadedMessage, int>(new(container), (int)MessageChannels.Plugins);
                return null;
            }
            catch (Exception loadException)
            {
                // If plugin.Load() fails, remove from dictionary to prevent partial state
                _plugins.Remove(plugin);

                // Attempt to unload the plugin gracefully
                try
                {
                    plugin.Unload();
                }
                catch
                {
                    // Ignore unload exceptions since loading already failed
                }

                throw new Exception($"Failed to initialize plugin: {loadException.Message}", loadException);
            }
        }
        catch (Exception e)
        {
            // Ensure cleanup if we have references but something went wrong
            if (plugin != null && _plugins.ContainsKey(plugin))
            {
                _plugins.Remove(plugin);
            }

            return e;
        }
    }

    /// <summary>
    /// Unloads the given plugin.
    /// </summary>
    /// <param name="plugin">The plugin to unload.</param>
    public void Unload(ISkuaPlugin plugin)
    {
        StrongReferenceMessenger.Default.Send<PluginUnloadedMessage, int>(new(_plugins[plugin]), (int)MessageChannels.Plugins);
        plugin.Unload();
        _plugins.Remove(plugin);
    }

    /// <summary>
    /// Unloads the plugin by it's given name.
    /// </summary>
    /// <param name="pluginName">The name o the plugin to unload.</param>
    public void Unload(string pluginName)
    {
        ISkuaPlugin? plugin = Containers.Find(x => x.ToString() == pluginName)?.Plugin;
        if (plugin is null)
            return;

        StrongReferenceMessenger.Default.Send<PluginUnloadedMessage, int>(new(_plugins[plugin]), (int)MessageChannels.Plugins);
        plugin.Unload();
        _plugins.Remove(plugin);
    }

    /// <summary>
    /// Gets the container for the given plugin.
    /// </summary>
    /// <param name="plugin">The plugin to get the container for.</param>
    /// <returns>The plugin's container.</returns>
    public IPluginContainer? GetContainer(ISkuaPlugin plugin)
    {
        return _plugins.TryGetValue(plugin, out IPluginContainer? container) ? container : null;
    }

    /// <summary>
    /// Gets the container for the given plugin name.
    /// </summary>
    /// <param name="pluginName">Name of the plugin to get the container for.</param>
    /// <returns>The plugin's container</returns>
    public IPluginContainer? GetContainer(string pluginName)
    {
        return Containers.Exists(x => x.ToString() == pluginName) ? Containers.Find(x => x.ToString() == pluginName) : null;
    }

    /// <summary>
    /// Gets the container for the plugin with the given type.
    /// </summary>
    /// <typeparam name="T">The type of the plugin to get the container for.</typeparam>
    /// <returns>The container for the plugin with the given type.</returns>
    public IPluginContainer GetContainer<T>() where T : ISkuaPlugin
    {
        return _plugins.FirstOrDefault((KeyValuePair<ISkuaPlugin, IPluginContainer> kvp) => kvp.Key is T).Value;
    }
}