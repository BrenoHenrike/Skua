namespace Skua.Core.Utils;
public class CustomClient : HttpClient
{
    public const string ClientID1 = "726820423be5c752df62";
    public const string ClientID2 = "449f889db3d655d2ef4a";
    public const string ClientSecret1 = "63b2a5b1a55fbeade88deab3b6c8914808bad7a6";
    public const string ClientSecret2 = "27863d426bc5bb46c410daf7ed6b479ba4a9f7eb";

    private readonly string _authString1 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(ClientID1 + ":" + ClientSecret1));
    private readonly string _authString2 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(ClientID2 + ":" + ClientSecret2));

    public CustomClient(bool use, bool accJson)
    {
        DefaultRequestHeaders.Authorization = use
            ? new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", _authString1)
            : new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", _authString2);
        if (accJson)
            DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        DefaultRequestHeaders.UserAgent.ParseAdd("RBot/Scripts");
    }

    public CustomClient(string token)
    {
        DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);
        DefaultRequestHeaders.UserAgent.ParseAdd("RBot/ScriptsUser");
    }
}

public static class HttpClients
{
    private static bool _use;
    public static CustomClient GitHubClient1 { get; private set; } = new(true, true);
    public static CustomClient GitHubClient2 { get; private set; } = new(false, true);
    public static CustomClient? UserGitHubClient { get; set; } = null;
    public static CustomClient GetMapClient { get; set; } = new(true, false);
    public static HttpClient Default { get; private set; } = new();

    public static HttpClient GetGHClient()
    {
        if (UserGitHubClient is not null)
            return UserGitHubClient;
        _use = !_use;
        return _use ? GitHubClient1 : GitHubClient2;
    }
}

