using Skua.Core.Interfaces;
using Skua.Core.Models.Factions;
using Skua.Core.Flash;

namespace Skua.Core.Scripts;
public partial class ScriptFaction : IScriptFaction
{
    private readonly Lazy<IFlashUtil> _lazyFlash;
    private IFlashUtil Flash => _lazyFlash.Value;
    public ScriptFaction(Lazy<IFlashUtil> flash)
    {
        _lazyFlash = flash;
    }

    [ObjectBinding("world.myAvatar.factions", Default = "new()", HasSetter = false)]
    private List<Faction> _factionList = new();
}
