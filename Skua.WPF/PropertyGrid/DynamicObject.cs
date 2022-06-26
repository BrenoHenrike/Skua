using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using System.Threading;

namespace Skua.WPF;
/// <summary>
/// Represents a dynamic object whose properties can be added or removed at runtime.
/// </summary>
public class DynamicObject : ICustomTypeDescriptor, IFormattable, INotifyPropertyChanged, IDataErrorInfo
{
    private readonly List<Attribute> _attributes = new List<Attribute>();
    private readonly List<EventDescriptor> _events = new List<EventDescriptor>();
    private readonly List<PropertyDescriptor> _properties = new List<PropertyDescriptor>();
    private readonly Dictionary<Type, object> _editors = new Dictionary<Type, object>();
    private readonly Dictionary<string, object> _values = new Dictionary<string, object>();

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Adds a new property.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="type">The type.</param>
    /// <param name="attributes">The property custom attributes or null.</param>
    /// <returns>An instance of the DynamicObjectProperty type.</returns>
    public virtual DynamicObjectProperty AddProperty(string name, Type type, IEnumerable<Attribute> attributes)
    {
        if (name == null)
            throw new ArgumentNullException("name");

        if (type == null)
            throw new ArgumentNullException("type");

        if (_properties.Find(x => x.Name == name) != null)
            throw new ArgumentException("Property '" + name + "' is already defined", "name");

        DynamicObjectProperty dop = CreateProperty(name, type, attributes);
        _properties.Add(dop);
        return dop;
    }

    /// <summary>
    /// Adds a new property.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="type">The type.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <param name="readOnly">if set to <c>true</c> the property is read only.</param>
    /// <param name="sortOrder">The property sort order.</param>
    /// <param name="attributes">The property custom attributes or null.</param>
    /// <returns>
    /// An instance of the DynamicObjectProperty type.
    /// </returns>
    public virtual DynamicObjectProperty AddProperty(string name, Type type, object defaultValue, bool readOnly, int sortOrder, Attribute[] attributes)
    {
        if (name == null)
            throw new ArgumentNullException("name");

        if (type == null)
            throw new ArgumentNullException("type");

        if (_properties.Find(x => x.Name == name) != null)
            throw new ArgumentException("Property '" + name + "' is already defined", "name");

        List<Attribute> newAtts;
        if (attributes != null)
        {
            newAtts = new List<Attribute>(attributes);
        }
        else
        {
            newAtts = new List<Attribute>();
        }

        newAtts.RemoveAll(a => a is ReadOnlyAttribute);
        newAtts.RemoveAll(a => a is DefaultValueAttribute);

        if (readOnly)
        {
            newAtts.Add(new ReadOnlyAttribute(true));
        }
        newAtts.Add(new DefaultValueAttribute(defaultValue));

        DynamicObjectProperty dop = CreateProperty(name, type, newAtts.ToArray());
        dop.SortOrder = sortOrder;
        _properties.Add(dop);
        return dop;
    }

    /// <summary>
    /// Sorts the properties using the specified comparer.
    /// </summary>
    /// <param name="comparer">The comparer to use.</param>
    public void SortProperties(IComparer<PropertyDescriptor> comparer)
    {
        if (comparer == null)
            throw new ArgumentNullException("comparer");

        _properties.Sort(comparer);
    }

    /// <summary>
    /// Gets a typed property value.
    /// </summary>
    /// <param name="name">The property name.</param>
    /// <param name="type">The expected type.</param>
    /// <param name="defaultValue">The default value if the property value is not defined.</param>
    /// <returns>The property value or the default value.</returns>
    public virtual object GetPropertyValue(string name, Type type, object defaultValue)
    {
        if (name == null)
            throw new ArgumentNullException("name");

        if (type == null)
            throw new ArgumentNullException("type");

        defaultValue = ConversionService.ChangeType(defaultValue, type);
        object obj;
        if (_values.TryGetValue(name, out obj))
            return ConversionService.ChangeType(obj, type, defaultValue);

        return defaultValue;
    }

    /// <summary>
    /// Gets a typed property value.
    /// </summary>
    /// <typeparam name="T">The expected type.</typeparam>
    /// <param name="name">The property name.</param>
    /// <param name="defaultValue">The default value if the property value is not defined.</param>
    /// <returns>The property value or the default value.</returns>
    public virtual T GetPropertyValue<T>(string name, T defaultValue)
    {
        if (name == null)
            throw new ArgumentNullException("name");

        object obj;
        if (_values.TryGetValue(name, out obj))
            return ConversionService.ChangeType(obj, defaultValue);

        return defaultValue;
    }

    /// <summary>
    /// Gets the property value.
    /// </summary>
    /// <param name="name">The property name.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// true if  the property value exists; false otherwise.
    /// </returns>
    public virtual bool TryGetPropertyValue(string name, out object value)
    {
        if (name == null)
            throw new ArgumentNullException("name");

        return _values.TryGetValue(name, out value);
    }

