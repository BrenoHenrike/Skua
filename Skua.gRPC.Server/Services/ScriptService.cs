using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skua.Core.Models.GitHub;
using Skua.Core.Utils;
using Skua.gRPC.Server.Models;

namespace Skua.gRPC.Server.Services;

public class ScriptService : GetScripts.GetScriptsBase
{
    private readonly ScriptContainer _container;

    public ScriptService(ScriptContainer container)
    {
        _container = container;
    }

    public override Task<Empty> SetupGHClient(TokenRequest request, ServerCallContext context)
    {
        if(HttpClients.UserGitHubClient is not null)
            return Task.FromResult(new Empty());

        if (string.IsNullOrWhiteSpace(request.Token))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid authentication token."));

        HttpClients.UserGitHubClient = new(request.Token);

        return Task.FromResult(new Empty());
    }

    public override async Task<GetReposResponse> GetRepos(GetReposRequest request, ServerCallContext context)
    {
        var response = new GetReposResponse();
        if (!request.Refresh && _container.Repos.Count > 0)
        {
            response.Repos.AddRange(_container.Repos);
            return response;
        }

        using HttpResponseMessage httpResponse = await HttpClients.Default.GetAsync(_container.BaseUrl, context.CancellationToken);

        if (!httpResponse.IsSuccessStatusCode)
            throw new RpcException(new Status(StatusCode.ResourceExhausted, "GitHub API limit reached."));

        var repos = (await httpResponse.Content.ReadAsStringAsync(context.CancellationToken)).Split('\n').Select(l => l.Trim().Split('|')).Where(p => p.Length == 5).Select(p => new ScriptRepo(p[0], p[1], p[2], p[3], p[4])).ToList();
        foreach(var repo in repos)
            response.Repos.Add(Map(repo));

        if(response.Repos.Count > 0)
        {
            _container.Repos.Clear();
            _container.Repos.AddRange(response.Repos);
        }

        return response;
    }

    public override async Task<GetScriptResponse> GetScriptsFromRepo(GetScriptRequest request, ServerCallContext context)
    {
        var response = new GetScriptResponse();
        if (!request.Refresh && _container.Scripts.ContainsKey(request.Repo))
        {
            response.ScriptInfos.Add(_container.Scripts[request.Repo]);
            return response;
        }

        var recursiveTreeUrl = await GetLastCommitRecursiveTree(request.Repo, context.CancellationToken);
        if (string.IsNullOrEmpty(recursiveTreeUrl))
            return new();
        using HttpResponseMessage treeResponse = await HttpClients.GetGHClient().GetAsync(recursiveTreeUrl, context.CancellationToken);
        IEnumerable<ScriptTreeInfo>? treeInfos =
            JsonConvert.DeserializeObject<ScriptTree>(await treeResponse.Content.ReadAsStringAsync(context.CancellationToken))?.TreeInfo?.Where(i => i.Type == "tree") ?? null;

        List<Task<HttpResponseMessage>> requests = treeInfos?.Select(i => HttpClients.GetGHClient().GetAsync(GetContentUrl(request.Repo, i.Path), context.CancellationToken))
                                                             .ToList() ?? new();
        requests.Add(HttpClients.GetGHClient().GetAsync(GetContentUrl(request.Repo), context.CancellationToken));
        await Task.WhenAll(requests);

        IEnumerable<Task<string>> contents = requests.Select(request => request.Result)
                               .Select(result => result.Content.ReadAsStringAsync());
        await Task.WhenAll(contents);

        response.ScriptInfos.Add(contents.Select(content => content.Result)
                                         .Select(t => JsonConvert.DeserializeObject<List<ScriptInfoResponse>>(t))
                                         .SelectMany(l => l!)
                                         .Where(s => s.FileName.EndsWith(".cs"))
                                         .Distinct()
                                         .Where(i => i is not null)
                                         .Select(s => Map(s)));
        if (_container.Scripts.ContainsKey(request.Repo))
            _container.Scripts[request.Repo] = response.ScriptInfos;
        else
            _container.Scripts.Add(request.Repo, response.ScriptInfos);

        return response;
    }

    private async Task<string> GetLastCommitRecursiveTree(GrpcScriptRepo repo, CancellationToken token)
    {
        using HttpResponseMessage response = await HttpClients.GetGHClient().GetAsync($"https://api.github.com/repos/{repo.Username}/{repo.RepoName}/git/refs/heads/{(string.IsNullOrEmpty(repo.Branch) ? "master" : repo.Branch)}", token);
        dynamic? commits = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync(token));
        if (commits is null)
            return string.Empty;
        return $"https://api.github.com/repos/{repo.Username}/{repo.RepoName}/git/trees/{Convert.ToString(commits["object"].sha)}?recursive=true";
    }

    private string GetContentUrl(GrpcScriptRepo repo, string extensionPath = "")
    {
        if (string.IsNullOrEmpty(extensionPath))
            return $"https://api.github.com/repos/{repo.Username}/{repo.RepoName}/contents/{repo.Extension}{(string.IsNullOrWhiteSpace(repo.Branch) ? string.Empty : $"?ref={repo.Branch}")}";

        return $"https://api.github.com/repos/{repo.Username}/{repo.RepoName}/contents/{extensionPath}{(string.IsNullOrWhiteSpace(repo.Branch) ? string.Empty : $"?ref={repo.Branch}")}";
    }

    private GrpcScriptRepo Map(ScriptRepo repo)
    {
        return new GrpcScriptRepo()
        {
            Username = repo.Username,
            RepoName = repo.Name,
            Extension = repo.Extension,
            Branch = repo.Branch,
            Author = repo.Author
        };
    }

    private GrpcScriptInfo Map(ScriptInfo info)
    {
        return new GrpcScriptInfo()
        {
            FileName = info.FileName,
            Hash = info.Hash,
            Size = info.Size,
            DownloadUrl = info.DownloadUrl,
            FilePath = info.FilePath
        };
    }

    private GrpcScriptInfo Map(ScriptInfoResponse info)
    {
        return new GrpcScriptInfo()
        {
            FileName = info.FileName,
            Hash = info.Hash,
            Size = info.Size,
            DownloadUrl = info.DownloadUrl,
            FilePath = info.FilePath
        };
    }
}
