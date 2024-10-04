public sealed class WaveletMatrix
{
    private List<BitVector> _bitArrays = new();
    private int[] _beginOne;
    private Dictionary<ulong, int> _begins;
    private ulong[][] _prefix;

    private int _length;
    private ulong _maxValue;
    private int _bitLength;

    public int Length => _length;

    public WaveletMatrix(int[] array)
    {
        ulong[] data = new ulong[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            data[i] = (ulong)array[i];
        }

        Construct(data);
    }

    public WaveletMatrix(long[] array)
    {
        ulong[] data = new ulong[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            data[i] = (ulong)array[i];
        }

        Construct(data);
    }

    public WaveletMatrix(ulong[] array)
    {
        Construct(array);
    }

    private void Construct(ulong[] array)
    {
        _length = array.Length;
        _maxValue = array.Max() + 1;
        _bitLength = GetBitLength(_maxValue);

        if (_bitLength == 0) _bitLength++;

        for (int i = 0; i < _bitLength; i++)
        {
            BitVector bv = new(_length);
            _bitArrays.Add(bv);
        }

        _beginOne = new int[_bitLength];
        _prefix = new ulong[_bitLength + 1][];
        for (int i= 0; i <= _bitLength; i++)
        {
            _prefix[i] = new ulong[_length + 1];
        }

        for (int i = 0; i < array.Length; i++)
        {
            _prefix[0][i + 1] = _prefix[0][i] + array[i];
        }

        List<ulong> v = new(array);
        List<ulong> temp = new(v.Count);
        for (int i = 0; i < _bitLength; i++)
        {
            temp.Clear();
            
            for (int j = 0; j < v.Count; j++)
            {
                ulong c = v[j];
                int bit = (int)((c >> (_bitLength - i - 1)) & 1);
                if (bit == 0)
                {
                    temp.Add(c);
                    _bitArrays[i][j] = 0;
                }
            }

            _beginOne[i] = temp.Count;

            for (int j = 0; j < v.Count; j++)
            {
                ulong c = v[j];
                int bit = (int)((c >> (_bitLength - i - 1)) & 1);
                if (bit == 1)
                {
                    temp.Add(c);
                    _bitArrays[i][j] = 1;
                }
            }

            for (int j = 0; j < temp.Count; j++)
            {
                _prefix[i + 1][j + 1] = _prefix[i + 1][j] + temp[j];
            }

            _bitArrays[i].Build();
            (v, temp) = (temp, v);
        }

        _begins = new();

        for (int i = v.Count - 1; i >= 0; i--)
        {
            _begins[v[i]] = i;
        }
    }

    public ulong Access(int pos)
    {
        if (pos >= _length) return ulong.MaxValue;

        ulong c = 0;
        for (int i = 0; i < _bitArrays.Count; i++)
        {
            int bit = _bitArrays[i][pos];
            c = (c <<= 1) | (uint)bit;
            pos = _bitArrays[i].Rank(bit, pos);
            if (bit == 1)
            {
                pos += _beginOne[i];
            }
        }

        return c;
    }

    public int Select(ulong c, int rank)
    {
        rank++;

        if (c >= _maxValue) return -1;
        if (!_begins.ContainsKey(c)) return -1;

        int index = _begins[c] + rank;
        for (int i = 0; i < _bitArrays.Count; i++)
        {
            int bit = (int)((c >> i) & 1);
            if (bit == 1)
            {
                index -= _beginOne[_bitLength - i - 1];
            }
            index = _bitArrays[_bitLength - i - 1].Select(bit, index);
        }

        return index - 1;
    }

    public int RangeMax(int l, int r)
    {
        return QuantileRange(l, r, r - l - 1);
    }

    public int RangeMin(int l, int r)
    {
        return QuantileRange(l, r, 0);
    }

    public int QuantileRange(int l, int r, int k)
    {
        if (r > _length || l >= _length || k >= r - l)
        {
            return -1;
        }

        ulong val = 0;
        for (int i = 0; i < _bitLength; i++)
        {
            int zeroBegin = _bitArrays[i].Rank0(l);
            int zeroEnd = _bitArrays[i].Rank0(r);
            int zero = zeroEnd - zeroBegin;
            int bit = (k < zero) ? 0 : 1;

            if (bit == 1)
            {
                k -= zero;
                l = _beginOne[i] + l - zeroBegin;
                r = _beginOne[i] + r - zeroEnd;
            }
            else
            {
                l = zeroBegin;
                r = zeroBegin + zero;
            }

            val = (val << 1) | (uint)bit;
        }

        int left = 0;
        for (int i = 0; i < _bitLength; i++)
        {
            int bit = (int)((val >> (_bitLength - i - 1)) & 1);
            left = _bitArrays[i].Rank(bit, left);
            if (bit == 1)
            {
                left += _beginOne[i];
            }
        }

        int rank = l + k - left;
        return Select(val, rank);
    }

    public int Rank(ulong c, int pos)
    {
        if (c >= _maxValue) return 0;
        if (!_begins.ContainsKey(c)) return 0;

        for (int i = 0; i < _bitLength; i++)
        {
            int bit = (int)((c >> (_bitLength - i - 1)) & 1);
            pos = _bitArrays[i].Rank(bit, pos);
            if (bit == 1)
            {
                pos += _beginOne[i];
            }
        }

        int begin = _begins[c];
        return pos - begin;
    }

    public (int, int, int) RankAll(ulong c, int l, int r)
    {
        int len = r - l;

        if (l >= r)
        {
            return (0, 0, 0);
        }

        if (c >= _maxValue || r == 0)
        {
            return (0, len, 0);
        }

        int rankLess = 0;
        int rankMore = 0;
        for (int i = 0; i < _bitLength && l < r; i++)
        {
            int bit = (int)((c >> (_bitLength - i - 1)) & 1);

            int rank0Begin = _bitArrays[i].Rank0(l);
            int rank0End = _bitArrays[i].Rank0(r);
            int rank1Begin = l - rank0Begin;
            int rank1End = r - rank0End;

            if (bit == 1)
            {
                rankLess += rank0End - rank0Begin;
                l = _beginOne[i] + rank1Begin;
                r = _beginOne[i] + rank1End;
            }
            else
            {
                rankMore += rank1End - rank1Begin;
                l = rank0Begin;
                r = rank0End;
            }
        }

        int rank = len - rankLess - rankMore;
        return (rank, rankLess, rankMore);
    }

    // 区間[l, r)内の値[min, max)の値の個数を返す.
    public int RankFrequency(int l, int r, ulong min, ulong max)
    {
        if (r > _length || l >= r || min >= max || min >= _maxValue) 
        {
            return 0;
        }

        var maxt = RankAll(max, l, r);
        var mint = RankAll(min, l, r);
        return maxt.Item1 - mint.Item1;
    }

    public int RankLessThan(ulong c, int l, int r)
    {
        return RankAll(c, l, r).Item1;
    }

    public int RankMoreThan(ulong c, int l, int r)
    {
        return RankAll(c, l, r).Item2;
    }

    public static int GetBitLength(ulong value)
    {
        int size = 0;
        while (value > 0)
        {
            value >>= 1;
            size++;
        }

        return size;
    }
}

