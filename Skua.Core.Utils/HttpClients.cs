namespace Skua.Core.Utils;
public class CustomClient : HttpClient
{
    //private readonly string _authString1 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("726820423be5c752df62:63b2a5b1a55fbeade88deab3b6c8914808bad7a6"));
    private readonly string _authString2 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("449f889db3d655d2ef4a:27863d426bc5bb46c410daf7ed6b479ba4a9f7eb"));

    public CustomClient(bool accJson)
    {
        DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", _authString2);
        if (accJson)
            DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        DefaultRequestHeaders.UserAgent.ParseAdd("Skua");
    }

    public CustomClient(string token)
    {
        DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);
        DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        DefaultRequestHeaders.UserAgent.ParseAdd("Skua/ScriptsUser");
    }
}

public static class HttpClients
{
    public static CustomClient GitHubClient { get; private set; } = new(true);
    public static CustomClient? UserGitHubClient { get; set; } = null;
    public static CustomClient GetMapClient { get; set; } = new(false);
    public static HttpClient Default { get; private set; } = new();

    public static HttpClient GetGHClient()
    {
        if (UserGitHubClient is not null)
            return UserGitHubClient;
        return GitHubClient;
    }
}

