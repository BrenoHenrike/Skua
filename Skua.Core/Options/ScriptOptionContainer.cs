using Skua.Core.Interfaces;
using Skua.Core.Models;

namespace Skua.Core.Options;

public class ScriptOptionContainer : OptionContainer, IScriptOptionContainer
{
    public ScriptOptionContainer(IDialogService dialogService)
        : base(dialogService) { }

    public string Storage { get; set; } = "default";

    public override string OptionsFile => Path.Combine(ClientFileSources.SkuaOptionsDIR, $"{Storage}.cfg");
}