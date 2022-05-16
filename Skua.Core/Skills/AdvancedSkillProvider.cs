using Skua.Core.Interfaces;
using Skua.Core.Models.Skills;
using Skua.Core.Utils;

namespace Skua.Core.Skills;

public class AdvancedSkillProvider : ISkillProvider
{
    public AdvancedSkillProvider(IScriptInterface bot)
    {
        Bot = bot;
    }
    public AdvancedSkillCommand Root { get; set; } = new AdvancedSkillCommand();
    public bool ResetOnTarget { get; set; } = false;
    public IScriptInterface Bot { get; }

    public int GetNextSkill()
    {
        return Root.GetNextSkill(Bot);
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
        string[] stringRules = useRule.Split(' ').Select(s => s.Trim()).ToArray();
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
        // TODO Save as json
    }

    public void OnTargetReset()
    {
        if (ResetOnTarget && !Bot.Player.HasTarget)
            Root.Reset();
    }
    public bool? ShouldUseSkill()
    {
        return Root.ShouldUse(Bot);
    }

    public void Stop()
    {
        Bot.Combat.CancelAutoAttack();
        Bot.Combat.CancelTarget();
        Root.Reset();
    }
}
