using Skua.Core.Interfaces;

namespace Skua.Core.Skills;
public class AdvancedSkillCommand
{
    public List<int> Skills { get; set; } = new List<int>();
    public List<UseRule[]> UseRules { get; set; } = new();
    private int _Index = 0;

    public int GetNextSkill()
    {
        int skill = Skills[_Index];
        _Index++;
        if (_Index >= Skills.Count)
            _Index = 0;

        return skill;
    }

    public bool? ShouldUse(IScriptPlayer player)
    {
        if (UseRules.Count == 0 || UseRules[_Index].First().None)
            return true;
        bool shouldUse = true;
        foreach (UseRule useRule in UseRules[_Index])
        {
            if (!player.Alive)
                return false;

            switch (useRule.Rule)
            {
                case true:
                    shouldUse = HealthUseRule(player, useRule.Greater, useRule.Value);
                    break;
                case false:
                    shouldUse = ManaUseRule(player, useRule.Greater, useRule.Value);
                    break;
                case null:
                    Thread.Sleep(useRule.Value);
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

public struct UseRule
{
    public UseRule(bool none)
    {
        None = none;
    }
    public UseRule(bool? rule, bool greater, int value, bool shouldSkip)
    {
        Rule = rule;
        Greater = greater;
        Value = value;
        ShouldSkip = shouldSkip;
    }
    public readonly bool None = false;
    /// <summary>
    /// <list type="bullet">
    /// <item><see langword="null"/> = Wait</item>
    /// <item><see langword="true"/> = Health</item>
    /// <item><see langword="false"/> = Mana</item>
    /// </list>
    /// </summary>
    public readonly bool? Rule = null;
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
