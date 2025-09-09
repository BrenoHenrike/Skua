using Skua.Core.Utils;

namespace Skua.WPF;

public static class DecamelizationService
{
    public static string Decamelize(string text)
    {
        return Decamelize(text, null);
    }

    public static string Decamelize(string text, DecamelizeOptions? options)
    {
        return PropertyGridServiceProvider.Current.GetService<IDecamelizer>().Decamelize(text, options);
    }
}