using System.Collections.ObjectModel;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Toolkit.Mvvm.Messaging.Messages;
using Skua.Core.Interfaces.Services;
using Skua.Core.Messaging;
using Skua.Core.Models;
using Skua.Core.Utils;

namespace Skua.Core.ViewModels;
public class GrabberListViewModel : ObservableRecipient
{
    public GrabberListViewModel(string title, IGrabberService grabberService, GrabberTypes grabType, IEnumerable<GrabberTaskViewModel> commands, bool selectMultiple = false)
    {
        Messenger.Register<GrabberListViewModel, PropertyChangedMessage<bool>>(this, Receive);
        Messenger.Register<GrabberListViewModel, PropertyChangedMessage<string>>(this, Receive);
        Title = title;
        SelectMultiple = selectMultiple;
        _grabberService = grabberService;
        _grabType = grabType;
        _grabberCommands = new(commands);
        _grabbedItems = new(new[] { title });
        GrabCommand = new AsyncRelayCommand(Grab);
        CancelTaskCommand = new RelayCommand(() =>
        {
            Messenger.Send<CancelGrabberTaskMessage>();
        });
    }
    public IRelayCommand CancelTaskCommand { get; }
    private readonly GrabberTypes _grabType;
    private readonly IGrabberService _grabberService;
    public string Title { get; }
    public bool SelectMultiple { get; }
    private bool _isBusy = false;
    public bool IsBusy
    {
        get { return _isBusy; }
        set { SetProperty(ref _isBusy, value); }
    }
    private string _progressReportMessage = string.Empty;
    public string ProgressReportMessage
    {
        get { return _progressReportMessage; }
        set { SetProperty(ref _progressReportMessage, value, true); }
    }

    private ObservableCollection<GrabberTaskViewModel> _grabberCommands;
    public ObservableCollection<GrabberTaskViewModel> GrabberCommands
    {
        get { return _grabberCommands; }
        set { SetProperty(ref _grabberCommands, value); }
    }

    private int _selectionMode;
    public int SelectionMode
    {
        get { return _selectionMode; }
        set { SetProperty(ref _selectionMode, value); }
    }

    private RangedObservableCollection<object> _grabbedItems;
    public RangedObservableCollection<object> GrabbedItems
    {
        get { return _grabbedItems; }
        set { SetProperty(ref _grabbedItems, value); }
    }

    private object? _selectedItem;
    public object? SelectedItem
    {
        get { return _selectedItem; }
        set { SetProperty(ref _selectedItem, value); }
    }

    public IAsyncRelayCommand GrabCommand { get; }
    private async Task Grab()
    {
        IsBusy = true;
        try
        {
            await Task.Delay(100);
            IEnumerable<object> items = _grabberService.Grab(_grabType);
            if (items.Any())
                GrabbedItems.ReplaceRange(items);
        }
        finally
        {
            IsBusy = false;
        }
    }

    public void Receive(GrabberListViewModel receiver, PropertyChangedMessage<bool> message)
    {
        if (message.Sender.GetType() == typeof(GrabberTaskViewModel)
            && message.PropertyName == nameof(GrabberTaskViewModel.IsBusy))
            receiver.IsBusy = message.NewValue;
    }

    public void Receive(GrabberListViewModel receiver, PropertyChangedMessage<string> message)
    {
        if (message.Sender.GetType() == typeof(GrabberTaskViewModel)
            && message.PropertyName == nameof(GrabberTaskViewModel.ProgressReportMessage))
            receiver.ProgressReportMessage = message.NewValue;
    }
}
