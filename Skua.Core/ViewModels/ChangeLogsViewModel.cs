using Skua.Core.Utils;

namespace Skua.Core.ViewModels;
public class ChangeLogsViewModel : BotControlViewModelBase
{
    private string _markDownContent = "Loading content...";
    
    public ChangeLogsViewModel() : base("Change Logs")
    {
        _markDownContent = string.Empty;
        Task.Run(async () => await GetChangeLogsContent());
    }

    public string MarkdownDoc
    {
        get { return _markDownContent; }
        set { SetProperty(ref _markDownContent, value); }
    }

    private async Task GetChangeLogsContent()
    {
        var response = await HttpClients.Default.GetAsync("https://raw.githubusercontent.com/BrenoHenrike/Skua/op-version/changelogs.md");
        if (!response.IsSuccessStatusCode)
            MarkdownDoc = "### No content found. Please check your internet connection.";

        MarkdownDoc = await response.Content.ReadAsStringAsync();
    }
}
