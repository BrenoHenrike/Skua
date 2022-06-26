namespace Skua.Core.Models.GitHub;

public class ScriptRepo
{
    public ScriptRepo(string username, string name, string extension, string author)
    {
        Author = author;
        Username = username;
        Name = name;
        Extension = extension;
    }

    public string Author { get; }
    public string Username { get; }
    public string Name { get; }
    public string Extension { get; }
    public string ContentsUrl => $"https://api.github.com/repos/{Username}/{Name}/contents/{Extension}";
    public string CommitsUrl => $"https://api.github.com/repos/{Username}/{Name}/commits";
    public string RecursiveTreeUrl { get; set; } = string.Empty;

    public string GetContentUrl(string extension)
    {
        return $"https://api.github.com/repos/{Username}/{Name}/contents/{extension}";
    }
}
