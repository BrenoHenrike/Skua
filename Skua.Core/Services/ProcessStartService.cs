using System.Diagnostics;
using Skua.Core.Interfaces;
namespace Skua.Core.Services;
public class ProcessStartService : IProcessService
{
    public ProcessStartService(ISettingsService settingsService, IDialogService dialogService)
    {
        _settingsService = settingsService;
        _dialogService = dialogService;
    }

    private readonly string _vscPath = Path.Combine(AppContext.BaseDirectory, "VSCode", "code");
    private readonly string _scriptsPath = Path.Combine(AppContext.BaseDirectory, "Scripts");
    private readonly ISettingsService _settingsService;
    private readonly IDialogService _dialogService;

    public void OpenLink(string link)
    {
        var ps = new ProcessStartInfo(link)
        {
            UseShellExecute = true,
            Verb = "open"
        };
        Process.Start(ps);
    }

    public void OpenVSC()
    {
        if (_settingsService.Get("UseLocalVSC", false))
        {
            Process.Start(_vscPath, _scriptsPath);
            return;
        }
        try
        {
            VSCode(string.Empty);
        }
        catch
        {
            _dialogService.ShowMessageBox("VSCode was not found. If you have VSCode, reinstall it and be sure to check it to install in you PATH folder.", "VSCode not found");
        }
    }

    public void OpenVSC(string path)
    {
        if (_settingsService.Get("UseLocalVSC", false))
        {
            Process.Start(_vscPath, new[] { _scriptsPath, path, "--reuse-window" });
            return;
        }
        try
        {
            VSCode(path);
        }
        catch
        {
            Process.Start("notepad", path);
        }
    }

    private void VSCode(string path)
    {
        ProcessStartInfo psi = new("code", $"{_scriptsPath} {path}")
        {
            UseShellExecute = true,
            CreateNoWindow = true

        };

        Process.Start(psi);
    }
}
