using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skua.Core.Interfaces;

public delegate void Command(params string[] arguments);

public interface IScriptSync
{
    void SendCommandToAll(string command, params string[] arguments);
    void SendCommand(int id, string command, params string[] arguments);
    bool RegisterCommand(string command, Command action);
    void OverrideCommand(string command, Command action);
    bool RemoveCommand(string command);
    void ClearCommands();
}
