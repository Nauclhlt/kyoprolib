public struct RollingHash
{
    private long _hashA;
    private long _hashB;
    private int _length;

    public long HashA => _hashA;
    public long HashB => _hashB;
    public int Length => _length;

    public static RollingHash Zero => new(0, 0, 0);

    public RollingHash()
    {
        _hashA = 0;
        _hashB = 0;
        _length = 0;
    }

    public RollingHash(long hashA, long hashB, int length)
    {
        _hashA = hashA;
        _hashB = hashB;
        _length = length;
    }

    public static bool operator ==(RollingHash left, RollingHash right)
    {
        return left._hashA == right._hashA && left._hashB == right._hashB && left._length == right._length;
    }

    public static bool operator !=(RollingHash left, RollingHash right)
    {
        return !(left == right);
    }

    public override int GetHashCode()
    {
        return (_hashA, _hashB).GetHashCode();
    }

    public override bool Equals([NotNullWhen(true)] object obj)
    {
        if (obj is RollingHash other)
        {
            return this == other;
        }
        else
        {
            return false;
        }
    }

    public static RollingHash FromChar(char c)
    {
        return new (c, c, 1);
    }
}

public static class RabinKarp
{
    public const long BaseA = 3491;
    public const long ModA = 481840747;
    public const long BaseB = 8761;
    public const long ModB = 999750347;

    private static long[] _powerA;
    private static long[] _powerB;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long SafeMod(long a, long m)
    {
        return ((a % m) + m) % m;
    }

    public static void Setup(int maxLength)
    {
        _powerA = new long[maxLength + 1];
        _powerB = new long[maxLength + 1];
        _powerA[0] = 1L;
        _powerB[0] = 1L;
        for (int i = 1; i <= maxLength; i++)
        {
            _powerA[i] = (_powerA[i - 1] * BaseA) % ModA;
            _powerB[i] = (_powerB[i - 1] * BaseB) % ModB;
        }
    }

    public static RollingHash PrefixDiff(RollingHash left, RollingHash right)
    {
        int diff = right.Length - left.Length;
        long hashA = SafeMod(right.HashA - (left.HashA * _powerA[diff]) % ModA, ModA);
        long hashB = SafeMod(right.HashB - (left.HashB * _powerB[diff]) % ModB, ModB);

        return new(hashA, hashB, diff);
    }

    public static RollingHash Concat(RollingHash left, RollingHash right)
    {
        int len = left.Length + right.Length;
        long hashA = ((left.HashA * _powerA[right.Length]) % ModA + right.HashA) % ModA;
        long hashB = ((left.HashB * _powerB[right.Length]) % ModB + right.HashB) % ModB;
        return new(hashA, hashB, len);
    }

    public static RollingHash HashFromString(string s)
    {
        long hashA = 0L;
        long hashB = 0L;
        for (int i = 0; i < s.Length; i++)
        {
            hashA = (hashA * BaseA) % ModA;
            hashA += s[i];
            hashA %= ModA;

            hashB = (hashB * BaseB) % ModB;
            hashB += s[i];
            hashB %= ModB;
        }

        return new(hashA, hashB, s.Length);
    }

    public static long PowerA(int exp)
    {
        return _powerA[exp];
    }

    public static long PowerB(int exp)
    {
        return _powerB[exp];
    }
}

public sealed class RollingHashString
{
    private int _length;
    private string _source;
    private RollingHash[] _prefix;

    public int Length => _length;
    public string Source => _source;

    public RollingHashString(string source)
    {
        _source = source;
        _length = source.Length;

        _prefix = new RollingHash[_length + 1];
        _prefix[0] = RollingHash.Zero;
        for (int i = 1; i <= _length; i++)
        {
            _prefix[i] = RabinKarp.Concat(_prefix[i - 1], RollingHash.FromChar(_source[i - 1]));
        }
    }

    public RollingHash GetHash(int l, int r)
    {
        return RabinKarp.PrefixDiff(_prefix[l], _prefix[r]);
    }

    public RollingHash GetPrefixHash(int length)
    {
        return _prefix[length];
    }
}