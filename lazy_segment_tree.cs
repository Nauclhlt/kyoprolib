// 区間に対する操作と区間に対するクエリを処理する.
// 空間計算量: O(4N)
// 時間計算量:
// - 構築: O(N)
// - 操作: O(logN)
// - クエリ: O(logN)
// - 一点参照: O(logN)
// operator: クエリに対応する二項演算をする
// mapping: 作用素を作用させる
// composition: 作用素を合成する
public sealed class LazySegmentTree<T, M> where T : struct, IEquatable<T>  where M : struct, IEquatable<M>
{
    private int _treeSize;
    private int _dataSize;
    private int _originalDataSize;
    private T[] _data;
    private M?[] _lazy;
    private Monoid<T> _operator;
    private Apply<T, M> _mapping;
    private Monoid<M> _composition;
    private T _identity;

    public T this[int index]
    {
        get
        {
            return GetByIndex(index);
        }
    }

    public LazySegmentTree(int n, Monoid<T> op, Apply<T, M> mapping, Monoid<M> composition, T identity)
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
        _data.AsSpan().Fill(_identity);
        _lazy = new M?[_treeSize];

        _identity = identity;
        _operator = op;
        _mapping = mapping;
        _composition = composition;
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

    private void Evaluate(int index, int l, int r)
    {
        if (_lazy[index] is null)
        {
            return;
        }
        if (index < _dataSize - 1)
        {
            _lazy[(index << 1) + 1] = GuardComposition(_lazy[(index << 1) + 1], _lazy[index]);
            _lazy[(index << 1) + 2] = GuardComposition(_lazy[(index << 1) + 2], _lazy[index]);
        }

        _data[index] = _mapping(_data[index], (M)_lazy[index], r - l);
        _lazy[index] = null;
    }

    private M GuardComposition(M? a, M? b)
    {
        if (a is null)
        {
            return (M)b;
        }
        else
        {
            return _composition((M)a, (M)b);
        }
    }

    private void ApplyRec(int a, int b, M m, int index, int l, int r)
    {
        Evaluate(index, l, r);

        if (a <= l && r <= b)
        {
            _lazy[index] = GuardComposition(_lazy[index], m);
            Evaluate(index, l, r);
        }
        else if (a < r && l < b)
        {
            ApplyRec(a, b, m, (index << 1) + 1, l, (l + r) / 2);
            ApplyRec(a, b, m, (index << 1) + 2, (l + r) / 2, r);
            _data[index] = _operator(_data[(index << 1) + 1], _data[(index << 1) + 2]);
        }
    }

    public void Apply(int left, int right, M m)
    {
        ApplyRec(left, right, m, 0, 0, _dataSize);
    }

    public T Query(int left, int right)
    {
        return QueryRec(left, right, 0, 0, _dataSize);
    }

    private T QueryRec(int left, int right, int index, int nodeLeft, int nodeRight)
    {
        Evaluate(index, nodeLeft, nodeRight);

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

    public T GetByIndex(int target)
    {
        if (target < 0 || target >= _originalDataSize)
        {
            throw new Exception("Index is out of range.");
        }

        return AccessRec(target, 0, 0, _dataSize);
    }

    private T AccessRec(int target, int index, int l, int r)
    {
        Evaluate(index, l, r);

        if (index >= _dataSize - 1)
        {
            return _data[index];
        }

        int mid = (l + r) / 2;
        if (target < mid)
        {
            return AccessRec(target, (index << 1) + 1, l, mid);
        }
        else
        {
            return AccessRec(target, (index << 1) + 2, mid, r);
        }
    }

    // index以下のノードをすべて即時に評価する.
    private void EvaluateAll(int index, int l, int r)
    {
        if (_lazy[index] is null)
        {
            if (index < _dataSize - 1)
            {
                EvaluateAll((index << 1) + 1, l, (l + r) / 2);
                EvaluateAll((index << 1) + 2, (l + r) / 2, r);
            }
            return;
        }

        if (index < _dataSize - 1)
        {
            _lazy[(index << 1) + 1] = GuardComposition(_lazy[(index << 1) + 1], _lazy[index]);
            _lazy[(index << 1) + 2] = GuardComposition(_lazy[(index << 1) + 2], _lazy[index]);
            EvaluateAll((index << 1) + 1, l, (l + r) / 2);
            EvaluateAll((index << 1) + 2, (l + r) / 2, r);
        }

        _data[index] = _mapping(_data[index], (M)_lazy[index], r - l);
        _lazy[index] = null;
    }

    // 返されたArraySegment<T>は変更してはいけない.
    public ArraySegment<T> GetData()
    {
        EvaluateAll(0, 0, _dataSize);

        return new ArraySegment<T>(_data, _dataSize - 1, _originalDataSize);
    }
}