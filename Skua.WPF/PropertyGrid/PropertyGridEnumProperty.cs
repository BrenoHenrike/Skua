using System.Reflection;

namespace Skua.WPF;
public class PropertyGridEnumProperty : PropertyGridProperty
{
    public PropertyGridEnumProperty(PropertyGridDataProvider provider)
        : base(provider)
    {
        EnumAttributes = provider.CreateDynamicObject();
    }

    public override void OnValueChanged()
    {
        base.OnValueChanged();
        EnumAttributes.Properties.Clear();
        foreach (FieldInfo fi in PropertyType.GetFields(BindingFlags.Static | BindingFlags.Public))
        {
            if (fi.Name.Equals(string.Format("{0}", base.Value)))
            {
                PropertyGridDataProvider.AddDynamicProperties(fi.GetAttributes<PropertyGridAttribute>(), EnumAttributes);
            }
        }
    }

    public virtual DynamicObject EnumAttributes { get; private set; }
}
