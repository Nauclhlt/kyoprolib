public static class Number
{
    public static long SafeMod(long x, long m)
    {
        x %= m;
        if (x < 0) x += m;
        return x;
    }

    public static long ExtEuclid(long a, long b, ref long p, ref long q)
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

    public static (long rem, long mod) CRT(long x1, long m1, long x2, long m2)
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

    public static (long rem, long mod) CRT(List<long> x, List<long> mod)
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