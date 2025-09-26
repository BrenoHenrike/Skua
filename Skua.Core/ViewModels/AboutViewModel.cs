using CommunityToolkit.Mvvm.Input;
using Skua.Core.Utils;
using System.Diagnostics;

namespace Skua.Core.ViewModels;

public class AboutViewModel : BotControlViewModelBase
{
    private string _markDownContent = "Loading content...";

    public AboutViewModel() : base("About")
    {
        _markDownContent = string.Empty;

        Task.Run(async () => await GetAboutContent());

        NavigateCommand = new RelayCommand<string>(url => Process.Start(new ProcessStartInfo(url) { UseShellExecute = true }));
    }

    public string MarkdownDoc
    {
        get { return _markDownContent; }
        set { SetProperty(ref _markDownContent, value); }
    }

    public IRelayCommand NavigateCommand { get; }

    private async Task GetAboutContent()
    {
        var response = await HttpClients.GitHubRaw.GetAsync("auqw/Skua/refs/heads/master/readme.md");
        if (!response.IsSuccessStatusCode)
            MarkdownDoc = "### No content found. Please check your internet connection.";

        MarkdownDoc = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
    }
}