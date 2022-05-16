using Skua.Core.Interfaces;

namespace Skua.Core.Scripts;

public class ScriptBotStats : IScriptBotStats
{
    public int Kills { get; set; }
    public int QuestsAccepted { get; set; }
    public int QuestsCompleted { get; set; }
    public int Deaths { get; set; }
    public int Relogins { get; set; }
    public int Drops { get; set; }
}
