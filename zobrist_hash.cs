public sealed class ZobristHash<T>
{
    public sealed class PrefixMultisetHash
    {
        private ulong[] _prefix;

        public int Length => _prefix.Length - 1;

        public PrefixMultisetHash(ulong[] prefix)
        {
            _prefix = prefix;
        }

        public ulong GetRange(int l, int r)
        {
            return _prefix[r] - _prefix[l];
        }

        public ulong GetPrefix(int length)
        {
            return _prefix[length];
        }
    }

    private Dictionary<T, ulong> _map;
    private Random _random;

    public ZobristHash()
    {
        _map = new();
        _random = new();
    }

    public ulong GetHash(T item)
    {
        if (_map.TryGetValue(item, out ulong hash)) return hash;
        else
        {
            ulong h = (ulong)_random.NextInt64();
            return _map[item] = h;
        }
    }

    public ulong GetMultisetHash(IEnumerable<T> sequence)
    {
        if (sequence is T[] array)
        {
            ulong hash = 0;
            for (int i = 0; i < array.Length; i++)
            {
                hash += GetHash(array[i]);
            }
            return hash;
        }
        else if (sequence is IList<T> list)
        {
            ulong hash = 0;
            for (int i = 0; i < list.Count; i++)
            {
                hash += GetHash(list[i]);
            }
            return hash;
        }
        else
        {
            ulong hash = 0;
            foreach (T item in sequence)
            {
                hash += GetHash(item);
            }
            return hash;
        }
    }

    public ulong GetUniqueSetHash(IEnumerable<T> sequence)
    {
        if (sequence is T[] array)
        {
            ulong hash = 0;
            for (int i = 0; i < array.Length; i++)
            {
                hash ^= GetHash(array[i]);
            }
            return hash;
        }
        else if (sequence is IList<T> list)
        {
            ulong hash = 0;
            for (int i = 0; i < list.Count; i++)
            {
                hash ^= GetHash(list[i]);
            }
            return hash;
        }
        else
        {
            ulong hash = 0;
            foreach (T item in sequence)
            {
                hash ^= GetHash(item);
            }
            return hash;
        }
    }

    public PrefixMultisetHash CreatePrefixMultisetHash(IList<T> list)
    {
        ulong[] prefix = new ulong[list.Count + 1];
        prefix[0] = 0;
        for (int i = 1; i <= list.Count; i++)
        {
            prefix[i] = prefix[i - 1] + GetHash(list[i - 1]);
        }

        return new (prefix);
    }

    public bool IsEqualMultiset(IEnumerable<T> a, IEnumerable<T> b)
    {
        return GetMultisetHash(a) == GetMultisetHash(b);
    }

    public bool IsEqualUniqueSet(IEnumerable<T> a, IEnumerable<T> b)
    {
        return GetUniqueSetHash(a) == GetUniqueSetHash(b);
    }
}