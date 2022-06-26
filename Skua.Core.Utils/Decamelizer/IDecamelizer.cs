namespace Skua.Core.Utils;
public interface IDecamelizer
{
    string Decamelize(string text, DecamelizeOptions? options);
}
