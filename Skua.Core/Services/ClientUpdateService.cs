using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using Skua.Core.Models.GitHub;
using Skua.Core.Utils;
using System.Collections.Specialized;
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
        var releaseSearch = await HttpClients.GetGHClient().GetAsync("https://raw.githubusercontent.com/BrenoHenrike/Skua/op-version/releases.json");
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
            string fileName = $"Skua-{info.Version}-Release-{(Environment.Is64BitOperatingSystem ? "x64" : "x86")}.zip";
            string uri = $"{info.URL.Replace("tag", "download")}/{fileName}";

            var file = await HttpClients.Default.GetByteArrayAsync(uri);

            progress?.Report("Writing to folder...");
            string path = _settingsService.Get("ClientDownloadPath", string.Empty);

            if (string.IsNullOrEmpty(path))
                path = Directory.GetParent(AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar))?.FullName ?? AppContext.BaseDirectory;

            string filePath = Path.Combine(path, fileName);
            await File.WriteAllBytesAsync(filePath, file);
            string updateFolder = Path.Combine(path, fileName.Replace(".zip", string.Empty));
            if(Directory.Exists(updateFolder))
                Directory.Delete(updateFolder, true);

            progress?.Report("Extracting files...");
            ZipFile.ExtractToDirectory(filePath, updateFolder);

            if (_settingsService.Get<bool>("DeleteZipFileAfter"))
                File.Delete(filePath);

            progress?.Report("Checking for Skua Manager...");
            if(File.Exists(Path.Combine(updateFolder, "Skua.Manager.exe")))
            {
                progress?.Report("Waiting for services shutdown...");
                if (await StrongReferenceMessenger.Default.Send<UpdateStartedMessage>() == false)
                {
                    progress?.Report("Something went wrong finishing services");
                    return;
                }

                List<string> args = new();

                StringCollection? themes = _settingsService.Get<StringCollection>("UserThemes");
                if(themes is not null)
                {
                    args.Add("--user-themes");
                    args.Add(string.Join('|', themes));
                }

                string? currTheme = _settingsService.Get<string>("CurrentTheme");
                if (!string.IsNullOrEmpty(currTheme))
                {
                    args.Add("--curr-theme");
                    args.Add(currTheme);
                }

                string? ghToken = _settingsService.Get<string>("UserGitHubToken");
                if(!string.IsNullOrEmpty(ghToken))
                {
                    args.Add("--gh-token");
                    args.Add(ghToken);
                }

                string? downloadPath = _settingsService.Get<string>("ClientDownloadPath");
                if (!string.IsNullOrEmpty(downloadPath))
                {
                    args.Add("--download-path");
                    args.Add(downloadPath);
                }

                args.Add("--client-releases");
                args.Add(_settingsService.Get<bool>("CheckClientUpdates").ToString());

                args.Add("--client-pre-releases");
                args.Add(_settingsService.Get<bool>("CheckClientPrereleases").ToString());

                args.Add("--auto-update-bot-scripts");
                args.Add(_settingsService.Get<bool>("AutoUpdateBotScripts").ToString());

                args.Add("--bot-script-updates");
                args.Add(_settingsService.Get<bool>("CheckBotScriptsUpdates").ToString());

                args.Add("--auto-update-advanceskill-sets");
                args.Add(_settingsService.Get<bool>("AutoUpdateAdvanceSkillSetsUpdates").ToString());

                args.Add("--advanceskill-sets-updates");
                args.Add(_settingsService.Get<bool>("CheckAdvanceSkillSetsUpdates").ToString());

                args.Add("--delete-zip");
                args.Add(_settingsService.Get<bool>("DeleteZipFileAfter").ToString());

                progress?.Report("Starting updated version...");

                Process.Start(Path.Combine(path, updateFolder, "Skua.Manager.exe"), args);
                //StrongReferenceMessenger.Default.Send<UpdateFinishedMessage>();
                return;
            }
        }
        catch(Exception e)
        {
            progress?.Report("Error while updating.");
            _dialogService.ShowMessageBox($"Error Message:\r\n{e.Message}", "Update Error");
            return;
        }
        progress?.Report("Failed to start downloaded version");
    }
}
