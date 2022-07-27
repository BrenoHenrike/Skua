using Skua.Core.Flash;
using Skua.Core.Interfaces;
using Skua.Core.Models.Skills;
using Skua.Core.Skills;

namespace Skua.Core.Scripts;

public partial class ScriptSkill : IScriptSkill
{
    private const string genericSkills = "1 | 2 | 3 | 4";
    private readonly Lazy<IFlashUtil> _lazyFlash;
    private readonly Lazy<IScriptPlayer> _lazyPlayer;
    private readonly Lazy<IScriptCombat> _lazyCombat;
    private readonly Lazy<IScriptWait> _lazyWait;
    private readonly Lazy<IScriptOption> _lazyOptions;
    private readonly Lazy<IScriptInventory> _lazyInventory;

    private IFlashUtil Flash => _lazyFlash.Value;
    private IScriptPlayer Player => _lazyPlayer.Value;
    private IScriptCombat Combat => _lazyCombat.Value;
    private IScriptWait Wait => _lazyWait.Value;
    private IScriptOption Options => _lazyOptions.Value;
    private IScriptInventory Inventory => _lazyInventory.Value;

    public IAdvancedSkillContainer AdvancedSkillContainer { get; set; }

    public ScriptSkill(
        IAdvancedSkillContainer advContainer,
        Lazy<IFlashUtil> flash,
        Lazy<IScriptPlayer> player,
        Lazy<IScriptCombat> combat,
        Lazy<IScriptOption> options,
        Lazy<IScriptInventory> inventory,
        Lazy<IScriptWait> wait)
    {
        _lazyFlash = flash;
        _lazyPlayer = player;
        _lazyCombat = combat;
        _lazyWait = wait;
        _lazyOptions = options;
        _lazyInventory = inventory;
        AdvancedSkillContainer = advContainer;
    }

    private ISkillProvider _provider;
    private Thread? SkillThread;

    private CancellationTokenSource? SkillsCTS;

    [MethodCallBinding("canUseSkill")]
    private bool _canUseSkill(int index) => false;

    [MethodCallBinding("useSkill")]
    private void _useSkill(int index) { }

    public ISkillProvider? OverrideProvider { get; set; } = null;
    public ISkillProvider BaseProvider { get; private set; }
    public bool TimerRunning => SkillThread?.IsAlive ?? false;
    public int SkillInterval { get; set; } = 100;
    public int SkillTimeout { get; set; } = -1;
    public SkillUseMode SkillUseMode { get; set; } = SkillUseMode.UseIfAvailable;

    public void Start()
    {
        if(BaseProvider is null)
        {
            BaseProvider = new AdvancedSkillProvider(Player, Combat);
            BaseProvider.Load(genericSkills);
            _provider = BaseProvider;
        }
        _provider = OverrideProvider ?? BaseProvider;
        if (SkillThread?.IsAlive ?? false)
            return;
        SkillThread = new(async () =>
        {
            SkillsCTS = new();
            try
            {
                await _Timer(SkillsCTS.Token);
            }
            catch { }
            SkillsCTS.Dispose();
            SkillsCTS = null;
        });
        SkillThread.Name = "Skill Timer";
        SkillThread.Start();
    }

    public void Stop()
    {
        _provider?.Stop();
        SkillsCTS?.Cancel();
        Wait.ForTrue(() => !TimerRunning, 20);
    }

    public void SetProvider(ISkillProvider provider)
    {
        _provider = OverrideProvider = provider;
    }

    public void UseBaseProvider()
    {
        _provider = BaseProvider;
    }

    public void LoadAdvanced(string className, bool autoEquip, ClassUseMode useMode = ClassUseMode.Base)
    {
        OverrideProvider = new AdvancedSkillProvider(Player, Combat);

        if(className == "generic")
        {
            OverrideProvider.Load(genericSkills);
            SkillUseMode = SkillUseMode.UseIfAvailable;
            return;
        }

        if (autoEquip)
            Inventory.EquipItem(className);

        List<AdvancedSkill> skills = AdvancedSkillContainer.LoadedSkills.Where(s => s.ClassName.ToLower() == className.ToLower()).ToList();
        if (skills is null || skills.Count == 0)
        {
            OverrideProvider.Load(genericSkills);
            SkillUseMode = SkillUseMode.UseIfAvailable;
            return;
        }
        AdvancedSkill skill = skills.Find(s => s.ClassUseMode == useMode) ?? skills.FirstOrDefault()!;
        OverrideProvider.Load(skill.Skills);
        SkillTimeout = skill.SkillTimeout;
        SkillUseMode = skill.SkillUseMode;
    }

    public void LoadAdvanced(string skills, int skillTimeout = -1, SkillUseMode skillMode = SkillUseMode.UseIfAvailable)
    {
        OverrideProvider = new AdvancedSkillProvider(Player, Combat);
        SkillTimeout = skillTimeout;
        SkillUseMode = skillMode;
        OverrideProvider.Load(skills);
    }

    private async Task _Timer(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (Options.AttackWithoutTarget || Player.HasTarget)
                _Poll(token);
            _provider?.OnTargetReset();
            if (!token.IsCancellationRequested)
                await Task.Delay(SkillInterval, token);
        }
    }

    private int _lastRank = -1;
    private SkillInfo[] _lastSkills = null!;
    private void _Poll(CancellationToken token)
    {
        int rank = Player.CurrentClassRank;
        if (_lastSkills is not null && rank > _lastRank)
        {
            using FlashArray<object> skills = (FlashArray<object>)Flash.CreateFlashObject<object>("world.actions.active").ToArray();
            int k = 0;
            foreach (FlashObject<object> skill in skills)
            {
                using FlashObject<long> ts = (FlashObject<long>)skill.GetChild<long>("ts");
                ts.Value = _lastSkills[k++]?.LastUse ?? 0;
            }
        }
        _lastRank = rank;
        if (Player.Skills is not null)
            _lastSkills = Player.Skills;
        if (token.IsCancellationRequested)
            return;
        switch (_provider?.ShouldUseSkill())
        {
            case true:
                int skill = _provider.GetNextSkill();
                if (skill != -1 && !_lastSkills![skill].IsOk)
                    break;
                UseSkill(skill);
                break;
            case null:
                _provider?.GetNextSkill();
                break;
            default:
                break;
        }

        void UseSkill(int skill)
        {
            switch (SkillUseMode)
            {
                case SkillUseMode.UseIfAvailable:
                    if (Options.AttackWithoutTarget || CanUseSkill(skill))
                        this.UseSkill(skill);
                    break;
                case SkillUseMode.WaitForCooldown:
                    if (Options.AttackWithoutTarget || (skill != -1 && Wait.ForTrue(() => CanUseSkill(skill), null, SkillTimeout, SkillInterval)))
                        this.UseSkill(skill);
                    break;
            }
        }
    }
}
