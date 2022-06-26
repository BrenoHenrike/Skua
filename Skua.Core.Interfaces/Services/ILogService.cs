using System.ComponentModel;
using Skua.Core.Models;

namespace Skua.Core.Interfaces;
public interface ILogService : INotifyPropertyChanged, IDisposable
{
    string DebugLogs { get; }
    string ScriptLogs { get; }
    string FlashLogs { get; }
    void DebugLog(string message);
    void ScriptLog(string message);
    void FlashLog(string message);
    void ClearLog(LogType logType);
}
