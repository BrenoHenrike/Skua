using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Skua.Core.Messaging;
using Skua.Core.Models.GitHub;

namespace Skua.Core.ViewModels.Manager;
public partial class ClientUpdateItemViewModel : ObservableObject
{
    public ClientUpdateItemViewModel(UpdateInfo info)
    {
        Info = info;
    }

    public UpdateInfo Info { get; }

    [RelayCommand]
    public void Download()
    {
        StrongReferenceMessenger.Default.Send<DownloadClientUpdateMessage>(new(Info));
    }
}
