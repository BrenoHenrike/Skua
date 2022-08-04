using Skua.Core.Interfaces;
using Skua.Core.Utils;

namespace Skua.Core.Skills;

public class AdvancedSkillProvider : ISkillProvider
{
    private readonly IScriptPlayer Player;
    private readonly IScriptCombat Combat;
    public AdvancedSkillProvider(IScriptPlayer player, IScriptCombat combat)
    {
        Player = player;
        Combat = combat;
    }
    public AdvancedSkillCommand Root { get; set; } = new AdvancedSkillCommand();
    public bool ResetOnTarget { get; set; } = false;

    public int GetNextSkill()
    {
        return Root.GetNextSkill();
    }

    public void Load(string skills)
    {
        foreach (string command in skills.ToLower().Split('|').Select(s => s.Trim()).ToList())
        {
            if(int.TryParse(command.AsSpan(0, 1), out int skill))
            {
                Root.Skills.Add(skill);
                Root.UseRules.Add(command.Length <= 1 ? none : ParseUseRule(command[1..]));
            }
        }
    }

    private readonly UseRule[] none = new[] { new UseRule(true) };

    private UseRule[] ParseUseRule(string useRule)
    {
        ReadOnlySpan<string> stringRules = useRule.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
        UseRule[] rules = new UseRule[stringRules.Length];
        bool shouldSkip = useRule.Last() == 's';
        for(int i = 0; i < stringRules.Length; i++)
        {
            if(stringRules[i].Contains('h'))
            {
                rules[i] = new UseRule(true, stringRules[i].Contains('>'), int.Parse(stringRules[i].RemoveLetters()), shouldSkip);
                continue;
            }
            if (stringRules[i].Contains('m'))
            {
                rules[i] = new UseRule(false, stringRules[i].Contains('>'), int.Parse(stringRules[i].RemoveLetters()), shouldSkip);
                continue;
            }
            if (stringRules[i].Contains('w'))
            {
                rules[i] = new UseRule(null, true, int.Parse(stringRules[i].RemoveLetters()), shouldSkip);
                continue;
            }
        }
        return rules;
    }

    public void Save(string file)
    {
    }

    public void OnTargetReset()
    {
        if (ResetOnTarget && !Player.HasTarget)
            Root.Reset();
    }
    public bool? ShouldUseSkill()
    {
        return Root.ShouldUse(Player);
    }

    public void Stop()
    {
        Combat.CancelAutoAttack();
        Combat.CancelTarget();
        Root.Reset();
    }
}
