using System.Text;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Skua.Core.Utils;

namespace Skua.Core.ViewModels;
public class SkillItemViewModel : ObservableObject
{
    public SkillItemViewModel(int skill, bool useRule, int waitValue, bool healthGreaterThanBool, int healthValue, bool manaGreaterThanBool, int manaValue, bool skipBool)
    {
        Skill = skill;
        _useRules = new SkillRulesViewModel()
        {
            UseRuleBool = useRule,
            WaitUseValue = waitValue,
            HealthGreaterThanBool = healthGreaterThanBool,
            HealthUseValue = healthValue,
            ManaGreaterThanBool = manaGreaterThanBool,
            ManaUseValue = manaValue,
            SkipUseBool = skipBool
        };
        _displayString = ToString();
    }
    public SkillItemViewModel(int skill, SkillRulesViewModel useRules)
    {
        Skill = skill;
        _useRules = new SkillRulesViewModel()
        {
            UseRuleBool = useRules.UseRuleBool,
            WaitUseValue = useRules.WaitUseValue,
            HealthGreaterThanBool = useRules.HealthGreaterThanBool,
            HealthUseValue = useRules.HealthUseValue,
            ManaGreaterThanBool = useRules.ManaGreaterThanBool,
            ManaUseValue = useRules.ManaUseValue,
            SkipUseBool = useRules.SkipUseBool
        };
        _displayString = ToString();
    }

    public SkillItemViewModel(string skill)
    {
        string[] skillRules = skill[1..].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        Skill = int.Parse(skill.AsSpan(0, 1));
        bool useRule = false, healthGreater = false, manaGreater = false, skip = false;
        int waitVal = 0, healthVal = 0, manaVal = 0;
        for (int i = 0; i < skillRules.Length; i++)
        {
            if (skillRules[i].Contains('W'))
            {
                useRule = true;
                waitVal = int.Parse(skillRules[i].RemoveLetters());
            }
            else if (skillRules[i].Contains('H'))
            {
                useRule = true;
                if (skillRules[i].Contains('>'))
                    healthGreater = true;
                healthVal = int.Parse(skillRules[i].RemoveLetters());
            }
            else if (skillRules[i].Contains('M'))
            {
                useRule = true;
                if (skillRules[i].Contains('>'))
                    manaGreater = true;
                manaVal = int.Parse(skillRules[i].RemoveLetters());
            }

            if (skillRules[i].Contains('s'))
                useRule = skip = true;
        }
        _useRules = new SkillRulesViewModel()
        {
            UseRuleBool = useRule,
            WaitUseValue = waitVal,
            HealthGreaterThanBool = healthGreater,
            HealthUseValue = healthVal,
            ManaGreaterThanBool = manaGreater,
            ManaUseValue = manaVal,
            SkipUseBool = skip
        };
        _displayString = ToString();
    }

    private SkillRulesViewModel _useRules;
    public SkillRulesViewModel UseRules
    {
        get { return _useRules; }
        set
        {
            _useRules = value;
            DisplayString = ToString();
        }
    }
    public int Skill { get; }

    private string _displayString;
    public string DisplayString
    {
        get { return _displayString; }
        set { SetProperty(ref _displayString, value); }
    }

    public override string ToString()
    {
        StringBuilder bob = new();
        bob.Append(Skill);
        if(!UseRules.UseRuleBool)
            return bob.ToString();
        if(UseRules.WaitUseValue != 0)
            bob.Append($" - Wait for {UseRules.WaitUseValue}");
        if(UseRules.HealthUseValue != 0)
        {
            bob.Append(" - Health");
            _ = UseRules.HealthGreaterThanBool ? bob.Append(" greater than ") : bob.Append(" less than ");
            bob.Append(UseRules.HealthUseValue);
        }
        if (UseRules.ManaUseValue != 0)
        {
            bob.Append(" - Mana");
            _ = UseRules.ManaGreaterThanBool ? bob.Append(" greater than ") : bob.Append(" less than ");
            bob.Append(UseRules.ManaUseValue);
        }
        if(UseRules.SkipUseBool)
            bob.Append(" [Skip if not available]");
        return bob.ToString();
    }

    public string Convert()
    {
        StringBuilder bob = new();
        bob.Append(Skill);
        if (!UseRules.UseRuleBool)
            return bob.ToString();
        if (UseRules.WaitUseValue != 0)
            bob.Append($" WW{UseRules.WaitUseValue}");
        if (UseRules.HealthUseValue != 0)
            bob.Append($" H{(UseRules.HealthGreaterThanBool ? ">" : "<")}{UseRules.HealthUseValue}");
        if (UseRules.ManaUseValue != 0)
            bob.Append($" M{(UseRules.ManaGreaterThanBool ? ">" : "<")}{UseRules.ManaUseValue}");
        if (UseRules.SkipUseBool)
            bob.Append('S');
        return bob.ToString();
    }
}
