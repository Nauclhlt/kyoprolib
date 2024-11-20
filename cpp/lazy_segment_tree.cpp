template <typename T, typename M, T OP(T, T), T MAPPING(T, M, int), T COMPOSITION(T, T)>
class LazySegmentTree
{
private:
    int _treeSize;
    int _dataSize;
    int _originalDataSize;
    vector<T> _data;
    vector<optional<T>> _lazy;
    T _identity;

public:
    LazySegmentTree(int n, T identity)
    {
        _originalDataSize = n;
        _identity = identity;

        int size = 1;
        while (n > size)
        {
            size <<= 1;
        }

        _dataSize = size;
        _treeSize = 2 * size - 1;

        _data.resize(_treeSize, _identity);
        _lazy.resize(_treeSize, nullopt);
    }

    void Build(vector<T>& array)
    {
        if (_originalDataSize != (int)array.size())
        {
            throw exception();
        }

        for (int i = 0; i < (int)array.size(); i++)
        {
            _data[i + _dataSize - 1] = array[i];
        }

        for (int i = _dataSize - 2; i >= 0; i--)
        {
            _data[i] = OP(_data[(i << 1) + 1], _data[(i << 1) + 2]);
        }
    }

    int TreeSize()
    {
        return _treeSize;
    }

    int OriginalDataSize()
    {
        return _originalDataSize;
    }

    void Apply(int left, int right, M m)
    {
        ApplyRec(left, right, m, 0, 0, _dataSize);
    }

    T Query(int left, int right)
    {
        return QueryRec(left, right, 0, 0, _dataSize);
    }

    T GetByIndex(int index)
    {
        if (index < 0 || index >= _originalDataSize)
        {
            throw exception("index");
        }

        return AccessRec(index, 0, 0, _dataSize);
    }

private:
    void Evaluate(int index, int l, int r)
    {
        if (!_lazy[index].has_value())
        {
            return;
        }

        if (index < _dataSize - 1)
        {
            _lazy[(index << 1) + 1] = GuardComposition(_lazy[(index << 1) + 1], _lazy[index]);
            _lazy[(index << 1) + 2] = GuardComposition(_lazy[(index << 1) + 2], _lazy[index]);
        }

        _data[index] = MAPPING(_data[index], _lazy[index].value(), r - l);
        _lazy[index] = nullopt;
    }

    optional<M> GuardComposition(optional<M> a, optional<M> b)
    {
        if (!a.has_value())
        {
            return b;
        }
        else
        {
            return COMPOSITION(a.value(), b.value());
        }
    }

    void ApplyRec(int left, int right, M m, int index, int l, int r)
    {
        Evaluate(index, l, r);

        if (left <= l && r <= right)
        {
            _lazy[index] = GuardComposition(_lazy[index], m);
            Evaluate(index, l, r);
        }
        else if (left < r && l < right)
        {
            ApplyRec(left, right, m, (index << 1) + 1, l, (l + r) / 2);
            ApplyRec(left, right, m, (index << 1) + 2, (l + r) / 2, r);
            _data[index] = OP(_data[(index << 1) + 1], _data[(index << 1) + 2]);
        }
    }

    T QueryRec(int left, int right, int index, int l, int r)
    {
        Evaluate(index, l, r);

        if (left >= r || right <= l)
        {
            return _identity;
        }

        if (left <= l && r <= right)
        {
            return _data[index];
        }

        return OP(QueryRec(left, right, (index << 1) + 1, l, (l + r) / 2), QueryRec(left, right, (index << 1) + 2, (l + r) / 2, r));
    }

    T AccessRec(int target, int index, int l, int r)
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
};