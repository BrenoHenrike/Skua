using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Numerics;
using CommunityToolkit.Mvvm.ComponentModel;
using Skua.Core.Interfaces;
using Skua.Core.Models.Monsters;
using Skua.Core.Models.Skills;

namespace Skua.Core.Scripts;
public partial class ScriptAuto : ObservableObject, IScriptAuto
{
    public ScriptAuto(
        ILogService logger,
        Lazy<IScriptPlayer> player,
        Lazy<IScriptDrop> drops,
        Lazy<IScriptSkill> skills,
        Lazy<IScriptBoost> boosts,
        Lazy<IScriptOption> options,
        Lazy<IScriptMonster> monsters,
        Lazy<IScriptKill> kill,
        Lazy<IScriptWait> wait,
        Lazy<IScriptCombat> lazyCombat,
        Lazy<IScriptMap> lazyMap)
    {
        _logger = logger;
        _lazyPlayer = player;
        _lazyDrops = drops;
        _lazySkills = skills;
        _lazyBoosts = boosts;
        _lazyOptions = options;
        _lazyMonsters = monsters;
        _lazyKill = kill;
        _lazyWait = wait;
        _lazyCombat = lazyCombat;
        _lazyMap = lazyMap;
        _lastHuntTick = Environment.TickCount;
    }

    private readonly ILogService _logger;
    private readonly Lazy<IScriptPlayer> _lazyPlayer;
    private readonly Lazy<IScriptDrop> _lazyDrops;
    private readonly Lazy<IScriptSkill> _lazySkills;
    private readonly Lazy<IScriptBoost> _lazyBoosts;
    private readonly Lazy<IScriptOption> _lazyOptions;
    private readonly Lazy<IScriptKill> _lazyKill;
    private readonly Lazy<IScriptMonster> _lazyMonsters;
    private readonly Lazy<IScriptWait> _lazyWait;
    private readonly Lazy<IScriptCombat> _lazyCombat;
    private readonly Lazy<IScriptMap> _lazyMap;
    private IScriptPlayer Player => _lazyPlayer.Value;
    private IScriptDrop Drops => _lazyDrops.Value;
    private IScriptSkill Skills => _lazySkills.Value;
    private IScriptBoost Boosts => _lazyBoosts.Value;
    private IScriptOption Options => _lazyOptions.Value;
    private IScriptKill Kill => _lazyKill.Value;
    private IScriptMonster Monsters => _lazyMonsters.Value;
    private IScriptWait Wait => _lazyWait.Value;
    private IScriptCombat Combat => _lazyCombat.Value;
    private IScriptMap Map => _lazyMap.Value;

    [ObservableProperty]
    private bool _isRunning;

    private Task? _autoTask;
    private CancellationTokenSource? _ctsAuto;

    public void StartAutoAttack(string? className = null, ClassUseMode classUseMode = ClassUseMode.Base)
    {
        _ctsAuto = new CancellationTokenSource();
        _DoActionAuto(hunt: false, className, classUseMode);
    }
    
    public void StartAutoHunt(string? className = null, ClassUseMode classUseMode = ClassUseMode.Base)
    {
        _ctsAuto = new CancellationTokenSource();
        _DoActionAuto(hunt: true, className, classUseMode);
    }

    public void Stop()
    {
        if(_ctsAuto is null)
        {
            IsRunning = false;
            return;
        }
        
        _ctsAuto?.Cancel();
        _autoTask?.Wait();
        Wait.ForTrue(() => _ctsAuto is null, 20);
        _autoTask?.Dispose();
        IsRunning = false;
    }

    public async ValueTask StopAsync()
    {
        if(_ctsAuto is null)
        {
            IsRunning = false;
            return;
        }
        
        _ctsAuto?.Cancel();
        await Wait.ForTrueAsync(() => _ctsAuto is null && (_autoTask?.IsCompleted ?? true), 40);
        _autoTask?.Dispose();
        IsRunning = false;
    }

    private void _DoActionAuto(bool hunt, string? className = null, ClassUseMode classUseMode = ClassUseMode.Base)
    {
        if (_autoTask != null && !_autoTask.IsCompleted)
            return;

        if (!Player.LoggedIn)
            return;

        CheckDropsandBoosts();
        if (className is not null)
            Skills.StartAdvanced(className, true, classUseMode);
        else
            Skills.StartAdvanced(Player.CurrentClass?.Name ?? "Generic", true);

        _autoTask = Task.Run(async () =>
        {
            try
            {
                if (hunt)
                    await _Hunt(_ctsAuto!.Token);
                else
                    await _Attack(_ctsAuto!.Token);
            }
            catch { }
            finally
            {
                Drops.Stop();
                Skills.Stop();
                Boosts.Stop();
                _ctsAuto?.Dispose();
                _ctsAuto = null;
                IsRunning = false;
            }
        });
        IsRunning = true;
    }

    private string _target = "";
    private async Task _Attack(CancellationToken token)
    {
        Trace.WriteLine("Auto attack started.");
        Player.SetSpawnPoint();

        var monsters = Monsters.CurrentMonsters;

        while (!token.IsCancellationRequested)
        {
            foreach (var monster in monsters)
            {
                if (token.IsCancellationRequested)
                    return;

                if (!Combat.Attack(monster.MapID))
                    continue;

                Kill.Monster(monster.MapID, token);
            }
        }
        Trace.WriteLine("Auto attack stopped.");
    }

    private int _lastHuntTick;

    private async Task _Hunt(CancellationToken token)
    {
        Trace.WriteLine("Auto hunt started.");

        if (Player.HasTarget)
            _target = Player.Target?.Name ?? "*";
        else
        {
            List<string> monsters = Monsters.CurrentMonsters.Select(m => m.Name).ToList();
            _target = string.Join('|', monsters);
        }

        var names = _target.Split('|');
        List<string> cells = names.SelectMany(n => Monsters.GetLivingMonsterDataLeafCells(n)).Distinct().ToList();

        _logger.ScriptLog($"[Auto Hunt] Hunting for {_target}");
        while (!token.IsCancellationRequested)
        {
            for (int i = cells.Count - 1; i >= 0; i--)
            {
                if (Player.Cell != cells[i] && !token.IsCancellationRequested)
                {
                    if (Environment.TickCount - _lastHuntTick < Options.HuntDelay)
                        Thread.Sleep(Options.HuntDelay - Environment.TickCount + _lastHuntTick);
                    Map.Jump(cells[i], "Left");
                    _lastHuntTick = Environment.TickCount;
                }

                foreach (string mon in names)
                {
                    if (token.IsCancellationRequested)
                        break;

                    if (Monsters.Exists(mon) && !token.IsCancellationRequested)
                    {
                        if (!Combat.Attack(mon))
                        {
                            cells.RemoveAt(i);
                            continue;
                        }
                        Thread.Sleep(Options.ActionDelay);
                        Kill.Monster(mon, token);
                        break;
                    }
                    else
                    {
                        cells.RemoveAt(i);
                    }
                }
            }
        }
        Trace.WriteLine("Auto hunt stopped.");
    }

    private void CheckDropsandBoosts()
    {
        if (Drops.ToPickup.Any())
            Drops.Start();

        if (Boosts.UsingBoosts)
            Boosts.Start();
    }
}
