using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skua.Core.Interfaces;
public interface ISettingsService
{
    void Set<T>(string key, T value);
    T? Get<T>(string key);
    T GetValue<T>(string key, T defaultValue);
}
