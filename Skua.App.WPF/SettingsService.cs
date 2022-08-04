using Skua.App.WPF.Properties;
using Skua.Core.Interfaces;

namespace Skua.App.WPF.Services;
public class SettingsService : ISettingsService
{
    public T? Get<T>(string key)
    {
        return (T?)Settings.Default[key];
    }

    public T Get<T>(string key, T defaultValue)
    {
        T value = (T)Settings.Default[key];
        return value is null ? defaultValue : value;
    }

    public void Set<T>(string key, T value)
    {
        Settings.Default[key] = value;
        Settings.Default.Save();
    }
}
