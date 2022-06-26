using Microsoft.Toolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using Skua.Core.Interfaces;
using Skua.Core.Models.GitHub;
using Skua.Core.Utils;

namespace Skua.Core.Services;
public partial class GetScriptsService : ObservableObject, IGetScriptsService
{
    public GetScriptsService(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }
    private const string BaseUrl = "https://raw.githubusercontent.com/brenohenrike/rbot/master/repos";
    private const string DebugUrl = "https://raw.githubusercontent.com/brenohenrike/rbot/master/debugrepos";
    private readonly IDialogService _dialogService;
    private List<ScriptRepo>? _repos;
    private RangedObservableCollection<ScriptInfo> _scripts = new();
    public RangedObservableCollection<ScriptInfo> Scripts
    {
        get { return _scripts; }
        set { SetProperty(ref _scripts, value); }
    }

    public async ValueTask<List<ScriptInfo>> GetScriptsAsync(IProgress<string>? progress, CancellationToken token)
    {
        if (_scripts.Any())
            return _scripts.ToList();
        await RefreshAsync(progress, token);

        return _scripts.ToList();
    }

    public async Task RefreshAsync(IProgress<string>? progress, CancellationToken token)
    {
        try
        {
            Scripts.Clear();
            progress?.Report("Fetching repos...");
            List<ScriptRepo> repos = await GetRepos(token);
            progress?.Report("Fetching scripts...");
            int total = 0;
            foreach (ScriptRepo repo in repos)
            {
                List<ScriptInfo> scripts = await GetScriptsInfo(repo, token);
                progress?.Report($"Found {scripts.Count} scripts.");
                _scripts.AddRange(scripts);
                total += scripts.Count;
            }
            progress?.Report($"Fetched {total} scripts.");
            OnPropertyChanged(nameof(Scripts));
        }
        catch (TaskCanceledException)
        {
            progress?.Report("Task Cancelled.");
        }
        catch
        {
            _dialogService.ShowMessageBox("GitHub API limit reached", "Get Scripts Error");
            progress?.Report("GitHub API limit reached.");
        }
    }

    private async ValueTask<List<ScriptRepo>> GetRepos(CancellationToken token)
    {
        if (_repos is not null)
            return _repos;
        using HttpResponseMessage response = await HttpClients.Default.GetAsync(BaseUrl, token);
        return _repos = (await response.Content.ReadAsStringAsync(token)).Split('\n').Select(l => l.Trim().Split('|')).Where(p => p.Length == 4).Select(p => new ScriptRepo(p[0], p[1], p[2], p[3])).ToList();
    }

    private async Task<List<ScriptInfo>> GetScriptsInfo(ScriptRepo repo, CancellationToken token)
    {
        await GetLastCommitRecursiveTree(repo, token);
        if (repo.RecursiveTreeUrl == string.Empty)
            return new();
        using HttpResponseMessage treeResponse = await HttpClients.GetGHClient().GetAsync(repo.RecursiveTreeUrl, token);
        IEnumerable<ScriptTreeInfo>? treeInfos = 
            JsonConvert.DeserializeObject<ScriptTree>(await treeResponse.Content.ReadAsStringAsync(token))?.TreeInfo?.Where(i => i.Type == "tree") ?? null;

        List<Task<HttpResponseMessage>> requests = treeInfos?.Select(i => HttpClients.GetGHClient().GetAsync(repo.GetContentUrl(i.Path), token))
                                                             .ToList() ?? new();
        requests.Add(HttpClients.GetGHClient().GetAsync(repo.ContentsUrl, token));
        await Task.WhenAll(requests);

        IEnumerable<Task<string>> contents = requests.Select(request => request.Result)
                               .Select(result => result.Content.ReadAsStringAsync());
        await Task.WhenAll(contents);
        return contents.Select(content => content.Result)
                       .Select(t => JsonConvert.DeserializeObject<List<ScriptInfo>>(t))
                       .SelectMany(l => l!)
                       .Where(s => s.FileName.EndsWith(".cs"))
                       .Distinct()
                       .ToList();
    }

    private async Task<string> GetLastCommitRecursiveTree(ScriptRepo repo, CancellationToken token)
    {
        using HttpResponseMessage response = await HttpClients.GetGHClient().GetAsync(repo.CommitsUrl, token);
        dynamic? commits = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
        if (commits is null)
            return string.Empty;
        return repo.RecursiveTreeUrl = $"https://api.github.com/repos/{repo.Username}/{repo.Name}/git/trees/{Convert.ToString(commits[0].sha)}?recursive=true";
    }

    public async Task DownloadScriptAsync(ScriptInfo info)
    {
        DirectoryInfo parent = Directory.GetParent(info.LocalFile)!;
        if (!parent.Exists)
            parent.Create();
        using (HttpResponseMessage response = await HttpClients.Default.GetAsync(info.DownloadUrl))
        {
            string script = await response.Content.ReadAsStringAsync();
            await File.WriteAllTextAsync(info.LocalFile, script);
        }
        DirectoryInfo sha = Directory.GetParent(info.LocalShaFile)!;
        if (!sha.Exists)
            sha.Create();
        await File.WriteAllTextAsync(info.LocalShaFile, info.Hash);
    }

    public async Task<int> DownloadAllWhereAsync(Func<ScriptInfo, bool> pred)
    {
        IEnumerable<ScriptInfo> toUpdate = _scripts.Where(pred);
        int count = toUpdate.Count();
        await Task.WhenAll(toUpdate.Select(s => DownloadScriptAsync(s)));
        return count;
    }

    public async Task DeleteScriptAsync(ScriptInfo info)
    {
        await Task.Run(() =>
        {
            try
            {
                File.Delete(info.LocalFile);
                File.Delete(info.LocalShaFile);
            }
            catch { }
        });
    }
}