    /// <summary>
    /// Gets a raw property value.
    /// </summary>
    /// <param name="name">The property name.</param>
    /// <param name="defaultValue">The default value if the property value is not defined.</param>
    /// <returns>The property value or the default value.</returns>
    public virtual object GetPropertyValue(string name, object defaultValue)
    {
        if (name == null)
            throw new ArgumentNullException("name");

        object obj;
        if (_values.TryGetValue(name, out obj))
            return obj;

        return defaultValue;
    }

    /// <summary>
    /// Sets the property value.
    /// </summary>
    /// <param name="name">The property name.</param>
    /// <param name="value">The value.</param>
    public virtual void SetPropertyValue(string name, object value)
    {
        if (name == null)
            throw new ArgumentNullException("name");

        object existing;
        bool exists = _values.TryGetValue(name, out existing);
        if (!exists)
        {
            _values.Add(name, value);
        }
        else
        {
            if (value == null)
            {
                if (existing == null)
                    return;

            }
            else if (value.Equals(existing))
                return;

            _values[name] = value;
        }
        OnPropertyChanged(name);
    }

    /// <summary>
    /// Creates a property object.
    /// </summary>
    /// <param name="name">The property name. May not be null.</param>
    /// <param name="type">The property type. May not be null.</param>
    /// <param name="attributes">The property custom attributes or null.</param>
    /// <returns>
    /// An instance of the DynamicObjectProperty type.
    /// </returns>
    protected virtual DynamicObjectProperty CreateProperty(string name, Type type, IEnumerable<Attribute> attributes)
    {
        if (name == null)
            throw new ArgumentNullException("name");

        return new DynamicObjectProperty(name, type, attributes);
    }

    /// <summary>
    /// Returns the name used as the return of a ToString() call.
    /// </summary>
    /// <value>The name used as the return of a ToString() call.</value>
    /// <returns>
    /// The name used as the return of a ToString() call, or null if the default value is to be used.
    /// </returns>
    public virtual string ToStringName { get; set; }

