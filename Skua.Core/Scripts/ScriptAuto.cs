using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using Skua.Core.Interfaces;
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
        Lazy<IScriptHunt> hunt,
        Lazy<IScriptWait> wait)
    {
        _logger = logger;
        _lazyPlayer = player;
        _lazyDrops = drops;
        _lazySkills = skills;
        _lazyBoosts = boosts;
        _lazyOptions = options;
        _lazyMonsters = monsters;
        _lazyKill = kill;
        _lazyHunt = hunt;
        _lazyWait = wait;
    }
    private CancellationTokenSource? _ctsAuto;
    private Thread? _autoThread;
    private readonly ILogService _logger;
    private readonly Lazy<IScriptPlayer> _lazyPlayer;
    private readonly Lazy<IScriptDrop> _lazyDrops;
    private readonly Lazy<IScriptSkill> _lazySkills;
    private readonly Lazy<IScriptBoost> _lazyBoosts;
    private readonly Lazy<IScriptOption> _lazyOptions;
    private readonly Lazy<IScriptKill> _lazyKill;
    private readonly Lazy<IScriptMonster> _lazyMonsters;
    private readonly Lazy<IScriptHunt> _lazyHunt;
    private readonly Lazy<IScriptWait> _lazyWait;
    private IScriptPlayer _player => _lazyPlayer.Value;
    private IScriptDrop _drops => _lazyDrops.Value;
    private IScriptSkill _skills => _lazySkills.Value;
    private IScriptBoost _boosts => _lazyBoosts.Value;
    private IScriptOption _options => _lazyOptions.Value;
    private IScriptKill _kill => _lazyKill.Value;
    private IScriptMonster _monsters => _lazyMonsters.Value;
    private IScriptHunt _hunt => _lazyHunt.Value;
    private IScriptWait _wait => _lazyWait.Value;

    [ObservableProperty]
    private bool _isRunning;

    public void StartAutoAttack(string? className = null, ClassUseMode classUseMode = ClassUseMode.Base)
    {
        StartAuto(false, className, classUseMode);
    }

    public void StartAutoHunt(string? className = null, ClassUseMode classUseMode = ClassUseMode.Base)
    {
        StartAuto(true, className, classUseMode);
    }

    public void Stop()
    {
        if(_ctsAuto is null)
        {
            IsRunning = false;
            return;
        }
        
        _ctsAuto?.Cancel();
        _wait.ForTrue(() => _ctsAuto is null, 20);
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
        await _wait.ForTrueAsync(() => _ctsAuto is null, 20);
        IsRunning = false;
    }

    private void StartAuto(bool hunt, string? className = null, ClassUseMode classUseMode = ClassUseMode.Base)
    {
        if (_autoThread?.IsAlive ?? false)
            return;

        if (!_player.LoggedIn)
            return;

        CheckDropsandBoosts();
        if(className is not null)
            _skills.StartAdvanced(className, true, classUseMode);
        else
            _skills.StartAdvanced(_player.CurrentClass?.Name ?? "Generic", true);

        _autoThread = new(async () =>
        {
            _ctsAuto = new();
            try
            {
                if (hunt)
                    await _Hunt(_ctsAuto.Token);
                else
                    await _Attack(_ctsAuto.Token);
            }
            catch { }
            _drops.Stop();
            _skills.Stop();
            _boosts.Stop();
            _ctsAuto?.Dispose();
            _ctsAuto = null;
        });
        _autoThread.Name = "Auto Thread";
        _autoThread.Start();
        IsRunning = true;
    }

    private string _target = "";
    private async Task _Attack(CancellationToken token)
    {
        Trace.WriteLine("Auto attack started.");
        _player.SetSpawnPoint();
        while (!token.IsCancellationRequested)
        {
            if (!_options.AttackWithoutTarget)
                _kill.Monster("*", token);
            await Task.Delay(500, token);
        }
        Trace.WriteLine("Auto attack stopped.");
    }

    private async Task _Hunt(CancellationToken token)
    {
        Trace.WriteLine("Auto hunt started.");

        if (_player.HasTarget)
            _target = _player.Target?.Name ?? "*";
        else
        {
            List<string> monsters = _monsters.CurrentMonsters.Select(m => m.Name).ToList();
            _target = string.Join('|', monsters);
        }

        _logger.ScriptLog($"[Auto Hunt] Hunting for {_target}");
        while (!token.IsCancellationRequested)
        {
            _hunt.Monster(_target, token);
            await Task.Delay(500, token);
        }
        Trace.WriteLine("Auto hunt stopped.");
    }

    private void CheckDropsandBoosts()
    {
        if (_drops.ToPickup.Any())
            _drops.Start();

        if (_boosts.UsingBoosts)
            _boosts.Start();
    }
}
