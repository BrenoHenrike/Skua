using Skua.Core.Flash;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
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

    private ISkillProvider? _provider;
    private Thread? _skillThread;
    private CancellationTokenSource? _skillsCTS;

    [MethodCallBinding("canUseSkill")]
    private bool _canUseSkill(int index) => false;

    [MethodCallBinding("useSkill")]
    private bool _useSkill(int index) => false;

    public ISkillProvider? OverrideProvider { get; set; } = null;
    public ISkillProvider BaseProvider { get; private set; }
    public bool TimerRunning { get; private set; } = false;
    public int SkillInterval { get; set; } = 100;
    public int SkillTimeout { get; set; } = -1;
    public SkillUseMode SkillUseMode { get; set; } = SkillUseMode.UseIfAvailable;

    public void Start()
    {
        if (BaseProvider is null)
        {
            BaseProvider = new AdvancedSkillProvider(Player, Combat);
            BaseProvider.Load(genericSkills);
            _provider = BaseProvider;
        }
        _provider = OverrideProvider ?? BaseProvider;

        if (TimerRunning)
            return;

        _skillThread = new(async () =>
        {
            _skillsCTS = new();
            try
            {
                await _Timer(_skillsCTS.Token);
            }
            catch { }
            finally
            {
                _skillsCTS?.Dispose();
                _skillsCTS = null;
                TimerRunning = false;
            }
        });

        _skillThread.Name = "Skill Timer";
        _skillThread.Start();
        TimerRunning = true;
    }

    public void Stop()
    {
        _provider?.Stop();
        _skillsCTS?.Cancel();
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

        if (className == "generic")
        {
            OverrideProvider.Load(genericSkills);
            SkillUseMode = SkillUseMode.UseIfAvailable;
            return;
        }

        if (autoEquip)
        {
            Inventory.EquipItem(className);
            Wait.ForItemEquip(className);
        }

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

    private int _lastRank = -1;
    private SkillInfo[] _playerSkills = null!;

    private async Task _Timer(CancellationToken token)
    {
        // get current player skills
        SkillInfo[]? playerSkills = Player.Skills;
        // if the player has skills assign to _playerSkills to be used later
        if (playerSkills is not null && playerSkills.Length > 0)
            _playerSkills = playerSkills;

        while (!token.IsCancellationRequested)
        {
            if (Combat.StopAttacking)
            {
                _counterAttack.WaitOne(10000);
                Combat.StopAttacking = false;
            }

            // if the player has target or bot attack without target option is on
            // then activate the skills
            if (Options.AttackWithoutTarget || Player.HasTarget)
            {
                _Poll(token);
            }

            // target reset if player has no target
            _provider?.OnTargetReset();

            // wait for skill interval
            if (!token.IsCancellationRequested)
                await Task.Delay(SkillInterval, token);
        }
    }

    private void _Poll(CancellationToken token)
    {
        // if the current player has skills and the current class rank is different from the last rank
        // then update the skills since classes will enable a certain skill base on the rank
        if (_playerSkills is not null && Player.CurrentClassRank > _lastRank)
        {
            // Update the player skills skills
            SkillInfo[]? playerSkills = Player.Skills;
            if (playerSkills is not null && playerSkills.Length > 0)
                _playerSkills = playerSkills;

            int k = 0;
            using FlashArray<object> skills = (FlashArray<object>)Flash.CreateFlashObject<object>("world.actions.active").ToArray();
            foreach (FlashObject<object> skill in skills)
            {
                using FlashObject<long> ts = (FlashObject<long>)skill.GetChild<long>("ts");
                ts.Value = _playerSkills[k++]?.LastUse ?? 0;
            }
        }

        // Store the last rank if the player is ranking up
        _lastRank = Player.CurrentClassRank;
        if (token.IsCancellationRequested)
            return;

        var (index, skillS) = _provider!.GetNextSkill();

        switch (_provider?.ShouldUseSkill(index, CanUseSkill(skillS)))
        {
            case true:
                if (skillS != -1 && !_playerSkills![skillS].IsOk)
                    break;
                UseSkill(skillS);
                break;

            case null:
                var (indexS, skill) = _provider!.GetNextSkill();
                if (skill != -1 && !_playerSkills![skill].IsOk)
                    break;
                UseSkill(skill);
                break;

            default:
                break;
        }

        // This method will activate a skill
        void UseSkill(int skill)
        {
            switch (SkillUseMode)
            {
                // if SkillUseMode is UseIfAvailable then use skills without waiting for cooldown
                case SkillUseMode.UseIfAvailable:
                    if ((Options.AttackWithoutTarget || CanUseSkill(skill)) && !Combat.StopAttacking)
                        this.UseSkill(skill);
                    break;

                // if SkillUseMode is UseIfAvailable then use skills with waiting for cooldown
                case SkillUseMode.WaitForCooldown:
                    if (Options.AttackWithoutTarget || (skill != -1 && Wait.ForTrue(() => CanUseSkill(skill), null, SkillTimeout, SkillInterval) && !Combat.StopAttacking))
                        this.UseSkill(skill);
                    break;
            }
        }
    }

    private readonly AutoResetEvent _counterAttack = new(false);

    private void CounterAttack(ScriptSkill recipient, CounterAttackMessage message)
    {
        if (message.Faded)
            recipient._counterAttack.Set();
    }
}