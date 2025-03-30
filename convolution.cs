/// <summary>
/// 素数mod上での畳み込み。MODはNTT-Friendlyじゃないと使い物にならない。
/// </summary>
public sealed class Convolution<T> where T : struct, IMod
{
    private readonly long _mod;
    private readonly ModInt<T> _primitiveRoot;
    private readonly int _maxExp;
    private readonly long _factor;
    private ModInt<T>[] _root;
    private ModInt<T>[] _inverseRoot;

    /// <summary>
    /// 初期化。計算量: modに依存するがおそらく軽い
    /// </summary>
    /// <param name="mod"></param>
    public Convolution()
    {
        _mod = ModInt<T>.Mod;
        _primitiveRoot = FindPrimitiveRoot(_mod);
        _maxExp = 0;
        long mm = _mod - 1;
        while ((mm & 1) == 0)
        {
            mm >>= 1;
            _maxExp++;
        }
        _factor = mm;

        _root = new ModInt<T>[_maxExp + 1];
        _inverseRoot = new ModInt<T>[_maxExp + 1];
        CalcRoot(_root, _inverseRoot);
    }

    private long FindPrimitiveRoot(long m)
    {
        if (m == 2) return 1;
        
        if (m == 167772161) return 3;
        if (m == 377487361) return 7;
        if (m == 469762049) return 3;
        if (m == 595591169) return 3;
        if (m == 645922817) return 3;
        if (m == 754974721) return 11;
        if (m == 880803841) return 26;
        if (m == 897581057) return 3;
        if (m == 998244353) return 3;

        List<long> divisors = new();
        long m1 = m - 1;
        for (long i = 2; i * i <= m1; i++)
        {
            if (m1 % i == 0)
            {
                while (m1 % i == 0) m1 /= i;
                divisors.Add(i);
            }
        }
        if (m1 > 1)
        {
            divisors.Add(m1);
        }

        Span<long> divSpan = CollectionsMarshal.AsSpan(divisors);

        for (long g = 2; g < m; g++)
        {
            bool ok = true;
            for (int i = 0; i < divSpan.Length; i++)
            {
                ok &= ModInt<T>.CreateFast(g).Power((m - 1) / divSpan[i]) != 1L;
            }

            if (ok)
            {
                return g;
            }
        }

        return -1;
    }

    private void CalcRoot(ModInt<T>[] root, ModInt<T>[] invroot)
    {
        root[0] = ModInt<T>.CreateFast(1L);
        invroot[0] = ModInt<T>.CreateFast(1L);

        root[_maxExp] = _primitiveRoot.Power(_factor);
        invroot[_maxExp] = root[_maxExp].Inv();
        for (int i = _maxExp - 1; i >= 1; i--)
        {
            root[i] = root[i + 1] * root[i + 1];
            invroot[i] = invroot[i + 1] * invroot[i + 1];
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private void ButterflyNTT(Span<ModInt<T>> target, int exp, Span<ModInt<T>> root)
    {
        if (target.Length == 1) return;

        int k = exp;
        int r = 1 << (k - 1);
        for (int m = k; m > 0; m--) 
        {
            for (int l = 0; l < target.Length; l += (r << 1)) 
            {
                ModInt<T> wi = ModInt<T>.One;
                for (int i = 0; i < r; i++) 
                {
                    ModInt<T> temp = target[l + i] + target[l + i + r];
                    target[l + i + r] = (target[l + i] - target[l + i + r]) * wi;
                    target[l + i] = temp;
                    wi *= root[m];
                }
            }
            r >>= 1;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private void ButterflyINTT(Span<ModInt<T>> target, int exp, Span<ModInt<T>> root)
    {
        if (target.Length == 1) return;
        
        int k = exp;
        int r = 1;
        for (int m = 1; m < k + 1; m++)
        {
            for (int l = 0; l < target.Length; l += (r << 1)) 
            {
                ModInt<T> wi = ModInt<T>.One;
                for (int i = 0; i < r; i++) 
                {
                    ModInt<T> temp = target[l + i] + target[l + i + r] * wi;
                    target[l + i + r] = target[l + i] - target[l + i + r] * wi;
                    target[l + i] = temp;
                    wi *= root[m];
                }
            }
            r <<= 1;
        }

        ModInt<T> ni = ModInt<T>.CreateFast(target.Length).Inv();
        for (int i = 0; i < target.Length; i++) {
            target[i] *= ni;
        }
    }

    /// <summary>
    /// aとbの畳み込みを計算して返す。計算量: O(NlogN), 定数倍はそこそこ重い。
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public ModInt<T>[] CalcConvolution(Span<ModInt<T>> a, Span<ModInt<T>> b)
    {
        int dsize = a.Length + b.Length;

        int exp = BitOperations.Log2((uint)dsize);
        if ((1 << exp) < dsize) exp++;
        int n = 1 << exp;

        Debug.Assert(exp <= _maxExp);

        ModInt<T>[] buffer = new ModInt<T>[n];
        ModInt<T>[] c = new ModInt<T>[n];

        a.CopyTo(c);
        b.CopyTo(buffer);

        ButterflyNTT(c, exp, _root);
        ButterflyNTT(buffer, exp, _root);

        for (int i = 0; i < n; i++)
        {
            c[i] *= buffer[i];
        }

        ButterflyINTT(c, exp, _inverseRoot);

        return c;
    }

    private static long SafeMod(long x, long m)
    {
        return x % m + ((x >> 63) & m);
    }

    private static long ExtEuclid(long a, long b, ref long p, ref long q)
    {
        if (b == 0)
        {
            p = 1;
            q = 0;
            return a;
        }
        long d = ExtEuclid(b, a % b, ref q, ref p);
        q -= a / b * p;
        return d;
    }

    private static (long rem, long mod) CRT(long x1, long m1, long x2, long m2)
    {
        long p = 0, q = 0;
        long d = ExtEuclid(m1, m2, ref p, ref q);
        if ((x2 - x1) % d != 0)
            return (0, -1);

        long m = m1 * (m2 / d);
        long temp = (x2 - x1) / d * p % (m2 / d);
        long r = SafeMod(x1 + m1 * temp, m);
        return (r, m);
    }

    private static (long rem, long mod) CRT(List<long> x, List<long> mod)
    {
        long r = 0, m = 1;
        for (int i = 0; i < x.Count; i++)
        {
            long p = 0, q = 0;
            long d = ExtEuclid(m, mod[i], ref p, ref q);
            if ((x[i] - r) % d != 0)
                return (0, -1);
            long temp = (x[i] - r) / d * p % (mod[i] / d);
            r += m * temp;
            m *= mod[i] / d;
        }

        return (SafeMod(r, m), m);
    }
}