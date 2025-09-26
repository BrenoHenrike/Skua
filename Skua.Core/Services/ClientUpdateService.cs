using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using Skua.Core.Models.GitHub;
using Skua.Core.Utils;
using System.Diagnostics;
using System.IO.Compression;

namespace Skua.Core.Services;

public class ClientUpdateService : IClientUpdateService
{
    private readonly ISettingsService _settingsService;
    private readonly IDialogService _dialogService;

    public ClientUpdateService(ISettingsService settingsService, IDialogService dialogService)
    {
        _settingsService = settingsService;
        _dialogService = dialogService;
    }

    public List<UpdateInfo> Releases { get; set; } = new();

    public async Task GetReleasesAsync()
    {
        var releaseSearch = await HttpClients.GitHubRaw.GetAsync("auqw/Skua/refs/heads/master/releases.json");
        if (!releaseSearch.IsSuccessStatusCode)
            return;

        var releases = await releaseSearch.Content.ReadAsStringAsync();
        var releaseList = JsonConvert.DeserializeObject<List<UpdateInfo>>(releases) ?? null;
        if (releaseList is null)
            return;

        Releases.Clear();
        Releases = releaseList.OrderByDescending(r => r.ParsedVersion).ToList();
    }

    public async Task DownloadUpdateAsync(IProgress<string>? progress, UpdateInfo info)
    {
        try
        {
            progress?.Report("Downloading...");
            string? downloadURL = null;
            string? fileName = null;
            if (Environment.Is64BitOperatingSystem)
            {
                downloadURL = info.Assets.FirstOrDefault(a => a.BrowserUrl!.Contains("x64"))?.BrowserUrl;
                fileName = downloadURL!.Split('/').Last();
            }
            else
            {
                downloadURL = info.Assets.FirstOrDefault(a => a.BrowserUrl!.Contains("x86"))?.BrowserUrl;
                fileName = downloadURL!.Split('/').Last();
            }

            var file = await HttpClients.Default.GetByteArrayAsync(downloadURL);

            progress?.Report("Writing to folder...");
            string path = _settingsService.Get("ClientDownloadPath", string.Empty);
            if (string.IsNullOrEmpty(path) && !AppDomain.CurrentDomain.BaseDirectory.Contains("Program Files"))
                path = Directory.GetParent(AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar))?.FullName ?? AppContext.BaseDirectory;

            string filePath = Path.Combine(path, fileName);
            await File.WriteAllBytesAsync(filePath, file);
            string extension = Path.GetExtension(filePath);
            if (extension == ".msi" || extension == ".exe")
            {
                string winDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                ProcessStartInfo startInfo = new(Path.Combine(winDir, @"System32\msiexec.exe"),
                    $"/i {filePath} /quiet /passive /qb!- /norestart ALLUSERS=1")
                {
                    Verb = "runas",
                    UseShellExecute = true
                };
                Process? proc = Process.Start(startInfo);
                proc!.WaitForExit();
                if (proc.ExitCode == 0)
                {
                    _settingsService.Set("ChangeLogActivated", false);
                    string startMenuPath = AppContext.BaseDirectory;
                    string appPath = Path.Combine(startMenuPath, "Skua.Manager.exe");
                    Process.Start(appPath);
                    Environment.Exit(0);
                }
            }
            else
            {
                string updateFolder = Path.Combine(path, fileName.Replace(".zip", string.Empty));
                if (Directory.Exists(updateFolder))
                    Directory.Delete(updateFolder, true);

                progress?.Report("Extracting files...");
                ZipFile.ExtractToDirectory(filePath, updateFolder);

                if (_settingsService.Get<bool>("DeleteZipFileAfter"))
                    File.Delete(filePath);

                progress?.Report("Checking for Skua Manager...");
                if (File.Exists(Path.Combine(updateFolder, "Skua.Manager.exe")))
                {
                    progress?.Report("Waiting for services shutdown...");
                    if (await StrongReferenceMessenger.Default.Send<UpdateStartedMessage>() == false)
                    {
                        progress?.Report("Something went wrong finishing services");
                        return;
                    }

                    progress?.Report("Starting updated version...");

                    Process.Start(Path.Combine(path, updateFolder, "Skua.Manager.exe"));
                    //StrongReferenceMessenger.Default.Send<UpdateFinishedMessage>();
                    return;
                }
            }
        }
        catch (Exception e)
        {
            progress?.Report("Error while updating.");
            _dialogService.ShowMessageBox($"Error Message:\r\n{e.Message}", "Update Error");
            return;
        }
        progress?.Report("Failed to start downloaded version");
    }
}