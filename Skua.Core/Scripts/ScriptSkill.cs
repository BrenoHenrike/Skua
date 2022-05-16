using Skua.Core.Flash;
using Skua.Core.Interfaces;
using Skua.Core.Interfaces.Skills;
using Skua.Core.Models.Skills;
using Skua.Core.PostSharp;
using Skua.Core.Skills;

namespace Skua.Core.Scripts;

public class ScriptSkill : ScriptableObject, IScriptSkill
{
    public IAdvancedSkillContainer AdvancedSkillContainer { get; set; }
    private const string genericSkills = "1 | 2 | 3 | 4";
    public ScriptSkill(IAdvancedSkillContainer advContainer)
    {
        AdvancedSkillContainer = advContainer;
        BaseProvider = new AdvancedSkillProvider(Bot);
        BaseProvider.Load(genericSkills);
        _provider = BaseProvider;
    }

    private ISkillProvider _provider;
    private Thread? SkillThread;

    private CancellationTokenSource? SkillsCTS;

    [MethodCallBinding("canUseSkill")]
    public bool CanUseSkill(int index) => false;

    [MethodCallBinding("useSkill")]
    public void UseSkill(int index) { }

    public ISkillProvider? OverrideProvider { get; set; } = null;
    public ISkillProvider BaseProvider { get; }
    public bool TimerRunning => SkillThread?.IsAlive ?? false;
    public int SkillInterval { get; set; } = 100;
    public int SkillTimeout { get; set; } = -1;
    public SkillMode SkillUseMode { get; set; } = SkillMode.UseIfAvailable;

    public void Start()
    {
        _provider = OverrideProvider ?? BaseProvider;
        if (SkillThread?.IsAlive ?? false)
            return;
        SkillThread = new(() =>
        {
            SkillsCTS = new();
            _Timer(SkillsCTS.Token);
            SkillsCTS.Dispose();
            SkillsCTS = null;
        })
        {
            Name = "Skill Timer"
        };
        SkillThread.Start();
    }

    public void Stop()
    {
        _provider?.Stop();
        SkillsCTS?.Cancel();
        Bot.Wait.ForTrue(() => !TimerRunning, 20);
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
        OverrideProvider = new AdvancedSkillProvider(Bot);
        if (autoEquip)
            Bot.Inventory.EquipItem(className);
        // TODO Implement Advanced Skills
        //List<AdvancedSkill> skills = AdvancedSkillContainer.LoadedSkills?.Where(s => s.ClassName.ToLower() == className.ToLower()).ToList();
        //if (skills is null || skills.Count == 0)
        //{
        //    OverrideProvider.Load(genericSkills);
        //    SkillTimeout = -1;
        //    SkillUseMode = SkillMode.UseIfAvailable;
        //    return;
        //}

        //AdvancedSkill skill = skills.Find(s => s.ClassUseMode == useMode) ?? skills.FirstOrDefault()!;
        //OverrideProvider.Load(skill.Skills);
        //SkillTimeout = skill.SkillTimeout;
        //SkillUseMode = skill.SkillUseMode;
    }

    public void LoadAdvanced(string skills, int skillTimeout = -1, SkillMode skillMode = SkillMode.UseIfAvailable)
    {
        OverrideProvider = new AdvancedSkillProvider(Bot);
        SkillTimeout = skillTimeout;
        SkillUseMode = skillMode;
        OverrideProvider.Load(skills);
    }

    private void _Timer(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (Bot.Options.AttackWithoutTarget || Bot.Player.HasTarget)
                _Poll(token);
            _provider?.OnTargetReset();
            if (!token.IsCancellationRequested)
                Thread.Sleep(SkillInterval);
        }
    }

    private int _lastRank = -1;
    private SkillInfo[] _lastSkills = null!;
    private void _Poll(CancellationToken token)
    {
        int rank = Bot.Player.CurrentClassRank;
        if (_lastSkills is not null && rank > _lastRank)
        {
            using FlashArray<object> skills = (FlashArray<object>)Bot.Flash.CreateFlashObject<object>("world.actions.active").ToArray();
            int k = 0;
            foreach (FlashObject<object> skill in skills)
            {
                using FlashObject<long> ts = (FlashObject<long>)skill.GetChild<long>("ts");
                ts.Value = _lastSkills[k++]?.LastUse ?? 0;
            }
        }
        _lastRank = rank;
        if (Bot.Player.Skills is not null)
            _lastSkills = Bot.Player.Skills;
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
                case SkillMode.UseIfAvailable:
                    if (Bot.Options.AttackWithoutTarget || CanUseSkill(skill))
                        UseSkill(skill);
                    break;
                case SkillMode.WaitForCooldown:
                    if (Bot.Options.AttackWithoutTarget || (skill != -1 && Bot.Wait.ForTrue(() => CanUseSkill(skill), null, SkillTimeout, SkillInterval)))
                        UseSkill(skill);
                    break;
            }
        }
    }
}
