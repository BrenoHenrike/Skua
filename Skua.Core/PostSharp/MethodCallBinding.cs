using System.Reflection;
using Newtonsoft.Json;
using PostSharp.Aspects;
using Skua.Core.Utils;

namespace Skua.Core.PostSharp;

[Serializable]
public class MethodCallBindingAttribute : MethodInterceptionAspect
{
    public string Name { get; set; }
    public bool RunMethodPre { get; set; } = false;
    public bool RunMethodPost { get; set; } = false;
    public bool GameFunction { get; set; } = false;
    public bool ForceJSON { get; set; } = false;
    public object? DefaultValue { get; set; } = null;
    public Type? DefaultProvider { get; set; } = null;

    private ITypedValueProvider _defaultProvider = new DefaultTypedValueProvider();
    private bool _defaultProviderSet;

    public MethodCallBindingAttribute(string name)
    {
        Name = name;
    }

    public override void OnInvoke(MethodInterceptionArgs args)
    {
        if (RunMethodPre)
            args.Proceed();

        if (DefaultProvider is not null && !_defaultProviderSet)
        {
            _defaultProvider = (ITypedValueProvider)Activator.CreateInstance(DefaultProvider)!;
            _defaultProviderSet = true;
        }

        Type retType = (args.Method as MethodInfo)!.ReturnType;

        try
        {
            if (GameFunction)
            {
                string ret = Test.FlashUtil.CallGameFunction(Name, args.Arguments.ToArray())!;
                args.ReturnValue = retType == typeof(void) ? null : JsonConvert.DeserializeObject(ret, retType);
            }
            else
                args.ReturnValue = !ForceJSON
                    ? Test.FlashUtil.Call(Name, retType, args.Arguments.ToArray())
                    : JsonConvert.DeserializeObject(Test.FlashUtil.Call(Name, args.Arguments.ToArray())!, retType);
        }
        catch
        {
            args.ReturnValue = DefaultValue ?? _defaultProvider.Provide(retType);
        }

        if (RunMethodPost && !RunMethodPre)
            args.Proceed();
    }
}
