namespace Skua.Core.ViewModels;
public class SkillRuleEditorDialogViewModel : DialogViewModelBase
{
    public SkillRuleEditorDialogViewModel(SkillRulesViewModel useRules)
        : base("Edit Rules")
    {
        UseRules = useRules;
    }

    public SkillRulesViewModel UseRules { get; }
}
