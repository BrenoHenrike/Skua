using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using Skua.Core.Interfaces;
using Skua.Core.Models.GitHub;
using Skua.Core.Utils;

namespace Skua.Core.Services;
public partial class GetScriptsService : ObservableObject, IGetScriptsService
{
    private const string _reposUrl = "https://raw.githubusercontent.com/brenohenrike/skua/master/repos";
    private const string _skillsSetsRawUrl = "https://raw.githubusercontent.com/BrenoHenrike/Scripts/Skua/Skills/AdvancedSkills.txt";
    private readonly IDialogService _dialogService;
    private List<ScriptRepo> _repos = new();
    
    [ObservableProperty]
    private RangedObservableCollection<ScriptInfo> _scripts = new();
    
    public GetScriptsService(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }

    public async ValueTask<List<ScriptInfo>> GetScriptsAsync(IProgress<string>? progress, CancellationToken token)
    {
        if (_scripts.Any())
            return _scripts.ToList();
        
        await GetScripts(progress, false, token);

        return _scripts.ToList();
    }

    public long GetSkillsSetsTextFileSize()
    {
        var skillsSetsFile = Path.Combine(AppContext.BaseDirectory, "AdvancedSkills.txt");
        var file = new FileInfo(skillsSetsFile);

        if (file.Exists)
            return file.Length;
           
        return -1;
    }

    public async Task RefreshScriptsAsync(IProgress<string>? progress, CancellationToken token)
    {
        await GetScripts(progress, true, token);
    }

    public async Task<long> DownloadSkillSetsFile()
    {
        using (var client = new HttpClient())
        {
            var response = await client.GetAsync(_skillsSetsRawUrl);
            var content = await response.Content.ReadAsStringAsync();
            var skillsSetsFile = Path.Combine(AppContext.BaseDirectory, "AdvancedSkills.txt");

            await File.WriteAllTextAsync(skillsSetsFile, content);

            var file = new FileInfo(skillsSetsFile);
            return file.Length;
        }
    }

    private async Task GetScripts(IProgress<string>? progress, bool refresh, CancellationToken token)
    {
        try
        {
            Scripts.Clear();
            progress?.Report("Fetching repos...");
            
            List<ScriptRepo> repos = await GetRepos(refresh, token);
            progress?.Report("Fetching scripts...");
            
            int total = 0;
            foreach (ScriptRepo repo in repos)
            {
                List<ScriptInfo> scripts = await GetScriptsInfo(repo, refresh, token);
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
        catch (Exception ex) when (ex.Message == "bad credentials")
        {
            _dialogService.ShowMessageBox("Something went wrong when retrieving the GitHub Token.\r\nPlease, redo the GitHub Authentication steps in the Manager > Options.", "Get Scripts Error");
        }
        catch
        {
            _dialogService.ShowMessageBox("GitHub API limit reached", "Get Scripts Error");
            progress?.Report("GitHub API limit reached.\r\nIf you are seeing this even after doing the Authentication steps, try doing it again in the Manager > Options.");
        }
    }

    private async ValueTask<List<ScriptRepo>> GetRepos(bool refresh, CancellationToken token)
    {
        if (_repos.Count != 0 && !refresh)
            return _repos;
        
        using HttpResponseMessage response = await HttpClients.Default.GetAsync(_reposUrl, token);
        
        return _repos = (await response.Content.ReadAsStringAsync(token))
            .Split('\n').Select(l => l.Trim()
            .Split('|')).Where(p => p.Length == 5)
            .Select(p => new ScriptRepo(p[0], p[1], p[2], p[3], p[4]))
            .ToList();
    }

    private async Task<List<ScriptInfo>> GetScriptsInfo(ScriptRepo repo, bool refresh, CancellationToken token)
    {
        if (_scripts.Count != 0 && !refresh)
            return _scripts.ToList();
        
        await GetLastCommitRecursiveTree(repo, token);
        
        if (string.IsNullOrEmpty(repo.RecursiveTreeUrl))
            return new();
        
        using HttpResponseMessage treeResponse = await HttpClients.GetGHClient()
            .GetAsync(repo.RecursiveTreeUrl, token);
        
        IEnumerable<ScriptTreeInfo>? treeInfos = JsonConvert.DeserializeObject<ScriptTree>(await treeResponse.Content.ReadAsStringAsync(token))?.TreeInfo?.Where(i => i.Type == "tree") ?? null;

        List<Task<HttpResponseMessage>> requests = treeInfos?.Select(i => HttpClients.GetGHClient().GetAsync(repo.GetContentUrl(i.Path), token)).ToList() ?? new();
        requests.Add(HttpClients.GetGHClient()
            .GetAsync(repo.ContentsUrl, token));
        
        await Task.WhenAll(requests);

        IEnumerable<Task<string>> contents = requests.Select(request => request.Result)
            .Select(result => result.Content.ReadAsStringAsync());
        await Task.WhenAll(contents);
        
        if (contents.Count() == 1 && contents.First().Result.Contains("Bad credentials"))
            throw new Exception("bad credentials");
        
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
        
        return repo.RecursiveTreeUrl = $"https://api.github.com/repos/{repo.Username}/{repo.Name}/git/trees/{Convert.ToString(commits.sha)}?recursive=true";
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

    public async Task ManagerDownloadScriptAsync(ScriptInfo info)
    {
        DirectoryInfo parent = Directory.GetParent(info.ManagerLocalFile)!;
        if (!parent.Exists)
            parent.Create();
        using (HttpResponseMessage response = await HttpClients.Default.GetAsync(info.DownloadUrl))
        {
            string script = await response.Content.ReadAsStringAsync();
            await File.WriteAllTextAsync(info.ManagerLocalFile, script);
        }
        DirectoryInfo sha = Directory.GetParent(info.ManagerLocalShaFile)!;
        if (!sha.Exists)
            sha.Create();
        await File.WriteAllTextAsync(info.ManagerLocalShaFile, info.Hash);
    }

    public async Task<int> DownloadAllWhereAsync(Func<ScriptInfo, bool> pred)
    {
        IEnumerable<ScriptInfo> toUpdate = _scripts.Where(pred);
        int count = toUpdate.Count();
        await Task.WhenAll(toUpdate.Select(s => DownloadScriptAsync(s)));
        return count;
    }

    public async Task<int> ManagerDownloadAllWhereAsync(Func<ScriptInfo, bool> pred)
    {
        IEnumerable<ScriptInfo> toUpdate = _scripts.Where(pred);
        int count = toUpdate.Count();
        await Task.WhenAll(toUpdate.Select(s => ManagerDownloadScriptAsync(s)));
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
