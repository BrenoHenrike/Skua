namespace Skua.Core.Generators.Models;
/// <summary>
/// A model describing the specific values an Object Binding has.
/// </summary>
/// <param name="Paths">Names of the properties. Multiple names are mostly used for setting multiple values.</param>
/// <param name="Get">Whether it should make a Flash Call to get the value.</param>
/// <param name="Set">Whether it should make a Flash Call to set the value.</param>
/// <param name="Select">Name of the value to select when retrieving a list of objects.</param>
/// <param name="RequireNotNull">Name of the object that must not be null to get the value.</param>
/// <param name="IsStatic">Whether the value is static.</param>
/// <param name="HasSetter">Whether the property should have a setter.</param>
public record struct ObjectBindingValues(
    string[] Paths,
    bool Get,
    bool Set,
    string? Select,
    string? RequireNotNull,
    string? Default,
    bool IsStatic,
    bool HasSetter);