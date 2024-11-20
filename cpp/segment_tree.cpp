template<typename T, T OP(T, T), T APPLY(T, T)>
class SegmentTree
{
private:
    int _treeSize;
    int _dataSize;
    int _originalDataSize;
    vector<T> _data;
    T _identity;

public:
    SegmentTree(int n, T identity)
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
    }

    int OriginalDataSize()
    {
        return _originalDataSize;
    }

    int TreeSize()
    {
        return _treeSize;
    }

    T Identity()
    {
        return _identity;
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

    void Apply(int index, T value)
    {
        index += _dataSize - 1;
        _data[index] = APPLY(_data[index], value);

        while (index > 0)
        {
            index = (index - 1) >> 1;
            _data[index] = OP(_data[(index << 1) + 1], _data[(index << 1) + 2]);
        }
    }

    T Query(int left, int right)
    {
        return QueryRec(left, right, 0, 0, _dataSize);
    }

    const T& operator[](size_t index) const
    {
        return _data[_dataSize - 1 + index];
    }

    T& operator[](size_t index)
    {
        return _data[_dataSize - 1 + index];
    }

private:
    T QueryRec(int left, int right, int index, int l, int r)
    {
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
};