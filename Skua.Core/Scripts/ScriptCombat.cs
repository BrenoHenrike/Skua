using Skua.Core.Interfaces;
using Skua.Core.Flash;

namespace Skua.Core.Scripts;
public partial class ScriptCombat : IScriptCombat
{
    public ScriptCombat(
        Lazy<IFlashUtil> flash,
        Lazy<IScriptOption> options,
        Lazy<IScriptWait> wait,
        Lazy<IScriptPlayer> player,
        Lazy<IScriptMap> map)
    {
        _lazyFlash = flash;
        _lazyOptions = options;
        _lazyWait = wait;
        _lazyPlayer = player;
        _lazyMap = map;
    }
    private readonly Lazy<IFlashUtil> _lazyFlash;
    private readonly Lazy<IScriptOption> _lazyOptions;
    private readonly Lazy<IScriptWait> _lazyWait;
    private readonly Lazy<IScriptPlayer> _lazyPlayer;
    private readonly Lazy<IScriptMap> _lazyMap;

    private IFlashUtil Flash => _lazyFlash.Value;
    private IScriptOption Options => _lazyOptions.Value;
    private IScriptWait Wait => _lazyWait.Value;
    private IScriptPlayer Player => _lazyPlayer.Value;
    private IScriptMap Map => _lazyMap.Value;

    [MethodCallBinding("world.approachTarget", GameFunction = true)]
    private void _approachTarget() { }

    [MethodCallBinding("untargetSelf")]
    private void _untargetSelf() { }

    [MethodCallBinding("world.cancelTarget", RunMethodPost = true, GameFunction = true)]
    private void _cancelTarget()
    {
        if (Options.SafeTimings)
            Wait.ForMonsterDeath();
    }

    [MethodCallBinding("world.cancelAutoAttack", GameFunction = true)]
    private void _cancelAutoAttack() { }

    public void Exit()
    {
        if (Player.State == 1)
            return;
        CancelAutoAttack();
        CancelTarget();
        Map.Jump(Player.Cell, Player.Pad);
        Thread.Sleep(300);
        Map.Jump(Player.Cell, Player.Pad);
        Wait.ForCombatExit();
    }

    [MethodCallBinding("attackMonsterName")]
    private void _attack(string name) { }

    [MethodCallBinding("attackMonsterID")]
    private void _attack(int id) { }

    [MethodCallBinding("attackPlayer")]
    private void _attackPlayer(string name) { }
}
