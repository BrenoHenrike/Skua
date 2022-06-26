using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Skua.WPF;
/// <summary>
/// Represents a dynamic object property.
/// </summary>
public class DynamicObjectProperty : PropertyDescriptor
{
    private Type _type;
    private bool _isReadOnly;
    private object _defaultValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicObjectProperty"/> class.
    /// </summary>
    /// <param name="descriptor">The descriptor.</param>
    public DynamicObjectProperty(PropertyDescriptor descriptor)
        : base(descriptor)
    {
        var atts = new List<Attribute>();
        foreach (Attribute att in descriptor.Attributes)
        {
            atts.Add(att);
        }
        Construct(descriptor.Name, descriptor.PropertyType, atts);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicObjectProperty"/> class.
    /// </summary>
    /// <param name="name">The property name.</param>
    /// <param name="type">The property type.</param>
    /// <param name="attributes">The property custom attributes.</param>
    public DynamicObjectProperty(string name, Type type, IEnumerable<Attribute> attributes)
        : base(name, GetAttributes(attributes))
    {
        Construct(name, type, attributes);
    }

    /// <summary>
    /// Constructs the current instance.
    /// </summary>
    /// <param name="name">The property name.</param>
    /// <param name="type">The property type.</param>
    /// <param name="attributes">The property custom attributes.</param>
    protected virtual void Construct(string name, Type type, IEnumerable<Attribute> attributes)
    {
        _type = type;

        var ro = Attributes.GetAttribute<ReadOnlyAttribute>();
        if (ro != null)
        {
            _isReadOnly = ro.IsReadOnly;
        }

        var dv = Attributes.GetAttribute<DefaultValueAttribute>();
        if (dv != null)
        {
            HasDefaultValue = true;
            _defaultValue = ConversionService.ChangeType(dv.Value, _type);
        }
        else
        {
            _defaultValue = ConversionService.ChangeType(null, _type);
        }
    }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return Name + " (" + _type.FullName + ")";
    }

    /// <summary>
    /// Gets or sets the default value.
    /// </summary>
    /// <value>The default value.</value>
    public virtual object DefaultValue
    {
        get
        {
            return _defaultValue;
        }
        set
        {
            _defaultValue = ConversionService.ChangeType(value, _type);
        }
    }

    /// <summary>
    /// Gets or sets the sort order.
    /// </summary>
    /// <value>The sort order.</value>
    public int SortOrder { get; set; }

    /// <summary>
    /// Gets a value indicating whether this instance has a default value.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance has a default value; otherwise, <c>false</c>.
    /// </value>
    public virtual bool HasDefaultValue { get; set; }

    private static Attribute[] GetAttributes(IEnumerable<Attribute> attributes)
    {
        var list = attributes == null ? new List<Attribute>() : new List<Attribute>(attributes);
        return list.ToArray();
    }

    /// <summary>
    /// When overridden in a derived class, returns whether resetting an object changes its value.
    /// </summary>
    /// <param name="component">The component to test for reset capability.</param>
    /// <returns>
    /// true if resetting the component changes its value; otherwise, false.
    /// </returns>
    public override bool CanResetValue(object component)
    {
        return HasDefaultValue;
    }

    /// <summary>
    /// When overridden in a derived class, gets the type of the component this property is bound to.
    /// </summary>
    /// <value></value>
    /// <returns>
    /// A <see cref="T:System.Type"/> that represents the type of component this property is bound to. When the <see cref="M:System.ComponentModel.PropertyDescriptor.GetValue(System.Object)"/> or <see cref="M:System.ComponentModel.PropertyDescriptor.SetValue(System.Object,System.Object)"/> methods are invoked, the object specified might be an instance of this type.
    /// </returns>
    public override Type ComponentType
    {
        get
        {
            return typeof(DynamicObject);
        }
    }

    /// <summary>
    /// When overridden in a derived class, gets the current value of the property on a component.
    /// </summary>
    /// <param name="component">The component with the property for which to retrieve the value.</param>
    /// <returns>
    /// The value of a property for a given component.
    /// </returns>
    public override object GetValue(object component)
    {
        var obj = component as DynamicObject;
        if (obj != null)
            return obj.GetPropertyValue(Name, _defaultValue);

        throw new ArgumentException("Component is not of the DynamicObject type", "component");
    }

    /// <summary>
    /// When overridden in a derived class, gets a value indicating whether this property is read-only.
    /// </summary>
    /// <value></value>
    /// <returns>true if the property is read-only; otherwise, false.
    /// </returns>
    public override bool IsReadOnly
    {
        get
        {
            return _isReadOnly;
        }
    }

    /// <summary>
    /// When overridden in a derived class, gets the type of the property.
    /// </summary>
    /// <value></value>
    /// <returns>
    /// A <see cref="T:System.Type"/> that represents the type of the property.
    /// </returns>
    public override Type PropertyType
    {
        get
        {
            return _type;
        }
    }

    /// <summary>
    /// When overridden in a derived class, resets the value for this property of the component to the default value.
    /// </summary>
    /// <param name="component">The component with the property value that is to be reset to the default value.</param>
    public override void ResetValue(object component)
    {
        if (HasDefaultValue)
        {
            SetValue(component, _defaultValue);
        }
    }

    /// <summary>
    /// When overridden in a derived class, sets the value of the component to a different value.
    /// </summary>
    /// <param name="component">The component with the property value that is to be set.</param>
    /// <param name="value">The new value.</param>
    public override void SetValue(object component, object value)
    {
        var obj = component as DynamicObject;
        if (obj != null)
        {
            obj.SetPropertyValue(Name, value);
            return;
        }

        throw new ArgumentException("Component is not of the DynamicObject type", "component");
    }

    /// <summary>
    /// When overridden in a derived class, determines a value indicating whether the value of this property needs to be persisted.
    /// </summary>
    /// <param name="component">The component with the property to be examined for persistence.</param>
    /// <returns>
    /// true if the property should be persisted; otherwise, false.
    /// </returns>
    public override bool ShouldSerializeValue(object component)
    {
        return false;
    }
}
