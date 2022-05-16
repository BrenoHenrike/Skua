namespace Skua.Core.Interfaces;

public interface IFlashObject<T> : IDisposable
{
    IFlashUtil FlashUtil { get; init; }
    int ID { get; }
    T? Value { get; set; }

    IFlashObject<R> GetChild<R>(string path);
    void ClearChild(string path);
    IFlashArray<T> ToArray();
}
