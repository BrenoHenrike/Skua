using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using Skua.Core.Interfaces;
using Skua.Core.Models.GitHub;
using Skua.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skua.Core.ViewModels.Manager;
public class GoalsViewModel : BotControlViewModelBase
{
    public GoalsViewModel()
        : base("Goals")
    {
        CopyEmailCommand = new RelayCommand(() => Ioc.Default.GetRequiredService<IClipboardService>().SetText("bhenrike@protonmail.com"));
        OpenDonationLinkCommand = new RelayCommand(() => Ioc.Default.GetRequiredService<IProcessService>().OpenLink("https://www.paypal.com/donate/?hosted_button_id=QVQ4Q7XSH9VBY"));
    }

    public IRelayCommand CopyEmailCommand { get; }
    public IRelayCommand OpenDonationLinkCommand { get; }

    protected override void OnActivated()
    {
        if (Goals.Count == 0)
            GetGoals();
    }

    private async Task GetGoals()
    {
        var response = await HttpClients.Default.GetAsync("https://raw.githubusercontent.com/brenohenrike/skua/master/goals");
        if (!response.IsSuccessStatusCode)
        {
            Status = "Failed to fetch data.";
            return;
        }

        List<GoalObject>? goals = JsonConvert.DeserializeObject<List<GoalObject>>(await response.Content.ReadAsStringAsync());

        if (goals is null || goals.Count == 0)
        {
            Status = "Failed to parse data.";
            return;
        }

        Goals.AddRange(goals);
    }

    private string _status = "Loading...";
    public string Status
    {
        get { return _status; }
        set { SetProperty(ref _status, value); }
    }


    public RangedObservableCollection<GoalObject> Goals { get; } = new();

}
