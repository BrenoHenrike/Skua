using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skua.App.Properties;
using Skua.Core.Interfaces.Services;

namespace Skua.App.Services;
public class SettingsService : ISettingsService
{
    public T? GetValue<T>(string key)
    {
        return (T?)Settings.Default[key];
    }

    public T GetValue<T>(string key, T defaultValue)
    {
        T value = (T)Settings.Default[key];
        return value is null ? defaultValue : value;
    }

    public void SetValue<T>(string key, T value)
    {
        Settings.Default[key] = value;
        Settings.Default.Save();
    }
}
