using System.Collections.ObjectModel;
using Microsoft.Toolkit.Mvvm.Input;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;

namespace Skua.Core.ViewModels;
public class GrabberViewModel : BotControlViewModelBase
{
    public GrabberViewModel(IEnumerable<GrabberListViewModel> grabberTabs, IDialogService dialogService)
        : base("Grabber")
    {
        _grabberTabs = new(grabberTabs);
        _selectedTab = _grabberTabs[0];
        _dialogService = dialogService;
    }
    private ObservableCollection<GrabberListViewModel> _grabberTabs;
    public ObservableCollection<GrabberListViewModel> GrabberTabs
    {
        get { return _grabberTabs; }
        set { SetProperty(ref _grabberTabs, value); }
    }
    private GrabberListViewModel _selectedTab;
    public GrabberListViewModel SelectedTab
    {
        get { return _selectedTab; }
        set { SetProperty(ref _selectedTab, value); }
    }

    private bool _isDialogOpen;
    public bool IsDialogOpen
    {
        get { return _isDialogOpen; }
        set { SetProperty(ref _isDialogOpen, value); }
    }
    private string _dialogHint;
    public string DialogHint
    {
        get { return _dialogHint; }
        set { SetProperty(ref _dialogHint, value); }
    }
    private string _dialogHeader;
    public string DialogHeader
    {
        get { return _dialogHeader; }
        set { SetProperty(ref _dialogHeader, value); }
    }
    private string _dialogTextInput;
    private readonly IDialogService _dialogService;

    public string DialogTextInput
    {
        get { return _dialogTextInput; }
        set { SetProperty(ref _dialogTextInput, value); }
    }

    public IRelayCommand DialogConfirmCommand { get; }
    public IRelayCommand DialogCancelCommand { get; }

    private void Receive(GrabberViewModel receiver, GrabberTaskRequest request)
    {

        if (int.TryParse(_dialogTextInput, out int result))
            request.Reply(result);
        else
            request.Reply(-1);
    }
}
