using PostSharp.Aspects;

namespace Skua.Core.PostSharp;

[Serializable]
public class ModuleBindingAttribute : LocationInterceptionAspect
{
    public string ModuleName { get; set; }

    public ModuleBindingAttribute(string name)
    {
        ModuleName = name;
    }

    public override void OnSetValue(LocationInterceptionArgs args)
    {
        base.OnSetValue(args);
        Test.FlashUtil.Call($"mod{((bool)args.Value ? "Enable" : "Disable")}", ModuleName);
    }
}
