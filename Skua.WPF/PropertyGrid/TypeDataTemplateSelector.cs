using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Skua.WPF;

[ContentProperty("DataTemplates")]
public class TypeDataTemplateSelector : DataTemplateSelector
{
    public TypeDataTemplateSelector()
    {
        DataTemplates = new ObservableCollection<DataTemplate>();
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public ObservableCollection<DataTemplate> DataTemplates { get; private set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        foreach (DataTemplate dt in DataTemplates.Where(dt => dt.DataType is Type))
        {
            var type = (Type)dt.DataType;
            if (item == null)
            {
                if (!type.IsValueType)
                    return dt;
            }
            else
            {
                if (type.IsInstanceOfType(item))
                    return dt;
            }
        }
        return DataTemplates.FirstOrDefault(dt => dt.DataType == null);
    }
}