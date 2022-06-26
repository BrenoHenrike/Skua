using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Skua.WPF;
[ContentProperty("DataTemplates")]
public class PropertyGridDataTemplateSelector : DataTemplateSelector
{
    private PropertyGrid _propertyGrid;

    public PropertyGridDataTemplateSelector()
    {
        DataTemplates = new ObservableCollection<PropertyGridDataTemplate>();
    }

    public PropertyGrid PropertyGrid
    {
        get
        {
            return _propertyGrid;
        }
    }

    protected virtual bool Filter(PropertyGridDataTemplate template, PropertyGridProperty property)
    {
        if (template == null)
            throw new ArgumentNullException("template");

        if (property == null)
            throw new ArgumentNullException("property");

        // check various filters
        if (template.IsCollection.HasValue && template.IsCollection.Value != property.IsCollection)
        {
            return true;
        }

        if (template.IsCollectionItemValueType.HasValue && template.IsCollectionItemValueType.Value != property.IsCollectionItemValueType)
        {
            return true;
        }

        if (template.IsValueType.HasValue && template.IsValueType.Value != property.IsValueType)
        {
            return true;
        }

        if (template.IsReadOnly.HasValue && template.IsReadOnly.Value != property.IsReadOnly)
        {
            return true;
        }

        if (template.IsError.HasValue && template.IsError.Value != property.IsError)
        {
            return true;
        }

        if (template.IsValid.HasValue && template.IsValid.Value != property.IsValid)
        {
            return true;
        }

        if (template.IsFlagsEnum.HasValue && template.IsFlagsEnum.Value != property.IsFlagsEnum)
        {
            return true;
        }

        if (template.Category != null && !property.Category.EqualsIgnoreCase(template.Category))
        {
            return true;
        }

        if (template.Name != null && !property.Name.EqualsIgnoreCase(template.Name))
        {
            return true;
        }

        return false;
    }

    public virtual bool IsAssignableFrom(Type type, Type propertyType, PropertyGridDataTemplate template, PropertyGridProperty property)
    {
        if (type == null)
            throw new ArgumentNullException("type");

        if (propertyType == null)
            throw new ArgumentNullException("propertyType");

        if (template == null)
            throw new ArgumentNullException("template");

        if (property == null)
            throw new ArgumentNullException("property");

        if (type.IsAssignableFrom(propertyType))
        {
            // bool? is assignable from bool, but we don't want that match
            if (!type.IsNullable() || propertyType.IsNullable())
                return true;
        }

        if (type == PropertyGridDataTemplate.NullableEnumType)
        {
            Type enumType;
            bool nullable;
            PropertyGridProperty.IsEnumOrNullableEnum(propertyType, out enumType, out nullable);
            if (nullable)
                return true;
        }

        var options = PropertyGridOptionsAttribute.FromProperty(property);
        if (options != null)
        {
            if ((type.IsEnum || type == typeof(Enum)) && options.IsEnum)
            {
                if (!options.IsFlagsEnum)
                    return true;

                if (Extensions.IsFlagsEnum(type))
                    return true;

                if (template.IsFlagsEnum.HasValue && template.IsFlagsEnum.Value)
                    return true;
            }
        }

        return false;
    }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (container == null)
            throw new ArgumentNullException("container");

        var property = item as PropertyGridProperty;
        if (property == null)
            return base.SelectTemplate(item, container);

        DataTemplate propTemplate = PropertyGridOptionsAttribute.SelectTemplate(property, item, container);
        if (propTemplate != null)
            return propTemplate;

        if (_propertyGrid == null)
        {
            _propertyGrid = container.GetVisualSelfOrParent<PropertyGrid>();
        }

        if (_propertyGrid.ValueEditorTemplateSelector != null && _propertyGrid.ValueEditorTemplateSelector != this)
        {
            DataTemplate template = _propertyGrid.ValueEditorTemplateSelector.SelectTemplate(item, container);
            if (template != null)
                return template;
        }

        foreach (PropertyGridDataTemplate template in DataTemplates)
        {
            if (Filter(template, property))
                continue;

            if (template.IsCollection.HasValue && template.IsCollection.Value)
            {
                if (string.IsNullOrWhiteSpace(template.CollectionItemPropertyType) && template.DataTemplate != null)
                    return template.DataTemplate;

                if (property.CollectionItemPropertyType != null)
                {
                    foreach (Type type in template.ResolvedCollectionItemPropertyTypes)
                    {
                        if (IsAssignableFrom(type, property.CollectionItemPropertyType, template, property))
                            return template.DataTemplate;
                    }
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(template.PropertyType) && template.DataTemplate != null)
                    return template.DataTemplate;

                foreach (Type type in template.ResolvedPropertyTypes)
                {
                    if (IsAssignableFrom(type, property.PropertyType, template, property))
                        return template.DataTemplate;
                }
            }
        }
        return base.SelectTemplate(item, container);
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public ObservableCollection<PropertyGridDataTemplate> DataTemplates { get; private set; }
}
