using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Skua.Core.Messaging;

namespace Skua.Core.ViewModels;
public partial class GrabberTaskViewModel : ObservableRecipient
{
    public GrabberTaskViewModel(string content, Func<IList<object>?, IProgress<string>, CancellationToken, Task> command)
    {
        Content = content;
        _command = command;
    }

    protected override void OnActivated()
    {
        Messenger.Register<GrabberTaskViewModel, CancelGrabberTaskMessage>(this, CancelTask);
    }

    private readonly Func<IList<object>?, IProgress<string>, CancellationToken, Task> _command;
    [ObservableProperty]
    [NotifyPropertyChangedRecipients]
    private string _progressReportMessage = string.Empty;
    [ObservableProperty]
    [NotifyPropertyChangedRecipients]
    private bool _isBusy;

    public string Content { get; }

    [RelayCommand]
    private async Task GrabberTask(IList<object>? items, CancellationToken token)
    {
        IsBusy = true;
        Progress<string> progress = new(ProgressReport);
        await _command(items, progress, token);
        IsBusy = false;
        await Task.Delay(1000, token);
        ProgressReportMessage = string.Empty;
    }

    private void ProgressReport(string progress)
    {
        ProgressReportMessage = progress;
    }

    private void CancelTask(GrabberTaskViewModel receiver, CancelGrabberTaskMessage message)
    {
        receiver.GrabberTaskCommand.Cancel();
    }
}
