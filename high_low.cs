// 上位k個, および下位k個の値を管理する.
// 各操作O(logN).
// Depends on: NumberSet<T>
// @author Nauclhlt.
public sealed class LowHighSet<T> : IEnumerable<T> where T : struct, INumber<T>
{
    private NumberSet<T> _high;
    private T _highSum;
    private NumberSet<T> _low;
    private T _lowSum;
    private int _k;

    public NumberSet<T> High => _high;
    public NumberSet<T> Low => _low;
    public T HighSum => _highSum;
    public T LowSum => _lowSum;
    public int K => _k;
    public int Count => _high.Count + _low.Count;

    public LowHighSet(int k)
    {
        _high = new();
        _low = new();
        _highSum = T.CreateChecked(0);
        _lowSum = T.CreateChecked(0);
        _k = k;
    }

    public void Add(T item)
    {
        if (_low.Count < _k)
        {
            InsertLow(item);
        }
        else
        {
            if (item <= _low.Max)
            {
                UpMax();
                InsertLow(item);
            }
            else
            {
                InsertHigh(item);
            }
        }
    }

    public bool Remove(T item)
    {
        if (_low.Count < _k)
        {
            if (!_low.Contains(item)) return false;
            RemoveLow(item);
            return true;
        }
        else
        {
            if (item <= _low.Max)
            {
                if (!_low.Contains(item)) return false;
                RemoveLow(item);
                DownMin();

                return true;
            }
            else
            {
                if (!_high.Contains(item)) return false;
                RemoveHigh(item);

                return true;
            }
        }
    }

    private void InsertHigh(T item)
    {
        _high.Add(item);
        _highSum += item;
    }

    private void InsertLow(T item)
    {
        _low.Add(item);
        _lowSum += item;
    }

    private void RemoveHigh(T item)
    {
        _high.Remove(item);
        _highSum -= item;
    }

    private void RemoveLow(T item)
    {
        _low.Remove(item);
        _lowSum -= item;
    }

    private void DownMin()
    {
        if (_high.Count == 0) return;

        T min = _high.Min;
        RemoveHigh(min);
        InsertLow(min);
    }

    private void UpMax()
    {
        if (_low.Count == 0) return;

        T max = _low.Max;
        RemoveLow(max);
        InsertHigh(max);
    }

    public bool Contains(T item)
    {
        return _high.Contains(item) || _low.Contains(item);
    }

    public bool HighContains(T item)
    {
        return _high.Contains(item);
    }

    public bool LowContains(T item)
    {
        return _low.Contains(item);
    }

    public int CountOf(T item)
    {
        return _high.CountOf(item) + _low.CountOf(item);
    }

    public int HighCountOf(T item)
    {
        return _high.CountOf(item);
    }

    public int LowCountOf(T item)
    {
        return _low.CountOf(item);
    }

    public void Clear()
    {
        _high.Clear();
        _low.Clear();
        _highSum = T.CreateChecked(0);
        _lowSum = T.CreateChecked(0);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _high.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}

public sealed class HighLowSet<T> : IEnumerable<T> where T : struct, INumber<T>
{
    private NumberSet<T> _high;
    private T _highSum;
    private NumberSet<T> _low;
    private T _lowSum;
    private int _k;

    public NumberSet<T> High => _high;
    public NumberSet<T> Low => _low;
    public T HighSum => _highSum;
    public T LowSum => _lowSum;
    public int K => _k;
    public int Count => _high.Count + _low.Count;

    public HighLowSet(int k)
    {
        _high = new();
        _low = new();
        _highSum = T.CreateChecked(0);
        _lowSum = T.CreateChecked(0);
        _k = k;
    }

    public void Add(T item)
    {
        if (_high.Count < _k)
        {
            InsertHigh(item);
        }
        else
        {
            if (item >= _high.Min)
            {
                DownMin();
                InsertHigh(item);
            }
            else
            {
                InsertLow(item);
            }
        }
    }

    public bool Remove(T item)
    {
        if (_high.Count < _k)
        {
            if (!_high.Contains(item)) return false;
            RemoveHigh(item);
            return true;
        }
        else
        {
            if (item >= _high.Min)
            {
                if (!_high.Contains(item)) return false;
                RemoveHigh(item);
                UpMax();

                return true;
            }
            else
            {
                if (!_low.Contains(item)) return false;
                RemoveLow(item);

                return true;
            }
        }
    }

    private void InsertHigh(T item)
    {
        _high.Add(item);
        _highSum += item;
    }

    private void InsertLow(T item)
    {
        _low.Add(item);
        _lowSum += item;
    }

    private void RemoveHigh(T item)
    {
        _high.Remove(item);
        _highSum -= item;
    }

    private void RemoveLow(T item)
    {
        _low.Remove(item);
        _lowSum -= item;
    }

    private void DownMin()
    {
        if (_high.Count == 0) return;

        T min = _high.Min;
        RemoveHigh(min);
        InsertLow(min);
    }

    private void UpMax()
    {
        if (_low.Count == 0) return;

        T max = _low.Max;
        RemoveLow(max);
        InsertHigh(max);
    }

    public bool Contains(T item)
    {
        return _high.Contains(item) || _low.Contains(item);
    }

    public bool HighContains(T item)
    {
        return _high.Contains(item);
    }

    public bool LowContains(T item)
    {
        return _low.Contains(item);
    }

    public int CountOf(T item)
    {
        return _high.CountOf(item) + _low.CountOf(item);
    }

    public int HighCountOf(T item)
    {
        return _high.CountOf(item);
    }

    public int LowCountOf(T item)
    {
        return _low.CountOf(item);
    }

    public void Clear()
    {
        _high.Clear();
        _low.Clear();
        _highSum = T.CreateChecked(0);
        _lowSum = T.CreateChecked(0);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _high.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}