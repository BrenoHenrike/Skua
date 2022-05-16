using Skua.Core.Interfaces;
using Skua.Core.Models.Factions;
using Skua.Core.PostSharp;

namespace Skua.Core.Scripts;
public class ScriptFaction : ScriptableObject, IScriptFaction
{
    [ObjectBinding("world.myAvatar.factions")]
    public List<Faction> FactionList { get; } = new();
}
