namespace Skua.WPF;
public class PropertyGridItem : AutoObject
{
    public PropertyGridItem()
    {
        IsChecked = false;
    }

    public virtual bool IsUnset { get { return GetProperty<bool>(); } set { SetProperty(value); } }
    public virtual bool IsZero { get { return GetProperty<bool>(); } set { SetProperty(value); } }
    public virtual string Name { get { return GetProperty<string>(); } set { SetProperty(value); } }
    public virtual object Value { get { return GetProperty<object>(); } set { SetProperty(value); } }
    public virtual bool? IsChecked { get { return GetProperty<bool?>(); } set { SetProperty(value); } }
    public virtual PropertyGridProperty Property { get { return GetProperty<PropertyGridProperty>(); } set { SetProperty(value); } }

    public override string ToString()
    {
        return Name;
    }
}
