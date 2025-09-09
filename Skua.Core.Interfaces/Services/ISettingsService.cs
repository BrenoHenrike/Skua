namespace Skua.Core.Interfaces;

public interface ISettingsService
{
    void Set<T>(string key, T value);

    T? Get<T>(string key);

    T Get<T>(string key, T defaultValue);
}