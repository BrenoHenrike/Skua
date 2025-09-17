using System.Net;

namespace Skua.Core.Utils;

/// <summary>
/// HttpClient
/// </summary>
public class WebClient : HttpClient
{
    //private readonly string _authString1 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("726820423be5c752df62:63b2a5b1a55fbeade88deab3b6c8914808bad7a6"));
    private readonly string _authString2 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("449f889db3d655d2ef4a:27863d426bc5bb46c410daf7ed6b479ba4a9f7eb"));

    /// <param name="accJson"></param>
    public WebClient(bool accJson)
    {
        DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", _authString2);
        if (accJson)
            DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        DefaultRequestHeaders.UserAgent.ParseAdd("Skua");
    }

    /// <param name="token"></param>
    public WebClient(string token)
    {
        DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);
        DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        DefaultRequestHeaders.UserAgent.ParseAdd("Skua/ScriptsUser");
    }
}

/// <summary>
/// All HttpClients
/// </summary>
public static class HttpClients
{

    static HttpClients()
    {
        ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
    }

    /// <summary>
    /// Gets the GitHub Client
    /// </summary>
    public static WebClient GitHubClient { get; private set; } = new(true);

    /// <summary>
    /// Gets the GitHub User Client
    /// </summary>
    public static WebClient? UserGitHubClient { get; set; } = null;

    /// <summary>
    /// Gets the Map Client
    /// </summary>
    public static WebClient GetMapClient { get; set; } = new(false);

    /// <summary>
    /// Default HttpClient
    /// </summary>
    public static HttpClient Default { get; private set; } = new();

    /// <summary>
    /// Gets either the GitHub or Github User Client
    /// </summary>
    /// <returns>Client Type</returns>
    public static HttpClient GetGHClient()
    {
        return UserGitHubClient is not null ? UserGitHubClient : (HttpClient)GitHubClient;
    }
}