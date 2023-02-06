using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Skua.Core.Interfaces;
using Skua.Core.Utils;

namespace Skua.Core.ViewModels;
public class ChangeLogsViewModel : BotControlViewModelBase
{
    private string _markDownContent = "Loading content...";
    
    public ChangeLogsViewModel() : base("Change Logs", 460, 500)
    {
        _markDownContent = string.Empty;
        Task.Run(async () => await GetChangeLogsContent());

        OpenDelfinaDonationLink = new RelayCommand(() => Ioc.Default.GetRequiredService<IProcessService>().OpenLink("https://www.paypal.com/donate/?hosted_button_id=DMZFDRYJ5BT96"));
        
        OpenBrenoHenrikeDonationLink = new RelayCommand(() => Ioc.Default.GetRequiredService<IProcessService>().OpenLink("https://www.paypal.com/donate/?hosted_button_id=QVQ4Q7XSH9VBY"));
    }

    public IRelayCommand OpenDelfinaDonationLink { get; }
    public IRelayCommand OpenBrenoHenrikeDonationLink { get; }

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
