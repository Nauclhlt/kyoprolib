// 値とその数のペアを管理する.
// 各操作O(logN).
// @author Nauclhlt.
public sealed class NumberSet<T> : IEnumerable<T> where T : struct, INumber<T>
{
    private SortedSet<T> _set;
    private Dictionary<T, int> _countMap;
    private int _count;

    public SortedSet<T> SortedSet => _set;
    public Dictionary<T, int> CountMap => _countMap;
    public int Count => _count;
    public int UniqueCount => _set.Count;
    public T Max => _set.Max;
    public T Min => _set.Min;

    public NumberSet()
    {
        _set = new SortedSet<T>();
        _countMap = new Dictionary<T, int>();
        _count = 0;
    }

    public void Add(T item)
    {
        _count++;
        if (_countMap.ContainsKey(item)) _countMap[item]++;
        else
        {
            _countMap[item] = 1;
            _set.Add(item);
        }
    }

    public void AddMany(T item, int count)
    {
        _count += count;
        if (_countMap.ContainsKey(item)) _countMap[item] += count;
        else
        {
            _countMap[item] = count;
            _set.Add(item);
        }
    }

    public bool Remove(T item)
    {
        if (!_countMap.ContainsKey(item)) return false;

        _countMap[item]--;
        if (_countMap[item] == 0)
        {
            _countMap.Remove(item);
            _set.Remove(item);
        }
        _count--;

        return true;
    }

    public bool Contains(T item)
    {
        return _set.Contains(item);
    }

    public int CountOf(T item)
    {
        if (_countMap.TryGetValue(item, out int res)) return res;

        return 0;
    }

    public void Clear()
    {
        _set.Clear();
        _countMap.Clear();
        _count = 0;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return new NumberSetEnumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    private sealed class NumberSetEnumerator : IEnumerator<T>
    {
        private IEnumerator<T> _setEnumerator;
        private NumberSet<T> _set;
        private int _count = 0;
        private T _current;
        public T Current => _current;

        object IEnumerator.Current => _current;

        public NumberSetEnumerator(NumberSet<T> set)
        {
            _setEnumerator = set.SortedSet.GetEnumerator();
            _set = set;
        }

        public void Reset()
        {
            _setEnumerator.Reset();
        }
        public bool MoveNext()
        {
            if (_count == 0)
            {
                if (!_setEnumerator.MoveNext()) return false;
                T next = _setEnumerator.Current;
                _count = _set.CountMap[next];
                _current = next;
            }

            _count--;
            return true;
        }

        public void Dispose()
        {
        }
    }
}