using Skua.Core.Models;

namespace Skua.Core.Interfaces;
public interface IHotKeyService
{
    void Reload();
    List<T> GetHotKeys<T>()
        where T : IHotKey, new();

    HotKey? ParseToHotKey(string keyGesture);
}