public sealed class BitVector
{
    private int[] _data;
    private int[] _prefix;

    public int Length => _data.Length;

    public int this[int index]
    {
        get => _data[index];
        set 
        {
            _data[index] = value;
        }
    }

    public BitVector(int length)
    {
        _data = new int[length];
    }

    public BitVector(int[] data)
    {
        _data = data;
    }

    public void Build()
    {
        if (_prefix is null) _prefix = new int[_data.Length + 1];
        _prefix[0] = 0;
        for (int i = 1; i <= _data.Length; i++)
        {
            _prefix[i] = _prefix[i - 1] + _data[i - 1];
        }
    }

    public int Rank0(int i)
    {
        if (i <= 0) return 0;
        i = int.Min(i, _data.Length);
        return i - Rank1(i);
    }

    public int Rank1(int i)
    {
        if (i <= 0) return 0;
        i = int.Min(i, _data.Length);
        return _prefix[i];
    }

    public int Rank(int bit, int i)
    {
        if (bit == 0) return Rank0(i);
        else return Rank1(i);
    }

    public int AllRank0()
    {
        return Rank0(_data.Length);
    }

    public int AllRank1()
    {
        return Rank1(_data.Length);
    }

    public int Select0(int k)
    {
        if (k <= 0 || k > AllRank0()) return -1;

        int left = 0;
        int right = _data.Length;

        while (right > left)
        {
            int mid = left + (right - left) / 2;
            if (Rank0(mid) < k)
            {
                left = mid + 1;
            }
            else
            {
                right = mid;
            }
        }

        return left;
    }

    public int Select1(int k)
    {
        if (k <= 0 || k > AllRank1()) return -1;

        int left = 0;
        int right = _data.Length;

        while (right > left)
        {
            int mid = left + (right - left) / 2;
            if (Rank1(mid) < k)
            {
                left = mid + 1;
            }
            else
            {
                right = mid;
            }
        }

        return left;
    }

    public int Select(int bit, int k)
    {
        if (bit == 0) return Select0(k);
        else return Select1(k);
    }
}