using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;

namespace Skua.Core.Models;

public partial class ProcessInfo : ObservableObject
{
    public ProcessInfo(Process process, string accountName)
    {
        Process = process;
        AccountName = accountName;
    }

    public Process Process { get; }

    [ObservableProperty]
    private string _accountName;

    public int ProcessId => Process.Id;

    public bool HasExited
    {
        get
        {
            try
            {
                return Process.HasExited;
            }
            catch
            {
                return true;
            }
        }
    }
}