using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using Skua.Core.Models;
using Skua.Core.Utils;

namespace Skua.Core.ViewModels;
public partial class GrabberListViewModel : ObservableRecipient
{
    public GrabberListViewModel(string title, IGrabberService grabberService, GrabberTypes grabType, IEnumerable<GrabberTaskViewModel> commands, bool selectMultiple = false)
    {
        Title = title;
        SelectMultiple = selectMultiple;
        _grabberService = grabberService;
        _grabType = grabType;
        _grabberCommands = new(commands);
    }

    public GrabberListViewModel(string title, IGrabberService grabberService, GrabberTypes grabType, GrabberTaskViewModel command, bool selectMultiple = false)
    {
        Title = title;
        SelectMultiple = selectMultiple;
        _grabberService = grabberService;
        _grabType = grabType;
        _grabberCommands = new() { command };
    }
    public GrabberListViewModel(string title, IGrabberService grabberService, GrabberTypes grabType, bool selectMultiple = false)
    {
        Title = title;
        SelectMultiple = selectMultiple;
        _grabberService = grabberService;
        _grabType = grabType;
        _grabberCommands = new();
    }

    private readonly GrabberTypes _grabType;
    private readonly IGrabberService _grabberService;
    public string Title { get; }
    public bool SelectMultiple { get; }
    [ObservableProperty]
    private string _searchText = string.Empty;
    [ObservableProperty]
    private bool _isBusy = false;
    [ObservableProperty]
    private string _progressReportMessage = string.Empty;
    [ObservableProperty]
    private ObservableCollection<GrabberTaskViewModel> _grabberCommands;
    [ObservableProperty]
    private int _selectionMode;
    [ObservableProperty]
    private RangedObservableCollection<object> _grabbedItems = new();
    [ObservableProperty]
    private object? _selectedItem;

    [RelayCommand]
    private async Task Grab()
    {
        IsBusy = true;
        ProgressReportMessage = "Working...";
        try
        {
            IEnumerable<object> items = await Task.Run(() => _grabberService.Grab(_grabType));
            if (items.Any())
                GrabbedItems.ReplaceRange(items);
        }
        finally
        {
            IsBusy = false;
            ProgressReportMessage = "Finished.";
            await Task.Delay(1000);
            ProgressReportMessage = string.Empty;
        }
    }

    [RelayCommand]
    private void CancelTask()
    {
        WeakReferenceMessenger.Default.Send<CancelGrabberTaskMessage>();
    }

    protected override void OnActivated()
    {
        foreach(var task in GrabberCommands)
            task.IsActive = true;

        Messenger.Register<GrabberListViewModel, PropertyChangedMessage<bool>>(this, IsBusyChanged);
        Messenger.Register<GrabberListViewModel, PropertyChangedMessage<string>>(this, ProgressReportMessageChanged);
    }

    protected override void OnDeactivated()
    {
        foreach (var task in GrabberCommands)
            task.IsActive = false;

        base.OnDeactivated();
    }

    public void IsBusyChanged(GrabberListViewModel receiver, PropertyChangedMessage<bool> message)
    {
        if (message.Sender.GetType() == typeof(GrabberTaskViewModel)
            && message.PropertyName == nameof(GrabberTaskViewModel.IsBusy))
            receiver.IsBusy = message.NewValue;
    }

    public void ProgressReportMessageChanged(GrabberListViewModel receiver, PropertyChangedMessage<string> message)
    {
        if (message.Sender.GetType() == typeof(GrabberTaskViewModel)
            && message.PropertyName == nameof(GrabberTaskViewModel.ProgressReportMessage))
            receiver.ProgressReportMessage = message.NewValue;
    }
}
