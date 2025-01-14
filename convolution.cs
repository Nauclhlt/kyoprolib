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

    private static long CalcPow(long b, long exp, long mod)
    {
        if (exp == 0) return 1;
        if (exp == 1) return b % mod;

        long m = CalcPow(b, exp / 2L, mod);
        m *= m;
		m %= mod;
        if (exp % 2L == 1) m *= b % mod;
        m %= mod;

        return m;
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

    private void NTT(long[] target, int size, int exp, long[] root)
    {
        if (size == 1)
        {
            return;
        }
        else
        {
            int half = size >> 1;
            
            long[] odd = new long[half];
            long[] even = new long[half];

            for (int i = 0; i < size; i++)
            {
                if ((i & 1) == 0)
                {
                    even[i >> 1] = target[i];
                }
                else
                {
                    odd[(i - 1) >> 1] = target[i];
                }
            }

            NTT(even, half, exp - 1, root);
            NTT(odd, half, exp - 1, root);

            long r = root[exp];

            long f = 1L;

            for (int i = 0; i < size; i++)
            {
                target[i] = (even[i % half] + (f * odd[i % half]) % _mod) % _mod;
                f *= r;
                f %= _mod;
            }
        }
    }

    public long[] CalcConvolution(long[] a, long[] b)
    {
        int dsize = a.Length + b.Length;

        int exp = 0;
        while ((1 << exp) <= dsize)
        {
            exp++;
        }
        int n = 1 << exp;

        if (exp > _maxExp)
        {
            throw new InvalidOperationException("Data too large.");
        }

        long[] resizedA = new long[n];
        long[] resizedB = new long[n];
        Array.Copy(a, 0, resizedA, 0, a.Length);
        Array.Copy(b, 0, resizedB, 0, b.Length);

        NTT(resizedA, n, exp, _root);
        NTT(resizedB, n, exp, _root);

        long[] c = new long[n];
        for (int i = 0; i < n; i++)
        {
            c[i] = (resizedA[i] * resizedB[i]) % _mod;
        }

        NTT(c, n, exp, _inverseRoot);
        long invN = Inverse(n, _mod);
        for (int i = 0; i < n; i++)
        {
            c[i] = (c[i] * invN) % _mod;
        }

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

    // m1, m2はNTT-Friendlyで大きめの素数
    public static long[] DualModConvolutionCRT(long[] a, long[] b, long m1, long m2)
    {
        Convolution c1 = new(m1);
        Convolution c2 = new(m2);
        long[] res1 = c1.CalcConvolution(a, b);
        long[] res2 = c2.CalcConvolution(a, b);
        long[] restored = new long[res1.Length];
        for (int i = 0; i < res1.Length; i++)
        {
            restored[i] = CRT(res1[i], m1, res2[i], m2).rem;
        }

        return restored;
    }
}