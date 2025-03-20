/// <summary>
/// FPSを扱う。定数倍はあんまり良くない。
/// </summary>
public sealed class FPS
{
    private const long Mod = 998244353L;
    private static Convolution Conv = new(Mod);

    private long[] _coef;

    public long[] Coef => _coef;
    public int Length => _coef.Length;
    public int MaxExponent => _coef.Length - 1;

    public long this[int index]
    {
        get 
        {
            if (index < 0 || index >= _coef.Length) return 0;
            return _coef[index];
        }
        set => _coef[index] = value;
    }

    public FPS(long[] sequence)
    {
        _coef = sequence;
    }

    public FPS(ModInt[] sequence)
    {
        _coef = new long[sequence.Length];
        for (int i = 0; i < _coef.Length; i++)
        {
            _coef[i] = sequence[i].Raw();
        }
    }

    public FPS(int length)
    {
        _coef = new long[length];
    }

    public static FPS CopyCreate(long[] sequence)
    {
        long[] copy = new long[sequence.Length];
        Array.Copy(sequence, copy, sequence.Length);
        return new(copy);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long SafeMod(long a)
    {
        return ((a % Mod) + Mod) % Mod;
    }

    /// <summary>
    /// 長さを変更する。計算量: O(n)
    /// </summary>
    /// <param name="length"></param>
    public void Resize(int length)
    {
        Array.Resize(ref _coef, length);
    }

    /// <summary>
    /// 長さをlengthに変更したものを取得する。計算量: O(n)
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public FPS GetResized(int length)
    {
        long[] res = new long[length];
        Array.Copy(_coef, res, int.Min(_coef.Length, res.Length));
        return new(res);
    }

    /// <summary>
    /// aのasize項とbのbsize項の積を計算する。計算量: O(nlogn)
    /// </summary>
    /// <param name="a"></param>
    /// <param name="asize"></param>
    /// <param name="b"></param>
    /// <param name="bsize"></param>
    /// <returns></returns>
    public static FPS ResizedMultiply(FPS a, int asize, FPS b, int bsize)
    {
        long[] res = Conv.CalcConvolution(a._coef.AsSpan(0, asize), b._coef.AsSpan(0, bsize));
        return new FPS(res);
    }

    public static FPS operator -(FPS target)
    {
        long[] res = new long[target.Length];
        for (int i = 0; i < target.Length; i++) res[i] = SafeMod(-target[i]);

        return new(res);
    }

    public static FPS operator +(FPS left, FPS right)
    {
        int len = int.Max(left.Length, right.Length);
        long[] res = new long[len];
        for (int i = 0; i < left.Length; i++) res[i] += left[i];
        for (int i = 0; i < right.Length; i++) res[i] = SafeMod(res[i] + right[i]);

        return new (res);
    }

    public static FPS operator -(FPS left, FPS right)
    {
        int len = int.Max(left.Length, right.Length);
        long[] res = new long[len];
        for (int i = 0; i < left.Length; i++) res[i] += left[i];
        for (int i = 0; i < right.Length; i++) res[i] = SafeMod(res[i] - right[i]);

        return new (res);
    }

    public static FPS operator *(FPS left, FPS right)
    {
        long[] res = Conv.CalcConvolution(left.Coef, right.Coef);
        int len = left.MaxExponent + right.MaxExponent + 1;
        Array.Resize(ref res, len);
        return new(res);
    }

    /// <summary>
    /// 逆元の先頭n項を求める。計算量: O(nlogn)
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public FPS Inv(int n)
    {
        if (_coef[0] == 0) throw new Exception("No Inv");
        FPS g = new(new long[]{ Convolution.CalcPow(_coef[0], Mod - 2, Mod) });
        int k = 1;
        while (k < n)
        {
            k *= 2;
            FPS fg = ResizedMultiply(this, int.Min(Length, k), g, g.Length);
            fg.Resize(k);
            for (int i = 0; i < fg.Length; i++) fg[i] = SafeMod(-fg[i]);
            fg[0] = SafeMod(fg[0] + 2);
            g *= fg;
            g.Resize(k);
        }
        g.Resize(n);

        return g;
    }

    /// <summary>
    /// expの先頭n項を求める。計算量: O(nlogn)
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public FPS Exp(int n)
    {
        FPS g = new(new long[] {1});
        int k = 1;
        while (k < n)
        {
            k <<= 1;
            FPS logG = g.Log(k);
            for (int i = 0; i < k; i++) logG[i] = SafeMod(this[i] - logG[i]);
            logG[0] = SafeMod(logG[0] + 1);
            g *= logG;
            g.Resize(k);
        }

        g.Resize(n);
        return g;
    }

    /// <summary>
    /// 微分を求める。計算量: O(n)
    /// </summary>
    /// <returns></returns>
    public FPS Diff()
    {
        FPS result = new (Length);
        for (int i = 1; i < Length; i++)
        {
            result[i - 1] = SafeMod(_coef[i] * i);
        }
        return result;
    }

    /// <summary>
    /// 積分を求める。計算量: O(n)
    /// </summary>
    /// <returns></returns>
    public FPS Integral()
    {
        FPS result = new(Length + 1);
        for (int i = 1; i <= Length; i++)
        {
            result[i] = SafeMod(_coef[i - 1] * Convolution.CalcPow(i, Mod - 2, Mod));
        }
        return result;
    }

    /// <summary>
    /// in-placeに積分を求める。計算量: O(n)
    /// </summary>
    public void InplaceIntegral()
    {
        for (int i = Length - 1; i >= 1; i--)
        {
            _coef[i] = SafeMod(_coef[i - 1] * Convolution.CalcPow(i, Mod - 2, Mod));
        }
        _coef[0] = 0;
    }

    /// <summary>
    /// logの先頭n項を求める。計算量: O(nlogn)
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public FPS Log(int n)
    {
        if (_coef[0] != 1)
        {
            throw new InvalidOperationException("Cannot define log(f(x)) for f(x) such that [x^0]f(x) != 1.");
        }

        FPS df = Diff();
        FPS inv = Inv(n);

        //FPS log = df * inv;
        FPS log = ResizedMultiply(df, int.Min(df.Length, n), inv, inv.Length);
        log.Resize(n);

        log.InplaceIntegral();
        return log;
    }

    /// <summary>
    /// exponent乗の先頭n項を求める。計算量: O(nlogn), 絶望的に定数倍が重く使えない可能性がある
    /// </summary>
    /// <param name="exponent"></param>
    /// <param name="n"></param>
    /// <returns></returns>
    public FPS Power(int exponent, int n)
    {
        FPS log = Log(n);
        for (int i = 0; i < log.Length; i++)
        {
            log[i] = SafeMod(log[i] * exponent);
        }
        return log.Exp(n);
    }
}

public sealed class Convolution
{
    private readonly long _mod;
    private readonly long _primitiveRoot;
    private readonly int _maxExp;
    private readonly long _factor;
    private long[] _root;
    private long[] _inverseRoot;

