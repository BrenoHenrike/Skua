using Newtonsoft.Json;
using PostSharp.Aspects;
using Skua.Core.Utils;

namespace Skua.Core.PostSharp;

[Serializable]
public class CallBindingAttribute : LocationInterceptionAspect
{
    public string Name { get; set; }
    public bool UseValue { get; set; } = true;
    public bool Get { get; set; } = true;
    public bool Set { get; set; } = true;
    public bool Json { get; set; } = false;
    public Type ConvertType { get; set; } = null!;
    public Type DefaultProvider { get; set; } = null!;

    private ITypedValueProvider _defaultProvider = new DefaultTypedValueProvider();
    private bool _defaultProviderSet;

    public CallBindingAttribute(string name)
    {
        Name = name;
    }

    public override void OnGetValue(LocationInterceptionArgs args)
    {
        base.OnGetValue(args);

        if (DefaultProvider != null && !_defaultProviderSet)
        {
            _defaultProvider = (ITypedValueProvider)Activator.CreateInstance(DefaultProvider)!;
            _defaultProviderSet = true;
        }
        if (Get)
        {
            if (ConvertType == null)
                ConvertType = args.Location.LocationType;
            try
            {
                args.Value = Json ? JsonConvert.DeserializeObject(Test.FlashUtil.Call(Name)!, ConvertType) : Test.FlashUtil.Call(Name, ConvertType);
            }
            catch
            {
                args.Value = _defaultProvider.Provide(ConvertType);
            }
        }
    }

    public override void OnSetValue(LocationInterceptionArgs args)
    {
        base.OnSetValue(args);
        if (Set)
        {
            try
            {
                if (UseValue)
                    Test.FlashUtil.Call(Name, args.Value);
                else
                    Test.FlashUtil.Call(Name);
            }
            catch
            {

            }
        }
    }
}
