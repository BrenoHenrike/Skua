namespace Skua.Core.Interfaces;
public interface IFlashArray<T>
{
    IFlashObject<T> Get(int index);
    void Set(int index, T value);
}
