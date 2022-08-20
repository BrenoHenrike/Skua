using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using Skua.Core.Models;
using Skua.Core.Utils;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace Skua.WPF.Services;
public class HotKeyService : IHotKeyService
{
    public HotKeyService(Dictionary<string, IRelayCommand> hotKeys, ISettingsService settingsService, IDecamelizer decamelizer)
    {
        _hotKeys = hotKeys;
        _settingsService = settingsService;
        _decamelizer = decamelizer;
    }

    private readonly Dictionary<string, IRelayCommand> _hotKeys;
    private readonly ISettingsService _settingsService;
    private readonly IDecamelizer _decamelizer;

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
            {
                KeyBinding? kb = ParseToKeyBinding(split[1]);
                if(kb is null)
                {
                    StrongReferenceMessenger.Default.Send<HotKeyErrorMessage>(new(split[0]));
                    continue;
                }
                kb.Command = _hotKeys[split[0]];
                Application.Current.MainWindow.InputBindings.Add(kb);
            }
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

    public HotKey? ParseToHotKey(string keyGesture)
    {
        KeyBinding? kb = ParseToKeyBinding(keyGesture);
        if (kb is null)
            return null;
        return new HotKey(kb.Key.ToString(), kb.Modifiers.HasFlag(ModifierKeys.Control), kb.Modifiers.HasFlag(ModifierKeys.Alt), kb.Modifiers.HasFlag(ModifierKeys.Shift));
    }

    private KeyBinding? ParseToKeyBinding(string keyGesture)
    {
        string ksc = keyGesture.ToLower();
        KeyBinding kb = new();

        if (ksc.Contains("alt"))
            kb.Modifiers = ModifierKeys.Alt;
        if (ksc.Contains("shift"))
            kb.Modifiers |= ModifierKeys.Shift;
        if (ksc.Contains("ctrl") || ksc.Contains("ctl"))
            kb.Modifiers |= ModifierKeys.Control;

        string key =
            ksc.Replace("+", string.Empty)
               .Replace("alt", string.Empty)
               .Replace("shift", string.Empty)
               .Replace("ctrl", string.Empty)
               .Replace("ctl", string.Empty);

        key = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(key);
        if (!string.IsNullOrEmpty(key))
        {
            KeyConverter k = new();
            kb.Key = (Key)k.ConvertFromString(key);
        }

        if (kb.Key == Key.None)
            return null;

        return kb;
    }
}
