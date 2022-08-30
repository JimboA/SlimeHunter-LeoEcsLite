using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace JimboA.Plugins
{
    /// <summary>
    /// Just a list without additional checks and with a public array of elements (can be pretty useful).
    /// Approximately 20 percent faster than standard List<T>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public class FastList<T> : IEnumerable<T>
    {
        public T[] Items;
        public ref T this[int index] => ref Items[index];
        public int Length => _length;

        private int _length;
        private IEqualityComparer<T> _comparer;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FastList()
        {
            Items = new T[6];
            _length = 0;
            _comparer = EqualityComparer<T>.Default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FastList(int cap = 0)
        {
            Items = new T[cap > 0 ? cap : 6];
            _length = 0;
            _comparer = EqualityComparer<T>.Default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FastList(IEqualityComparer<T> comparer, int cap = 0)
        {
            Items = new T[cap > 0 ? cap : 6];
            _length = 0;
            _comparer = comparer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T item)
        {
            if (_length >= Items.Length)
                Grow(_length << 1);

            Items[_length++] = item;
        }
        
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRange(IEnumerable<T> items)
        {
            if (items == null)
                throw new ArgumentNullException (nameof(items));

            if (items is ICollection<T> collection)
            {
                var count = collection.Count;
                if(count <= 0)
                    return;

                var newLen = _length += count;
                if(newLen > Items.Length)
                    Grow(newLen << 1);
                
                collection.CopyTo(Items, _length);
                _length = newLen;
            }
            else
            {
                using var item = items.GetEnumerator ();
                while (item.MoveNext ()) 
                {
                    Add(item.Current);
                }
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Insert(T item, int index)
        {
            if(index >= Items.Length || index < 0)
                return;
            
            if (_length >= Items.Length)
                Grow(_length << 1);
            
            Array.Copy(Items, index, Items, index + 1, _length++);
            Items[index] = item;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(T item)
        {
            int index = IndexOf(ref item);
            var removed = index > -1;
            if (removed && index < --_length)
                Array.Copy(Items, index + 1, Items, index, _length - index);
            return removed;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(ref T item)
        {
            int index = IndexOf(ref item);
            var removed = index > -1;
            if (removed && index < --_length)
                Array.Copy(Items, index + 1, Items, index, _length - index);
            return removed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(T item, out int index)
        {
            index = IndexOf(ref item);
            var removed = index > -1;
            if (removed && index < --_length)
                Array.Copy(Items, index + 1, Items, index, _length - index);
            return removed;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(ref T item, out int index)
        {
            index = IndexOf(ref item);
            var removed = index > -1;
            if (removed && index < --_length)
                Array.Copy(Items, index + 1, Items, index, _length - index);
            return removed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveAt(int index)
        {
            _length--;
            if(index < _length)
                Array.Copy(Items, index + 1, Items, index, _length - index);
            Items[_length] = default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearAt(int index)
        {
            if (index <= 0)
            {
                Clear();
                return;
            }
            _length = index;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(ref T item)
        {
            for (int i = 0; i < _length; i++)
            {
                if (_comparer.Equals(Items[i], item))
                    return i;
            }

            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            _length = 0;
        }

        public bool TryGetLast(out T item)
        {
            if (_length > 0)
            {
                item = Items[_length - 1];
                return true;
            }
            
            item = default;
            return false;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArray()
        {
            var arr = new T[_length];
            Array.Copy(Items, arr, _length);
            return arr;
        }

        public void Sort(int index, int length)
        {
            Array.Sort(Items, index, length);
        }

        public void Sort<TKeys>(TKeys[] keys, int index, int length)
        {
            Array.Sort(keys, Items, index, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Grow(int newLength)
        {
            var arr = new T[newLength];
            Array.Copy(Items, arr, _length);
            Items = arr;
        }

        #region Enumerator

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        [System.Serializable]
        public struct Enumerator : IEnumerator<T>
        {
            public FastList<T> List;
            public int Index;

            public T Current => List.Items[Index];

            object IEnumerator.Current => Current;

            internal Enumerator(FastList<T> list)
            {
                List = list;
                Index = list._length;
            }

            [MethodImpl (MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                return --Index >= 0;
            }

            [MethodImpl (MethodImplOptions.AggressiveInlining)]
            public void Reset()
            {
                Index = List._length;
            }

            [MethodImpl (MethodImplOptions.AggressiveInlining)]
            public void Dispose()
            {
                List = null;
            }
        }

        #endregion
    }
}