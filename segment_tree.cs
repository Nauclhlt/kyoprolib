// 一点に対する操作と区間に対するクエリを処理する.
// 空間計算量: O(2N)
// 時間計算量:
// - 構築: O(N)
// - 操作: O(logN)
// - クエリ: O(logN)
public sealed class SegmentTree<T> where T : struct
{
    private int _treeSize;
    private int _dataSize;
    private int _originalDataSize;
    private T[] _data;
    private Monoid<T> _operator;
    private Monoid<T> _apply;
    private T _identity;

    public int OriginalDataSize => _originalDataSize;
    public int TreeSize => _treeSize;
    public T Identity => _identity;

    public T this[int index]
    {
        get
        {
            return _data[_dataSize - 1 + index];
        }
    }

    public SegmentTree(int n, Monoid<T> op, Monoid<T> apply, T identity)
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
        _identity = identity;
        _operator = op;
        _apply = apply;
    }

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

    public void Apply(int index, T value)
    {
        index += _dataSize - 1;
        _data[index] = _apply(_data[index], value);

        while (index > 0)
        {
            index = (index - 1) >> 1;
            _data[index] = _operator(_data[(index << 1) + 1], _data[(index << 1) + 2]);
        }
    }

    public T Query(int left, int right)
    {
        return QueryRec(left, right, 0, 0, _dataSize);
    }

    private T QueryRec(int left, int right, int index, int nodeLeft, int nodeRight)
    {
        if (left >= nodeRight || right <= nodeLeft)
        {
            return _identity;
        }

        if (left <= nodeLeft && nodeRight <= right)
        {
            return _data[index];
        }

        T leftChild = QueryRec(left, right, (index << 1) + 1, nodeLeft, (nodeLeft + nodeRight) >> 1);
        T rightChild = QueryRec(left, right, (index << 1) + 2, (nodeLeft + nodeRight) >> 1, nodeRight);

        return _operator(leftChild, rightChild);
    }

    // 返されたArraySegment<T>は変更してはいけない.
    public ArraySegment<T> GetData()
    {
        return new ArraySegment<T>(_data, _dataSize - 1, _originalDataSize);
    }
}