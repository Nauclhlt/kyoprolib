// ローリングハッシュを生成する.
// 2通りのハッシュを生成, 比較する.
// 前計算O(|S|), 部分文字列のハッシュ取得定数時間.
// @author Nauclhlt.
public sealed class RabinKarp
{
    private const long BaseA = 3491;
    private const long ModA = 481840747;
    private const long BaseB = 8761;
    private const long ModB = 999750347;


    private long[] _prefixA;
    private long[] _prefixB;
    private long[] _powersA;
    private long[] _powersB;
    private string _source;

    public int Length => _source.Length;
    public string Source => _source;

    public RabinKarp(string s)
    {
        _source = s;

        int length = s.Length;
        ReadOnlySpan<char> span = s.AsSpan();

        _prefixA = new long[length + 1];
        _powersA = new long[length + 1];
        _powersA[0] = 1;

        _prefixB = new long[length + 1];
        _powersB = new long[length + 1];
        _powersB[0] = 1;

        for (int i = 0; i < length; i++)
        {
            long c = (long)span[i];
            _prefixA[i + 1] = (_prefixA[i] * BaseA + c) % ModA;
            _prefixB[i + 1] = (_prefixB[i] * BaseB + c) % ModB;
            _powersA[i + 1] = (_powersA[i] * BaseA) % ModA;
            _powersB[i + 1] = (_powersB[i] * BaseB) % ModB;
        }
    }

    public RollingHash GetHash(int l, int r)
    {
        if (r < l) throw new ArgumentException("! left <= right");

        long hashA = (_prefixA[r] - _powersA[r - l] * _prefixA[l]) % ModA;
        long hashB = (_prefixB[r] - _powersB[r - l] * _prefixB[l]) % ModB;
        if (hashA < 0) hashA += ModA;
        if (hashB < 0) hashB += ModB;

        return new RollingHash(r - l, hashA, hashB);
    }

    public RollingHash Concat(RollingHash a, RollingHash b)
    {
        long destA = (_powersA[b.Length] * a.HashA + b.HashA) % ModA;
        long destB = (_powersB[b.Length] * a.HashB + b.HashB) % ModB;
        if (destA < 0) destA += ModA;
        if (destB < 0) destB += ModB;

        return new RollingHash(a.Length + b.Length, destA, destB);
    }
}

public readonly struct RollingHash : IEquatable<RollingHash>
{
    private readonly long _hashA;
    private readonly long _hashB;
    private readonly int _length;

    public long HashA => _hashA;
    public long HashB => _hashB;
    public int Length => _length;

    public RollingHash(int length, long hashA, long hashB)
    {
        _length = length;
        _hashA = hashA;
        _hashB = hashB;
    }

    public bool Equals(RollingHash other)
    {
        return _length == other._length && _hashA == other._hashA && _hashB == other._hashB;
    }

    public override bool Equals([NotNullWhen(true)] object obj)
    {
        if (obj is RollingHash other)
        {
            return this.Equals(other);
        }
        else
            return false;
    }

    public override int GetHashCode() => (_length, _hashA, _hashB).GetHashCode();

    public static bool operator ==(RollingHash a, RollingHash b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(RollingHash a, RollingHash b)
    {
        return !a.Equals(b);
    }

    public override string ToString()
    {
        return $"{_length} {{{_hashA}, {_hashB}}}";
    }
}