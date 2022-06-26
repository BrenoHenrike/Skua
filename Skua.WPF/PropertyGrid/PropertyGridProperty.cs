using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Skua.WPF;
public class PropertyGridProperty : AutoObject, IComparable, IComparable<PropertyGridProperty>
{
    public event EventHandler<PropertyGridEventArgs> Event;
    private string _defaultEditorResourceKey;
    private object _clonedValue;
    private bool _valueCloned;

    public PropertyGridProperty(PropertyGridDataProvider dataProvider)
    {
        if (dataProvider == null)
            throw new ArgumentNullException("dataProvider");

        DataProvider = dataProvider;
        PropertyType = typeof(object);
        Attributes = dataProvider.CreateDynamicObject();
        TypeAttributes = dataProvider.CreateDynamicObject();
    }

    public override string ToString()
    {
        return Name;
    }

    public virtual void CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        var handler = Value as IPropertyGridCommandHandler;
        if (handler != null)
        {
            handler.CanExecute(this, sender, e);
        }
    }

    public virtual void Executed(object sender, ExecutedRoutedEventArgs e)
    {
        var handler = Value as IPropertyGridCommandHandler;
        if (handler != null)
        {
            handler.Executed(this, sender, e);
        }
    }

    public virtual void OnEvent(object sender, PropertyGridEventArgs e)
    {
        var handler = Event;
        if (handler != null)
        {
            handler(sender, e);
        }
    }

    public void UpdateCellBindings(Action<BindingExpression> action)
    {
        UpdateCellBindings(null, null, action);
    }

    public void UpdateCellBindings(string childName, Action<BindingExpression> action)
    {
        UpdateCellBindings(childName, null, action);
    }

    public virtual void UpdateCellBindings(string childName, Func<Binding, bool> where, Action<BindingExpression> action)
    {
        DataProvider.Grid.UpdateCellBindings(this, childName, where, action);
    }

    public static bool IsEnumOrNullableEnum(Type type, out Type enumType, out bool nullable)
    {
        if (type == null)
            throw new ArgumentNullException("type");

        nullable = false;
        if (type.IsEnum)
        {
            enumType = type;
            return true;
        }

        if (type.Name == typeof(Nullable<>).Name)
        {
            Type[] args = type.GetGenericArguments();
            if (args.Length == 1 && args[0].IsEnum)
            {
                enumType = args[0];
                nullable = true;
                return true;
            }
        }

        enumType = null;
        return false;
    }

    public static PropertyGridProperty FromEvent(RoutedEventArgs e)
    {
        if (e == null)
            return null;

        var fe = e.OriginalSource as FrameworkElement;
        if (fe == null)
            return null;

        return fe.DataContext as PropertyGridProperty;
    }

    public PropertyGridDataProvider DataProvider { get; private set; }
    public virtual int SortOrder { get; set; }
    public virtual DynamicObject Attributes { get; private set; }
    public virtual DynamicObject TypeAttributes { get; private set; }
    public virtual PropertyGridOptionsAttribute Options { get; set; }
    public virtual object Tag { get; set; }

    public virtual Type PropertyType { get { return GetProperty<Type>(); } set { SetProperty(value); } }
    public virtual string Name { get { return GetProperty<string>(); } set { SetProperty(value); } }
    public virtual bool IsError { get { return GetProperty<bool>(); } set { SetProperty(value); } }
    public virtual bool IsEnum { get { return GetProperty<bool>(); } set { SetProperty(value); } }
    public virtual bool IsFlagsEnum { get { return GetProperty<bool>(); } set { SetProperty(value); } }
    public virtual string Category { get { return GetProperty<string>(); } set { SetProperty(value); } }
    public virtual string DisplayName { get { return GetProperty<string>(); } set { SetProperty(value); } }
    public virtual string Description { get { return GetProperty<string>(); } set { SetProperty(value); } }
    public virtual bool HasDefaultValue { get { return GetProperty<bool>(); } set { SetProperty(value); } }
    public virtual PropertyDescriptor Descriptor { get { return GetProperty<PropertyDescriptor>(); } set { SetProperty(value); } }
    public virtual TypeConverter Converter { get { return GetProperty<TypeConverter>(); } set { SetProperty(value); } }

    public virtual object DefaultValue
    {
        get
        {
            return GetProperty<object>();
        }
        set
        {
            if (SetProperty(value))
            {
                DefaultValues["Value"] = value;
                OnPropertyChanged("IsDefaultValue");
            }
        }
    }

    public virtual string DefaultEditorResourceKey
    {
        get
        {
            if (_defaultEditorResourceKey != null)
                return _defaultEditorResourceKey;

            if (IsCollection)
                return "CollectionEditorWindow";

            return "ObjectEditorWindow";
        }
        set
        {
            _defaultEditorResourceKey = value;
        }
    }

    public virtual Type CollectionItemPropertyType
    {
        get
        {
            if (!IsCollection)
                return null;

            return Extensions.GetElementType(PropertyType);
        }
    }

    public virtual bool IsDefaultValue
    {
        get
        {
            if (!HasDefaultValue)
                return false;

            if (DefaultValue == null)
                return Value == null;

            return DefaultValue.Equals(Value);
        }
    }

    public virtual bool IsReadOnly
    {
        get
        {
            bool def = false;
            if (DataProvider != null && DataProvider.Grid != null && DataProvider.Grid.IsReadOnly)
            {
                def = true;
            }

            return GetProperty(def);
        }
        set
        {
            if (SetProperty(value))
            {
                OnPropertyChanged("IsReadWrite");
            }
        }
    }

    public virtual bool IsCollectionItemValueType
    {
        get
        {
            return CollectionItemPropertyType != null && CollectionItemPropertyType.IsValueType;
        }
    }

    public virtual bool IsValueType
    {
        get
        {
            return PropertyType != null && PropertyType.IsValueType;
        }
    }

    public virtual int CollectionCount
    {
        get
        {
            var enumerable = Value as IEnumerable;
            if (enumerable != null)
                return enumerable.Cast<object>().Count();

            return 0;
        }
    }

    public virtual bool IsCollection
    {
        get
        {
            if (PropertyType == null)
                return false;

            if (PropertyType == typeof(string))
                return false;

            return typeof(IEnumerable).IsAssignableFrom(PropertyType);
        }
    }

    public bool IsReadWrite
    {
        get
        {
            return !IsReadOnly;
        }
        set
        {
            IsReadOnly = !value;
        }
    }

    public bool? BooleanValue
    {
        get
        {
            if (Value == null)
                return null;

            bool def = HasDefaultValue && ConversionService.ChangeType(DefaultValue, false);
            return ConversionService.ChangeType(Value, def);
        }
        set
        {
            if (value == null)
            {
                Value = null;
                return;
            }

            Value = value.Value;
        }
    }

    public virtual string TextValue
    {
        get
        {
            if (Converter != null && Converter.CanConvertTo(typeof(string)))
                return (string)Converter.ConvertTo(Value, typeof(string));

            return ConversionService.ChangeType<string>(Value);
        }
        set
        {
            if (Converter != null)
            {
                if (Converter.CanConvertFrom(typeof(string)))
                {
                    Value = Converter.ConvertFrom(value);
                    return;
                }

                if (Descriptor != null && Converter.CanConvertTo(Descriptor.PropertyType))
                {
                    Value = Converter.ConvertTo(value, Descriptor.PropertyType);
                    return;
                }
            }

            if (Descriptor != null)
            {
                object v;
                if (ConversionService.TryChangeType(value, Descriptor.PropertyType, out v))
                {
                    Value = v;
                    return;
                }
            }
            Value = value;
        }
    }

    public virtual void OnValueChanged()
    {
        OnPropertyChanged("TextValue");
        OnPropertyChanged("BooleanValue");
        OnPropertyChanged("IsCollection");
        OnPropertyChanged("CollectionCount");
        OnPropertyChanged("IsDefaultValue");
    }

    public virtual void SetValue(object value, bool setChanged, bool forceRaise, bool trackChanged)
    {
        bool set = SetProperty("Value", value, setChanged, forceRaise, trackChanged);
        if (set || forceRaise)
        {
            OnValueChanged();
        }
    }

    public void ResetClonedValue()
    {
        _valueCloned = false;
    }

    public virtual void CloneValue(bool refresh)
    {
        if (_valueCloned && !refresh)
            return;

        var c = Value as ICloneable;
        _clonedValue = c != null ? c.Clone() : Value;
        _valueCloned = true;
    }

    public virtual object ClonedValue
    {
        get
        {
            CloneValue(false);
            return _clonedValue;
        }
    }

    public virtual object Value
    {
        get
        {
            return GetProperty<object>();
        }
        set
        {
            object changedValue;
            if (!TryChangeType(value, PropertyType, CultureInfo.CurrentCulture, out changedValue))
                throw new ArgumentException("Cannot convert value {" + value + "} to type '" + PropertyType.FullName + "'.");

            if (Descriptor != null)
            {
                try
                {
                    Descriptor.SetValue(DataProvider.Data, changedValue);
                    object finalValue = Descriptor.GetValue(DataProvider.Data);
                    SetValue(finalValue, true, false, true);
                }
                catch (Exception e)
                {
                    throw new ArgumentException("Cannot set value {" + value + "} to object.", e);
                }
            }
        }
    }

    protected virtual bool TryChangeType(object value, Type type, IFormatProvider provider, out object changedValue)
    {
        if (type == null)
            throw new ArgumentNullException("type");

        return ConversionService.TryChangeType(value, type, provider, out changedValue);
    }

    public virtual bool RaiseOnPropertyChanged(string name)
    {
        return OnPropertyChanged(name);
    }

    public virtual void OnDescribed()
    {
    }

    public virtual void RefreshValueFromDescriptor(bool setChanged, bool forceRaise, bool trackChanged)
    {
        if (Descriptor == null)
            return;

        try
        {
            object value = Descriptor.GetValue(DataProvider.Data);
            SetValue(value, setChanged, forceRaise, trackChanged);
        }
        catch (Exception e)
        {
            if (PropertyType == typeof(string))
            {
                Value = e.GetAllMessages();
            }
            IsError = true;
        }
    }

    int IComparable.CompareTo(object obj)
    {
        return CompareTo(obj as PropertyGridProperty);
    }

    public virtual int CompareTo(PropertyGridProperty other)
    {
        if (other == null)
            throw new ArgumentNullException("other");

        if (SortOrder != 0)
            return SortOrder.CompareTo(other.SortOrder);

        if (other.SortOrder != 0)
            return -other.SortOrder.CompareTo(0);

        if (DisplayName == null)
            return 1;

        return string.Compare(DisplayName, other.DisplayName, StringComparison.OrdinalIgnoreCase);
    }
}
