/// <summary>
/// 解K個を管理するset
/// </summary>
/// <typeparam name="T"></typeparam>
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

    /// <summary>
    /// itemを追加する。計算量: O(logn)
    /// </summary>
    /// <param name="item"></param>
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

    /// <summary>
    /// itemを削除する。計算量: O(logn)
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
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

    /// <summary>
    /// itemが含まれるならtrue, そうでないならfalseを返す。計算量: O(logn)
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(T item)
    {
        return _high.Contains(item) || _low.Contains(item);
    }

    /// <summary>
    /// itemが下位K個に入っていなければtrue, そうでなければfalseを返す。計算量: O(logn)
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool HighContains(T item)
    {
        return _high.Contains(item);
    }

    /// <summary>
    /// itemが下位K個に入っていればtrue, そうでなければfalseを返す。計算量: O(logn)
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool LowContains(T item)
    {
        return _low.Contains(item);
    }

    /// <summary>
    /// itemの個数を返す。計算量: O(1)
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int CountOf(T item)
    {
        return _high.CountOf(item) + _low.CountOf(item);
    }

    /// <summary>
    /// itemが下位K個でない部分に入っている個数を返す。計算量: O(1)
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int HighCountOf(T item)
    {
        return _high.CountOf(item);
    }

    /// <summary>
    /// itemが下位K個に入っている個数を返す。計算量: O(1)
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int LowCountOf(T item)
    {
        return _low.CountOf(item);
    }

    /// <summary>
    /// 内容をクリアする。計算量: O(1)
    /// </summary>
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

    // itemを要素に追加する.
    // O(logN)
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

    // itemを削除する.
    // O(logN)
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

    // itemを含むかを返す.
    // O(1)
    public bool Contains(T item)
    {
        return _high.Contains(item) || _low.Contains(item);
    }

    // itemが上位k個に入っているかを返す.
    // O(1)
    public bool HighContains(T item)
    {
        return _high.Contains(item);
    }

    // itemが上位k個より小さい部分に入っているかを返す.
    // O(1)
    public bool LowContains(T item)
    {
        return _low.Contains(item);
    }

    // itemがいくつ含まれているかを返す.
    // O(1)
    public int CountOf(T item)
    {
        return _high.CountOf(item) + _low.CountOf(item);
    }

    // itemが上位k個にいくつ含まれているかを返す.
    // O(1)
    public int HighCountOf(T item)
    {
        return _high.CountOf(item);
    }

    // itemが上位k個より小さい部分にいくつ含まれているかを返す.
    // O(1)
    public int LowCountOf(T item)
    {
        return _low.CountOf(item);
    }

    // 中身をクリアする.
    // O(1)
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