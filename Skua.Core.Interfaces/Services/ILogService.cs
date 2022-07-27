using System.ComponentModel;
using Skua.Core.Models;

namespace Skua.Core.Interfaces;
public interface ILogService : INotifyPropertyChanged
{
    void DebugLog(string message);
    void ScriptLog(string message);
    void FlashLog(string message);
    void ClearLog(LogType logType);
    List<string> GetLogs(LogType logType);
}
