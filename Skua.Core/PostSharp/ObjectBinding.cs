using Newtonsoft.Json;
using PostSharp.Aspects;
using Skua.Core.Utils;

namespace Skua.Core.PostSharp;

[Serializable]
public class ObjectBindingAttribute : LocationInterceptionAspect
{
    public string[] Names { get; set; }
    public bool Get { get; set; } = true;
    public bool Set { get; set; } = true;
    public int GetIndex { get; set; } = 0;
    public Type ConvertType { get; set; } = null!;
    public object DefaultValue { get; set; } = null!;
    public string Select { get; set; } = null!;
    public string RequireNotNull { get; set; } = null!;
    public bool Static { get; set; } = false;
    public Type? DefaultProvider { get; set; }

    private ITypedValueProvider _defaultProvider = new DefaultTypedValueProvider();
    private bool _defaultProviderSet;

    public ObjectBindingAttribute(params string[] names)
    {
        Names = names;
    }

    public override void OnGetValue(LocationInterceptionArgs args)
    {
        if (DefaultProvider is not null && !_defaultProviderSet)
        {
            _defaultProvider = (ITypedValueProvider)Activator.CreateInstance(DefaultProvider)!;
            _defaultProviderSet = true;
        }

        if (Get)
        {
            if (RequireNotNull is not null && Test.FlashUtil.IsNull(RequireNotNull))
                args.Value = DefaultValue ?? _defaultProvider.Provide(ConvertType);
            else
            {
                try
                {
                    if (ConvertType is null)
                        ConvertType = args.Location.LocationType;
                    if (Select is not null)
                        args.Value = JsonConvert.DeserializeObject(Test.FlashUtil.Call("selectArrayObjects", Names[GetIndex], Select)!, ConvertType);
                    else
                        args.Value = JsonConvert.DeserializeObject(Test.FlashUtil.Call("getGameObject" + (Static ? "S" : ""), Names[GetIndex])!, ConvertType);
                }
                catch
                {
                    args.Value = DefaultValue ?? _defaultProvider.Provide(ConvertType);
                }
            }
        }
        else
            base.OnGetValue(args);
    }

    public override void OnSetValue(LocationInterceptionArgs args)
    {
        base.OnSetValue(args);
        if (Set)
        {
            foreach (string name in Names)
            {
                if (name.Contains('['))
                {
                    string key = name.Split(new char[] { '"', '[', ']' }, StringSplitOptions.RemoveEmptyEntries).Last();
                    string finalPath = name.Split('[')[0];
                    Test.FlashUtil.Call("setGameObjectKey", finalPath, key, args.Value);
                    continue;
                }
                Test.FlashUtil.Call("setGameObject", name, args.Value);
            }
        }
    }
}
