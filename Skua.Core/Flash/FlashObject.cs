using Newtonsoft.Json;
using Skua.Core.Interfaces;

namespace Skua.Core.Flash;

public class FlashObject<T> : IFlashObject<T>, IDisposable
{
    public IFlashUtil FlashUtil { get; init; }
    public int ID { get; private set; }

    public FlashObject(int id, IFlashUtil flashUtil)
    {
        ID = id;
        FlashUtil = flashUtil;
    }

    public T? Value
    {
        get
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(FlashUtil.Call("lnkGetValue", ID)!);
            }
            catch
            {
                return default;
            }
        }
        set
        {
            FlashUtil.Call("lnkSetValue", ID, value!);
        }
    }

    public IFlashObject<R> GetChild<R>(string path)
    {
        return new FlashObject<R>(FlashUtil.Call<int>("lnkGetChild", ID, path), FlashUtil);
    }

    public void ClearChild(string path)
    {
        FlashUtil.Call("lnkDeleteChild", ID, path);
    }

    public void Dispose()
    {
        FlashUtil.Call("lnkDestroy", ID);
    }

    public IFlashArray<T> ToArray()
    {
        return new FlashArray<T>(ID, FlashUtil);
    }

    public IFlashObject<T> Create(string path)
    {
        return new FlashObject<T>(FlashUtil.Call<int>("lnkCreate", path), FlashUtil);
    }
}