    public Convolution(long mod = 998244353L)
    {
        _mod = mod;
        _primitiveRoot = FindPrimitiveRoot(mod);
        _maxExp = 0;
        long mm = mod - 1;
        while ((mm & 1) == 0)
        {
            mm >>= 1;
            _maxExp++;
        }
        _factor = mm;

        _root = new long[_maxExp + 1];
        _inverseRoot = new long[_maxExp + 1];
        CalcRoot(_root);

        for (int i = 0; i <= _maxExp; i++)
        {
            _inverseRoot[i] = Inverse(_root[i], _mod);
        }
    }

    private static long Inverse(long a, long mod)
    {
        return CalcPow(a, mod - 2, mod);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long CalcPow(long b, long exp, long mod)
    {
        b %= mod;

        long res = 1L;
        while (exp > 0)
        {
            if ((exp & 1L) == 1L)
            {
                res *= b;
                res %= mod;
            }
            b *= b;
            b %= mod;
            exp >>= 1;
        }

        return res;
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

        for (long g = 2; g <= m; g++)
        {
            bool ok = true;
            for (int i = 0; i < divSpan.Length; i++)
            {
                ok &= CalcPow(g, (m - 1) / divSpan[i], m) != 1L;
            }

            if (ok)
            {
                return g;
            }
        }

        return -1;
    }

    private void CalcRoot(long[] root)
    {
        root[0] = 1L;

        root[_maxExp] = CalcPow(_primitiveRoot, _factor, _mod);
        for (int i = _maxExp - 1; i >= 1; i--)
        {
            root[i] = (root[i + 1] * root[i + 1]) % _mod;
        }
    }
    
    private void ButterflyNTT(Span<long> target, int exp, long[] root)
    {
        if (target.Length == 1) return;

        int n = target.Length;
        int k = exp;
        int r = 1 << (k - 1);
        for (int m = k; m > 0; m--) 
        {
            for (int l = 0; l < n; l += (r << 1)) 
            {
                long wi = 1;
                for (int i = 0; i < r; i++) 
                {
                    long temp = (target[l + i] + target[l + i + r]) % _mod;
                    target[l + i + r] = (target[l + i] - target[l + i + r]) * wi % _mod;
                    target[l + i] = temp;
                    wi = wi * root[m] % _mod;
                }
            }
            r >>= 1;
        }
    }

    private void ButterflyINTT(Span<long> target, int exp, long[] root)
    {
        if (target.Length == 1)
            return;
        int n = target.Length;
        int k = exp;
        int r = 1;
        for (int m = 1; m < k + 1; m++)
        {
            for (int l = 0; l < n; l += (r << 1)) 
            {
                long wi = 1;
                for (int i = 0; i < r; i++) 
                {
                    long temp = (target[l + i] + target[l + i + r] * wi) % _mod;
                    target[l + i + r] = (target[l + i] - target[l + i + r] * wi) % _mod;
                    target[l + i] = temp;
                    wi = wi * root[m] % _mod;
                }
            }
            r <<= 1;
        }

        long ni = Inverse(n, _mod);
        for (int i = 0; i < n; i++) {
            target[i] = ((target[i] * ni % _mod) + _mod) % _mod;
        }
    }

    public long[] CalcConvolution(Span<long> a, Span<long> b)
    {
        int dsize = a.Length + b.Length;

        int exp = 0;
        while ((1 << exp) < dsize)
        {
            exp++;
        }
        int n = 1 << exp;

        if (exp > _maxExp)
        {
            throw new InvalidOperationException("Data too large.");
        }

        long[] buffer = new long[n];
        long[] c = new long[n];

        //Array.Copy(a, 0, c, 0, a.Length);
        //Array.Copy(b, 0, buffer, 0, b.Length);
        a.CopyTo(c.AsSpan(0, a.Length));
        b.CopyTo(buffer.AsSpan(0, b.Length));

        ButterflyNTT(c, exp, _root);
        ButterflyNTT(buffer, exp, _root);

        for (int i = 0; i < n; i++)
        {
            c[i] *= buffer[i];
            c[i] %= _mod;
        }

        ButterflyINTT(c, exp, _inverseRoot);

        return c;
    }

    private static long SafeMod(long x, long m)
    {
        x %= m;
        if (x < 0) x += m;
        return x;
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