    /// <summary>
    /// Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return ToStringName ?? base.ToString();
    }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public virtual string ToString(string format, IFormatProvider formatProvider)
    {
        if (string.IsNullOrEmpty(format))
            return ToString();

        return Extensions.Format(this, format, formatProvider);
    }

    /// <summary>
    /// Gets the attributes.
    /// </summary>
    /// <value>The attributes.</value>
    public virtual IList<Attribute> Attributes
    {
        get
        {
            return _attributes;
        }
    }

    AttributeCollection ICustomTypeDescriptor.GetAttributes()
    {
        return new AttributeCollection(_attributes.ToArray());
    }

    /// <summary>
    /// Returns the class name of this instance of a component.
    /// </summary>
    /// <value>The class name.</value>
    /// <returns>
    /// The class name of the object, or null if the class does not have a name.
    /// </returns>
    public virtual string ClassName { get; set; }

    string ICustomTypeDescriptor.GetClassName()
    {
        return ClassName;
    }

    /// <summary>
    /// Returns the name of this instance of a component.
    /// </summary>
    /// <value>The component name.</value>
    /// <returns>
    /// The name of the object, or null if the object does not have a name.
    /// </returns>
    public virtual string ComponentName { get; set; }

    string ICustomTypeDescriptor.GetComponentName()
    {
        return ComponentName;
    }

    /// <summary>
    /// Returns a type converter for this instance of a component.
    /// </summary>
    /// <value>The converter.</value>
    /// <returns>
    /// A <see cref="T:System.ComponentModel.TypeConverter"/> that is the converter for this object, or null if there is no <see cref="T:System.ComponentModel.TypeConverter"/> for this object.
    /// </returns>
    public virtual TypeConverter Converter { get; set; }

    TypeConverter ICustomTypeDescriptor.GetConverter()
    {
        return Converter;
    }

    /// <summary>
    /// Returns the default event for this instance of a component.
    /// </summary>
    /// <value>The default event.</value>
    /// <returns>
    /// An <see cref="T:System.ComponentModel.EventDescriptor"/> that represents the default event for this object, or null if this object does not have events.
    /// </returns>
    public virtual EventDescriptor DefaultEvent { get; set; }

    EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
    {
        return DefaultEvent;
    }

    /// <summary>
    /// Returns the default property for this instance of a component.
    /// </summary>
    /// <value>The default property.</value>
    /// <returns>
    /// A <see cref="T:System.ComponentModel.PropertyDescriptor"/> that represents the default property for this object, or null if this object does not have properties.
    /// </returns>
    public virtual PropertyDescriptor DefaultProperty { get; set; }

    PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
    {
        return DefaultProperty;
    }

    /// <summary>
    /// Gets the editors.
    /// </summary>
    /// <value>The editors.</value>
    public virtual IDictionary<Type, object> Editors
    {
        get
        {
            return _editors;
        }
    }

    object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
    {
        if (editorBaseType == null)
            throw new ArgumentNullException("editorBaseType");

        object editor;
        if (_editors.TryGetValue(editorBaseType, out editor))
            return editor;

        return null;
    }

    /// <summary>
    /// Gets the events.
    /// </summary>
    /// <value>The events.</value>
    public virtual IList<EventDescriptor> Events
    {
        get
        {
            return _events;
        }
    }

    EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
    {
        if (attributes == null || attributes.Length == 0)
            return ((ICustomTypeDescriptor)this).GetEvents();

        var list = new List<EventDescriptor>();
        foreach (EventDescriptor evt in _events)
        {
            if (evt.Attributes.Count == 0)
                continue;

            bool cont = false;
            foreach (var att in attributes)
            {
                if (!HasMatchingAttribute(evt, att))
                {
                    cont = true;
                    break;
                }
            }

            if (!cont)
            {
                list.Add(evt);
            }
        }
        return new EventDescriptorCollection(list.ToArray());
    }

    private static bool HasMatchingAttribute(MemberDescriptor member, Attribute attribute)
    {
        var att = member.Attributes[attribute.GetType()];
        if (att == null)
            return attribute.IsDefaultAttribute();

        return attribute.Match(att);
    }

    EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
    {
        return new EventDescriptorCollection(_events.ToArray());
    }

    PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
    {
        if (attributes == null || attributes.Length == 0)
            return ((ICustomTypeDescriptor)this).GetProperties();

        var list = new List<PropertyDescriptor>();
        foreach (PropertyDescriptor prop in _properties)
        {
            if (prop.Attributes.Count == 0)
                continue;

            bool cont = false;
            foreach (Attribute att in attributes)
            {
                if (!HasMatchingAttribute(prop, att))
                {
                    cont = true;
                    break;
                }
            }

            if (!cont)
            {
                list.Add(prop);
            }
        }
        return new PropertyDescriptorCollection(list.ToArray());
    }

    /// <summary>
    /// Called when a property value has changed.
    /// </summary>
    /// <param name="name">The property name.</param>
    protected virtual void OnPropertyChanged(string name)
    {
        PropertyChangedEventHandler handler = PropertyChanged;
        if (handler != null)
        {
            handler(this, new PropertyChangedEventArgs(name));
        }
    }

    /// <summary>
    /// Gets the properties.
    /// </summary>
    /// <value>The properties.</value>
    public virtual IList<PropertyDescriptor> Properties
    {
        get
        {
            return _properties;
        }
    }

    PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
    {
        return new PropertyDescriptorCollection(_properties.ToArray());
    }

    object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
    {
        return this;
    }

    /// <summary>
    /// Validates the whole object.
    /// </summary>
    /// <returns>
    /// A text describing the error or null if there was no error.
    /// </returns>
    public string Validate()
    {
        return Validate(null);
    }

    /// <summary>
    /// Validates the member.
    /// </summary>
    /// <param name="memberName">The name of the member to validate or null to validate the whole object.</param>
    /// <returns>
    /// A text describing the error or null if there was no error.
    /// </returns>
    public string ValidateMember(string memberName)
    {
        return ValidateMember(null, memberName);
    }

    /// <summary>
    /// Validates the member.
    /// </summary>
    /// <param name="culture">The culture to use for error messages.</param>
    /// <returns>
    /// A text describing the error or null if there was no error.
    /// </returns>
    public string Validate(CultureInfo culture)
    {
        return ValidateMember(culture, null);
    }

    /// <summary>
    /// Validates the member.
    /// </summary>
    /// <param name="culture">The culture to use for error messages.</param>
    /// <param name="memberName">The name of the member to validate or null to validate the whole object.</param>
    /// <returns>
    /// A text describing the error or null if there was no error.
    /// </returns>
    public string ValidateMember(CultureInfo culture, string memberName)
    {
        return ValidateMember(culture, memberName, null);
    }

    /// <summary>
    /// Validates the member.
    /// </summary>
    /// <param name="culture">The culture to use for error messages.</param>
    /// <param name="memberName">The name of the member to validate or null to validate the whole object.</param>
    /// <param name="separator">The separator string to use.</param>
    /// <returns>A text describing the error or null if there was no error.</returns>
    public virtual string ValidateMember(CultureInfo culture, string memberName, string separator)
    {
        if (culture == null)
        {
            culture = Thread.CurrentThread.CurrentUICulture;
        }

        if (separator == null)
        {
            separator = Environment.NewLine;
        }

        var list = new List<ValidationException>();
        ValidateMember(culture, list, memberName);
        if (list.Count == 0)
            return null;

        var sb = new StringBuilder();
        foreach (ValidationException e in list)
        {
            if (sb.Length != 0)
            {
                sb.Append(separator);
            }
            sb.Append(e.GetAllMessages(separator));
        }
        return sb.ToString();
    }

    /// <summary>
    /// Validates the member.
    /// </summary>
    /// <param name="culture">The culture to use for error messages.</param>
    /// <param name="list">The list of exception to fill.</param>
    /// <param name="memberName">The name of the member to validate or null to validate the whole object.</param>
    public virtual void ValidateMember(CultureInfo culture, IList<ValidationException> list, string memberName)
    {
        if (list == null)
            throw new ArgumentNullException("list");
    }

    string IDataErrorInfo.Error
    {
        get
        {
            return Validate();
        }
    }

    string IDataErrorInfo.this[string columnName]
    {
        get
        {
            return ValidateMember(columnName);
        }
    }
}
