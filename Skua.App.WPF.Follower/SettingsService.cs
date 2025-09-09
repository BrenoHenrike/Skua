using Skua.Core.Interfaces;

namespace Skua.App.WPF.Follower;

public class SettingsService : ISettingsService
{
    public T? Get<T>(string key)
    {
        return default;
    }

    public T Get<T>(string key, T defaultValue)
    {
        var suco = default(T);

        return suco is null ? defaultValue : suco;
    }

    public void Set<T>(string key, T value)
    {
    }
}