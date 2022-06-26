using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skua.Core.Interfaces.Services;
public interface ISettingsService
{
    void SetValue<T>(string key, T value);
    T? GetValue<T>(string key);
    T GetValue<T>(string key, T defaultValue);
}
