using System.ComponentModel;

namespace Skua.WPF;

public class PropertyGridEventArgs : CancelEventArgs
{
    public PropertyGridEventArgs(PropertyGridProperty property)
        : this(property, null)
    {
    }

    public PropertyGridEventArgs(PropertyGridProperty property, object context)
    {
        Property = property;
        Context = context;
    }

    public PropertyGridProperty Property { get; private set; }
    public object Context { get; set; }
}