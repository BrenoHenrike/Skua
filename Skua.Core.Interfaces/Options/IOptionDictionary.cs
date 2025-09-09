using System.Collections.Immutable;

namespace Skua.Core.Interfaces;

public interface IOptionDictionary
{
    ImmutableDictionary<string, Func<object>> OptionDictionary { get; }
}