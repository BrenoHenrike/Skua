using Skua.Core.Interfaces;

namespace Skua.Core.Options;
public class ScriptOptionContainer : OptionContainer, IScriptOptionContainer
{
    public ScriptOptionContainer(IDialogService dialogService)
        : base(dialogService) { }

    public string Storage { get; set; } = "default";

    public override string OptionsFile => Path.Combine("options", $"{Storage}.cfg");
}
