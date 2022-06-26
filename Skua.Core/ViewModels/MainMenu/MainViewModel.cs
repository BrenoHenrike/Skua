
using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace Skua.Core.ViewModels;
public sealed class MainViewModel : ObservableRecipient
{
    private string _title = "Skua";
    public string Title
    {
        get { return _title; }
        set{ SetProperty(ref _title, value); }
    }

    public MainViewModel()
    {
        
    }
}
