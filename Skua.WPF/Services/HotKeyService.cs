using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.Utils;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Input;

namespace Skua.WPF.Services;
public class HotKeyService : IHotKeyService
{
    public HotKeyService(Dictionary<string, ICommand> hotKeys, ISettingsService settingsService, IDecamelizer decamelizer)
    {
        _converter = new();
        _hotKeys = hotKeys;
        _settingsService = settingsService;
        _decamelizer = decamelizer;
    }

    private readonly Dictionary<string, ICommand> _hotKeys;
    private readonly ISettingsService _settingsService;
    private readonly IDecamelizer _decamelizer;
    private KeyGestureConverter _converter;

    public void Reload()
    {
        Application.Current.MainWindow.InputBindings.Clear();
        StringCollection? hotkeys = _settingsService.Get<StringCollection>("HotKeys");
        if (hotkeys is null)
            return;
        foreach (string? hk in hotkeys)
        {
            if (string.IsNullOrEmpty(hk))
                continue;

            var split = hk.Split('|');
            if(_hotKeys.ContainsKey(split[0]))
                Application.Current.MainWindow.InputBindings.Add(new KeyBinding(_hotKeys[split[0]], _converter.ConvertFromString(split[1]) as KeyGesture));
        }
    }

    public List<T> GetHotKeys<T>()
        where T : IHotKey, new()
    {
        var hotkeys = _settingsService.Get<StringCollection>("HotKeys");
        if (hotkeys is null)
            return new();

        List<T> parsed = new();
        foreach(string hk in hotkeys)
        {
            if (string.IsNullOrEmpty(hk))
                continue;
            var split = hk.Split('|');
            parsed.Add(new() { Binding = split[0], Title = _decamelizer.Decamelize(split[0], null), KeyGesture = split[1] });
        }
        return parsed;
    }

    public HotKey? Parse(string keyGesture)
    {
        if (_converter.ConvertFromString(keyGesture) is not KeyGesture kg)
            return null;

        return new HotKey(kg.Key.ToString(), kg.Modifiers.HasFlag(ModifierKeys.Control), kg.Modifiers.HasFlag(ModifierKeys.Alt), kg.Modifiers.HasFlag(ModifierKeys.Shift));
    }
}
