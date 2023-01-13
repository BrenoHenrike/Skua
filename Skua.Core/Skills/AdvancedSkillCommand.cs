using Skua.Core.Interfaces;
using System.Diagnostics;

namespace Skua.Core.Skills;
public class AdvancedSkillCommand
{
    public Dictionary<int, int> Skills { get; set; } = new();
    public List<UseRule[]> UseRules { get; set; } = new();
    private int _Index = 0;

    public (int, int) GetNextSkill()
    {
        int skill = Skills[_Index];
        int index = _Index;
        ++_Index;
        if (_Index >= Skills.Count)
            _Index = 0;
        return (index, skill);
    }

    
    public bool? ShouldUse(IScriptPlayer player, int skillIndex, bool canUse)
    {
        if (UseRules.Count == 0 || UseRules[skillIndex].First().Rule == SkillRule.None)
            return true; 
        
        bool shouldUse = true;
        foreach (UseRule useRule in UseRules[skillIndex])
        {
            if (!player.Alive)
                return false;

            switch (useRule.Rule)
            {
                case SkillRule.Health:
                    shouldUse = HealthUseRule(player, useRule.Greater, useRule.Value);
                    break;
                case SkillRule.Mana:
                    shouldUse = ManaUseRule(player, useRule.Greater, useRule.Value);
                    break;
                case SkillRule.Wait:
                    if (useRule.ShouldSkip && !canUse)
                        return null; 
                    Task.Delay(useRule.Value).Wait();
                    break;
            }

            if (useRule.ShouldSkip && !shouldUse)
                return null;

            if (!shouldUse)
                break;
        }
        return shouldUse;
    }

    private bool HealthUseRule(IScriptPlayer player, bool greater, int health)
    {
        if (player.Health == 0 || player.MaxHealth == 0)
            return false;
        int ratio = (int)(player.Health / (double)player.MaxHealth * 100.0);
        return greater ? ratio >= health : ratio <= health;
    }

    private bool ManaUseRule(IScriptPlayer player, bool greater, int mana)
    {
        return greater ? player.Mana >= mana : player.Mana <= mana;
    }

    public void Reset()
    {
        _Index = 0;
    }
}

public enum SkillRule
{
    None,
    Health,
    Mana,
    Wait
}

public struct UseRule
{
    public UseRule(SkillRule rule)
    {
        Rule = rule;
    }
    
    public UseRule(SkillRule rule, bool greater, int value, bool shouldSkip)
    {
        Rule = rule;
        Greater = greater;
        Value = value;
        ShouldSkip = shouldSkip;
    }
    
    /// <summary>
    /// <list type="bullet">
    /// <item><see langword="null"/> = Wait</item>
    /// <item><see langword="true"/> = Health</item>
    /// <item><see langword="false"/> = Mana</item>
    /// </list>
    /// </summary>
    public readonly SkillRule Rule = SkillRule.None;
    
    /// <summary>
    /// <list type="bullet">
    /// <item><see langword="true"/> = Great than</item>
    /// <item><see langword="false"/> = Less than</item>
    /// </list>
    /// </summary>
    public readonly bool Greater = default;
    public readonly int Value = default;
    public readonly bool ShouldSkip = default;
}
