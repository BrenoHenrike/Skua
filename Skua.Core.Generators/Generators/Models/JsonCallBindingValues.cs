namespace Skua.Core.Generators.Models;

/// <summary>
/// A model describing the specific values a JsonCallBinding Attribute has.
/// </summary>
/// <param name="FunctionName">The name of the Flash function to call.</param>
/// <param name="Default">The default value to use if the call fails or returns null/empty.</param>
/// <param name="Get">Whether it should make a Flash Call to get the value.</param>
/// <param name="HasSetter">Whether the property should have a setter.</param>
public record struct JsonCallBindingValues(
    string FunctionName,
    string? Default,
    bool Get,
    bool HasSetter);
