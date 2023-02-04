using Skua.Core.Utils;

namespace Skua.Core.ViewModels;
public class AboutViewModel : BotControlViewModelBase
{
    private string _markDownContent;
    
    public AboutViewModel() : base("About")
    {
        _markDownContent = string.Empty;
    }

    protected override void OnActivated()
    {
        GetAboutContent();
    }

    private async Task GetAboutContent()
    {
        var response = await HttpClients.Default.GetAsync("https://raw.githubusercontent.com/BrenoHenrike/Skua/op-version/about.md");
        if (!response.IsSuccessStatusCode)
            _markDownContent = "### No content found. Please check your internet connection.";
        
        _markDownContent = await response.Content.ReadAsStringAsync();
    }
    
    public string MarkdownDoc
    {
        get => _markDownContent;
        set
        {
            _markDownContent = value;
            OnPropertyChanged(nameof(MarkdownDoc));
        }
    }
}
