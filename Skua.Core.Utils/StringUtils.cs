using System.Text.RegularExpressions;

namespace Skua.Core.Utils;

public static class StringUtils
{
    public static readonly Regex RemoveLetter = new(@"[^0-9]", RegexOptions.Compiled);
    /// <summary>
    /// Removes all non numeric characters from a string.
    /// </summary>
    /// <param name="text">Text that will have the letters removed.</param>
    /// <returns>A string with only numeric characters</returns>
    public static string RemoveLetters(this string text)
    {
        return RemoveLetter.Replace(text, "");
    }

    /// <summary>
    /// Joins a string array with the defined separator.
    /// </summary>
    /// <param name="items">Array of items to be joined.</param>
    /// <param name="separator"><see cref="char"/> separator that will join the strings.</param>
    /// <returns>A string that consists of the elements of <paramref name="items"/> delimited by the <paramref name="separator"/> character.</returns>
    public static string Join(this string[] items, char separator)
    {
        return string.Join(separator, items);
    }

    /// <summary>
    /// Joins a string list with the defined separator.
    /// </summary>
    /// <param name="items">List of items to be joined.</param>
    /// <param name="separator"><see cref="char"/> separator that will join the strings.</param>
    /// <returns>A string that consists of the elements of <paramref name="items"/> delimited by the <paramref name="separator"/> character.</returns>
    public static string Join(this IEnumerable<string> items, char separator)
    {
        return string.Join(separator, items);
    }

    /// <summary>
    /// Joins a string array with the defined separator.
    /// </summary>
    /// <param name="items">Array of items to be joined.</param>
    /// <param name="separator"><see cref="string"/> separator that will join the strings.</param>
    /// <returns>A string that consists of the elements of <paramref name="items"/> delimited by the <paramref name="separator"/> string.</returns>
    public static string Join(this string[] items, string separator)
    {
        return string.Join(separator, items);
    }

    /// <summary>
    /// Joins a string list with the defined separator.
    /// </summary>
    /// <param name="items">List of items to be joined.</param>
    /// <param name="separator"><see cref="char"/> separator that will join the strings.</param>
    /// <returns>A string that consists of the elements of <paramref name="items"/> delimited by the <paramref name="separator"/> character.</returns>
    public static string Join(this IEnumerable<string> items, string separator)
    {
        return string.Join(separator, items);
    }
}
