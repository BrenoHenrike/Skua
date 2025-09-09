using System.Diagnostics;

namespace Skua.Core.Messaging;

public class AddProcessMessage
{
    public AddProcessMessage(Process process, string accountName)
    {
        Process = process;
        AccountName = accountName;
    }

    public Process Process { get; }
    public string AccountName { get; }
}