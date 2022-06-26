using System;
using System.Linq;
using System.Windows.Markup;

namespace Skua.WPF.MarkupExtensions;
public class EnumBindingSourceExtension : MarkupExtension
{
    public Type EnumType { get; private set; }
    public EnumBindingSourceExtension(Type enumType)
    {
        if (enumType is null || !enumType!.IsEnum)
            throw new ArgumentException("Provided type is null or not an enum");

        EnumType = enumType;
    }
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var names = Enum.GetNames(EnumType);
        return names.Select(n => n.ToString().Replace('_', ' '));
    }
}
