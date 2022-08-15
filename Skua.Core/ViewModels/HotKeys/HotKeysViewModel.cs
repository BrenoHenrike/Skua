using CommunityToolkit.Mvvm.Messaging;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using Skua.Core.Models;
using System.Collections.Specialized;

namespace Skua.Core.ViewModels;
public partial class HotKeysViewModel : BotControlViewModelBase, IManagedWindow
{
    public HotKeysViewModel(IHotKeyService hotKeyService, ISettingsService settingsService, IDialogService dialogService)
        : base("HotKeys", 380, 0)
    {
        _hotKeyService = hotKeyService;
        _settingsService = settingsService;
        _dialogService = dialogService;
        HotKeys = _hotKeyService.GetHotKeys<HotKeyItemViewModel>();
        _hotKeyService.Reload();
    }

    private readonly IHotKeyService _hotKeyService;
    private readonly ISettingsService _settingsService;
    private readonly IDialogService _dialogService;

    protected override void OnActivated()
    {
        StrongReferenceMessenger.Default.Register<HotKeysViewModel, EditHotKeyMessage>(this, EditHotKey);
    }

    protected override void OnDeactivated()
    {
        StrongReferenceMessenger.Default.UnregisterAll(this);
        base.OnDeactivated();
    }

    public List<HotKeyItemViewModel> HotKeys { get; }

    private void Save()
    {
        StringCollection hotkeys = new();
        foreach (var hk in HotKeys)
            hotkeys.Add($"{hk.Binding}|{hk.KeyGesture}");

        _settingsService.Set("HotKeys", hotkeys);
    }

    private void EditHotKey(HotKeysViewModel recipient, EditHotKeyMessage message)
    {
        HotKey? hotKey = recipient._hotKeyService.Parse(message.KeyGesture);
        if (hotKey == null)
            return;
        var diag = new AssignHotKeyDialogViewModel(message.Title, hotKey);
        if (recipient._dialogService.ShowDialog(diag) == true)
        {
            var hk = recipient.HotKeys.Find(hk => hk.Title == message.Title);
            if (hk != null)
            {
                hk.KeyGesture = diag.KeyGesture;
                recipient.Save();
                recipient._hotKeyService.Reload();
            }
        }
    }
}
