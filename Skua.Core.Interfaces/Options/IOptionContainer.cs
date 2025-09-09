namespace Skua.Core.Interfaces;

public interface IOptionContainer
{
    /// <summary>
    /// A dictionary containing lists of options paired with a category name.
    /// </summary>
    Dictionary<string, List<IOption>> MultipleOptions { get; }

    /// <summary>
    /// A list of the container's option definitions.
    /// </summary>
    List<IOption> Options { get; }

    /// <summary>
    /// The mapping of the container's option definitions to their values.
    /// </summary>
    string OptionsFile { get; set; }

    /// <summary>
    /// The mapping of the container's option definitions to their values.
    /// </summary>
    Dictionary<IOption, string> OptionValues { get; }

    /// <summary>
    /// Gets the option's value.
    /// </summary>
    /// <typeparam name="T">The type to return the value as.</typeparam>
    /// <param name="option">The option.</param>
    /// <returns>The value of the option converted from a string to type T.</returns>
    T? Get<T>(IOption? option) where T : IConvertible;

    /// <summary>
    /// Gets the option with the given name's value.
    /// </summary>
    /// <typeparam name="T">The type to return the value as.</typeparam>
    /// <param name="name">The name of the option.</param>
    /// <returns>The value of the option converted from a string to type T.</returns>
    T? Get<T>(string name) where T : IConvertible;

    /// <summary>
    /// Gets the option from the specified category in <see cref="MultipleOptions"/>
    /// </summary>
    /// <typeparam name="T">The type to return the value as.</typeparam>
    /// <param name="category">The name of the option category.</param>
    /// <param name="name">The name of the option.</param>
    /// <returns></returns>
    T? Get<T>(string category, string name) where T : IConvertible;

    /// <summary>
    /// Gets the option's value directly as a string.
    /// </summary>
    /// <param name="option">The option.</param>
    /// <returns>The option's value as a string.</returns>
    string GetDirect(IOption? option);

    /// <summary>
    /// Loads options from the container's options file, if it exists.
    /// </summary>
    void Load();

    /// <summary>
    /// Saves all non-transient options of this container's plugin to its options file.
    /// </summary>
    /// <remarks>This will overwrite previous options.</remarks>
    void Save();

    /// <summary>
    /// Sets the option with the given name to the given value.
    /// </summary>
    /// <param name="name">The name of the option.</param>
    /// <param name="value">The value to set the option to.</param>
    void Set(string name, object value);

    /// <summary>
    /// Sets the option from <see cref="MultipleOptions"/> to the given value
    /// </summary>
    /// <param name="category">The category name of the option.</param>
    /// <param name="name">The name of the option.</param>
    /// <param name="value">The value to set the option to.</param>
    void Set(string category, string name, object value);

    /// <summary>
    /// Sets the option to the given value of the given type.
    /// </summary>
    /// <typeparam name="T">The type of the value the option is being set to.</typeparam>
    /// <param name="option">The option.</param>
    /// <param name="value">The value to set the option to.</param>
    void Set<T>(IOption? option, T value);

    /// <summary>
    /// Sets this plugins options to the default values. This does not save the options.
    /// </summary>
    void SetDefaults();

    /// <summary>
    /// Opens the script option window and waits for the user to save the options.
    /// </summary>
    void Configure();
}