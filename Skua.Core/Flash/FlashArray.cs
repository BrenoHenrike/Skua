using System.Collections;
using System.Collections.Generic;
using Skua.Core.Interfaces;

namespace Skua.Core.Flash;

public class FlashArray<T> : FlashObject<T[]>, IFlashArray<T>, IEnumerable<FlashObject<T>>
{

    public FlashArray(int id, IFlashUtil flashUtil) : base(id, flashUtil) { }

    public IFlashObject<T> Get(int index)
    {
        return new FlashObject<T>(FlashUtil.Call<int>("lnkGetArray", ID, index), FlashUtil);
    }

    public void Set(int index, T value)
    {
        FlashUtil.Call("lnkSetArray", ID, index, value!);
    }

    IEnumerator<FlashObject<T>> IEnumerable<FlashObject<T>>.GetEnumerator()
    {
        return new FlashArrayEnumerator<T>(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return new FlashArrayEnumerator<T>(this);
    }

    private class FlashArrayEnumerator<S> : IEnumerator<FlashObject<S>>
    {
        private int _index;
        private FlashArray<S> _array;
        private FlashObject<int> _length;

        public FlashObject<S> Current { get; set; }
        object IEnumerator.Current => Current;

        public FlashArrayEnumerator(FlashArray<S> array)
        {
            _array = array;
            _length = (FlashObject<int>)_array.GetChild<int>("length");
        }

        public void Dispose()
        {
            Current?.Dispose();
            _length.Dispose();
        }

        public bool MoveNext()
        {
            int length = _length.Value;
            if (_index >= length)
                return false;
            Current?.Dispose();
            Current = (FlashObject<S>)_array.Get(_index);
            _index++;
            return true;
        }

        public void Reset()
        {
            _index = 0;
            Current?.Dispose();
        }
    }
}
