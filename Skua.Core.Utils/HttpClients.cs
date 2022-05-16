namespace Skua.Core.Utils;
public static class HttpClients
{
    public static GHHttpClient GitHubClient { get; private set; } = new();
    public static HttpClient Default { get; private set; } = new();
}

public class GHHttpClient : HttpClient
{
    public const string ClientID = "726820423be5c752df62";
    public const string ClientSecret = "63b2a5b1a55fbeade88deab3b6c8914808bad7a6";

    private readonly string _authString = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(ClientID + ":" + ClientSecret));

    public GHHttpClient()
    {
        DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", _authString);
        DefaultRequestHeaders.UserAgent.ParseAdd("Skua");
    }
}

