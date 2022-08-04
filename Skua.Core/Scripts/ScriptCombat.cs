using Skua.Core.Interfaces;
using Skua.Core.Flash;
using Microsoft.Toolkit.Mvvm.Messaging;
using Skua.Core.Messaging;
using Skua.Core.Models.Monsters;

namespace Skua.Core.Scripts;
public partial class ScriptCombat : IScriptCombat
{
    public ScriptCombat(
        Lazy<IFlashUtil> flash,
        Lazy<IScriptOption> options,
        Lazy<IScriptWait> wait,
        Lazy<IScriptPlayer> player,
        Lazy<IScriptMap> map,
        IMessenger messenger)
    {
        _lazyFlash = flash;
        _lazyOptions = options;
        _lazyWait = wait;
        _lazyPlayer = player;
        _lazyMap = map;
        _messenger = messenger;

        _messenger.Register<ScriptCombat, CounterAttackMessage>(this, CounterAttack);
        _messenger.Register<ScriptCombat, PlayerDeathMessage>(this, PlayerDead);
        _messenger.Register<ScriptCombat, ScriptStoppedMessage>(this, ScriptStopped);
    }

    private readonly Lazy<IFlashUtil> _lazyFlash;
    private readonly Lazy<IScriptOption> _lazyOptions;
    private readonly Lazy<IScriptWait> _lazyWait;
    private readonly Lazy<IScriptPlayer> _lazyPlayer;
    private readonly Lazy<IScriptMap> _lazyMap;
    private readonly IMessenger _messenger;

    private IFlashUtil Flash => _lazyFlash.Value;
    private IScriptOption Options => _lazyOptions.Value;
    private IScriptWait Wait => _lazyWait.Value;
    private IScriptPlayer Player => _lazyPlayer.Value;
    private IScriptMap Map => _lazyMap.Value;

    public bool StopAttacking { get; set; }

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

    [MethodCallBinding("attackMonsterName", RunMethodPre = true)]
    private void _attack(string name)
    {
        if (StopAttacking)
            return;
    }

    [MethodCallBinding("attackMonsterID", RunMethodPre = true)]
    private void _attack(int id)
    {
        if (StopAttacking)
            return;
    }

    [MethodCallBinding("attackPlayer")]
    private void _attackPlayer(string name) { }

    private void PlayerDead(ScriptCombat recipient, PlayerDeathMessage message)
    {
        recipient.StopAttacking = false;
    }

    private void ScriptStopped(ScriptCombat recipient, ScriptStoppedMessage message)
    {
        recipient.StopAttacking = false;
    }
    Monster? _target;
    private void CounterAttack(ScriptCombat recipient, CounterAttackMessage message)
    {
        if (message.Faded)
        {
            recipient.StopAttacking = false;
            if (recipient._target is not null)
                recipient.Attack(recipient._target.ID);
            recipient._target = null;
            return;
        }

        recipient.StopAttacking = true;
        recipient._target = recipient.Player.Target;
        recipient.CancelAutoAttack();
        recipient.CancelTarget();
    }
}
