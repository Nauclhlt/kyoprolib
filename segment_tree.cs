/// <summary>
/// セグメント木。一応非再帰の実装。
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class SegmentTree<T> where T : struct
{
    private int _treeSize;
    private int _dataSize;
    private int _originalDataSize;
    private T[] _data;
    private Monoid<T> _operator;
    private Monoid<T> _update;
    private T _identity;

    public int OriginalDataSize => _originalDataSize;
    public int TreeSize => _treeSize;
    public T Identity => _identity;

    /// <summary>
    /// 一点取得。計算量: O(1)
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public T this[int index]
    {
        get
        {
            return _data[_dataSize - 1 + index];
        }
    }

    /// <summary>
    /// 構築する。計算量: O(n)
    /// </summary>
    /// <param name="n"></param>
    /// <param name="op"></param>
    /// <param name="update"></param>
    /// <param name="identity"></param>
    public SegmentTree(int n, Monoid<T> op, Monoid<T> update, T identity)
    {
        _originalDataSize = n;

        int size = 1;
        while (n > size)
        {
            size <<= 1;
        }

        _dataSize = size;
        _treeSize = 2 * size - 1;

        _data = new T[_treeSize];
        Array.Fill(_data, identity);
        _identity = identity;
        _operator = op;
        _update = update;
    }

    /// <summary>
    /// <para>配列から再構築する。計算量: O(n)</para>
    /// <para>一点更新をn回繰り返すとO(nlogn)となるのでこれを呼んだ方が高速。</para>
    /// </summary>
    /// <param name="array"></param>
    public void Build(T[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            _data[i + _dataSize - 1] = array[i];
        }

        for (int i = _dataSize - 2; i >= 0; i--)
        {
            _data[i] = _operator(_data[(i << 1) + 1], _data[(i << 1) + 2]);
        }
    }

    public void UpdateByArray(T[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            _data[i + _dataSize - 1] = _update(_data[i + _dataSize - 1], array[i]);
        }

        for (int i = _dataSize - 2; i >= 0; i--)
        {
            _data[i] = _operator(_data[(i << 1) + 1], _data[(i << 1) + 2]);
        }
    }

    /// <summary>
    /// valueで埋める。計算量: O(n)
    /// </summary>
    /// <param name="value"></param>
    public void Fill(T value)
    {
        for (int i = 0; i < _originalDataSize; i++)
        {
            _data[i + _dataSize - 1] = value;
        }

        for (int i = _dataSize - 2; i >= 0; i--)
        {
            _data[i] = _operator(_data[(i << 1) + 1], _data[(i << 1) + 2]);
        }
    }

    /// <summary>
    /// 一点更新する。計算量: O(logn)
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    public void Update(int index, T value)
    {
        index += _dataSize - 1;
        _data[index] = _update(_data[index], value);

        while (index > 0)
        {
            index = (index - 1) >> 1;
            _data[index] = _operator(_data[(index << 1) + 1], _data[(index << 1) + 2]);
        }
    }

    /// <summary>
    /// 区間[l, r)の積を求める。計算量: O(logn)
    /// </summary>
    /// <param name="l"></param>
    /// <param name="r"></param>
    /// <returns></returns>
    public T Fold(int l, int r)
    {
        l += _dataSize - 1;
        r += _dataSize - 1;

        T leftFold = _identity;
        T rightFold = _identity;
        while (l < r)
        {
            if ((l & 1) == 0)
            {
                leftFold = _operator(leftFold, _data[l]);
                l++;
            }
            if ((r & 1) == 0)
            {
                r--;
                rightFold = _operator(_data[r], rightFold);
            }

            l = (l - 1) >> 1;
            r = (r - 1) >> 1;
        }

        return _operator(leftFold, rightFold);
    }

    /// <summary>
    /// 中身のspanを返す。計算量: O(1)
    /// </summary>
    /// <returns></returns>
    public ReadOnlySpan<T> AsSpan()
    {
        return _data.AsSpan(_dataSize - 1, _originalDataSize);
    }
}