namespace Skua.Core.Interfaces;
public interface IOption
{
    string Category { get; set; }
    string Name { get; }
    string DisplayName { get; }
    string Description { get; }
    object? DefaultValue { get; }
    bool Transient { get; }
    Type Type { get; }
}
