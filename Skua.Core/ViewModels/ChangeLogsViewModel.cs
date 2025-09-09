using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Skua.Core.Interfaces;

namespace Skua.Core.ViewModels;

public class ChangeLogsViewModel : BotControlViewModelBase
{
    private string _markDownContent = "Loading content...";
    private bool _hasLoadedOnce = false;

    public ChangeLogsViewModel() : base("Change Logs", 460, 500)
    {
        _markDownContent = string.Empty;

        OpenPurpleDonationLink = new RelayCommand(() => Ioc.Default.GetRequiredService<IProcessService>().OpenLink("https://www.paypal.com/paypalme/sharpiiee?country.x=US&locale.x=en_US"));

        OpenBrenoHenrikeDonationLink = new RelayCommand(() => Ioc.Default.GetRequiredService<IProcessService>().OpenLink("https://www.paypal.com/donate/?hosted_button_id=QVQ4Q7XSH9VBY"));
    }

    public IRelayCommand OpenPurpleDonationLink { get; }
    public IRelayCommand OpenBrenoHenrikeDonationLink { get; }

    public string MarkdownDoc
    {
        get { return _markDownContent; }
        set { SetProperty(ref _markDownContent, value); }
    }

    private async Task GetChangeLogsContent()
    {
        using (var client = new HttpClient())
        {
            client.Timeout = TimeSpan.FromSeconds(10);

            try
            {
                var response = await client.GetAsync("https://raw.githubusercontent.com/BrenoHenrike/Skua/refs/heads/master/changelogs.md").ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        MarkdownDoc = content;
                        return;
                    }
                }

                // If response was not successful, show error message
                MarkdownDoc = $"### Unable to Load Changelog\r\n\r\nFailed to load changelog (HTTP {response.StatusCode}).\r\n\r\nPlease check your internet connection and try again later.\r\n\r\nYou can also view the latest releases at: [Skua Releases](https://github.com/BrenoHenrike/Skua/releases)";
            }
            catch (Exception ex)
            {
                // Show error message with exception details for debugging
                MarkdownDoc = $"### Unable to Load Changelog\r\n\r\nError: {ex.Message}\r\n\r\nThis might be due to:\r\n- No internet connection\r\n- GitHub service issues\r\n- Repository access problems\r\n\r\nPlease check your internet connection and try again later.\r\n\r\nYou can also view the latest releases at: [Skua Releases](https://github.com/BrenoHenrike/Skua/releases)";
            }
        }
    }

    private async Task RefreshChangelogContent()
    {
        MarkdownDoc = "Refreshing changelog...";
        await GetChangeLogsContent();
    }

    public async void OnActivated()
    {
        MarkdownDoc = "Loading changelog...";
        await GetChangeLogsContent();
        _hasLoadedOnce = true;
    }
}