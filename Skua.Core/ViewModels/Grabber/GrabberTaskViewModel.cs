using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Skua.Core.Messaging;

namespace Skua.Core.ViewModels;
public partial class GrabberTaskViewModel : ObservableRecipient
{
    public GrabberTaskViewModel(string content, Func<IList<object>?, IProgress<string>, CancellationToken, Task> command)
    {
        Messenger.Register<GrabberTaskViewModel, CancelGrabberTaskMessage>(this, CancelTask);
        Content = content;
        _command = command;
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
        await Task.Delay(1000);
        ProgressReportMessage = string.Empty;
        IsBusy = false;
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
