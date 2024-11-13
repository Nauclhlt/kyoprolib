// 実行時に法を設定可能なmodint. 乗法の逆元が存在する必要がある.
// @author Nauclhlt.
public readonly struct DynamicModInt
{
    private static long Mod = 998244353;

    private readonly long Value;

    public static readonly DynamicModInt Empty = new DynamicModInt(0L);

    public DynamicModInt(long value)
    {
        Value = SafeMod(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long SafeMod(long a)
    {
        a %= Mod;
        if (a < 0) a += Mod;
        return a;
    }

    public DynamicModInt Power(long exp)
    {
        if (exp <= -1) return this;
        if (exp == 0) return 1;
        if (exp == 1) return this;

        DynamicModInt m = Power(exp / 2);
        m *= m;
        if (exp % 2 == 1) m *= this;

        return m;
    }

    public DynamicModInt Inv()
    {
        long a = Value;

        long b = Mod, u = 1, v = 0;
		while ( b > 0 )
		{
			long t = a / b;
			a -= t * b;
            (a, b) = (b, a);
			u -= t * v;
            (u, v) = (v, u);
		}
        
        SafeMod(u);
		return u;
    }

    public static DynamicModInt operator +(DynamicModInt left, DynamicModInt right)
    {
        return new DynamicModInt(SafeMod(left.Value + right.Value));
    }

    public static DynamicModInt operator -(DynamicModInt left, DynamicModInt right)
    {
        return new DynamicModInt(SafeMod(left.Value - right.Value));
    }

    public static DynamicModInt operator *(DynamicModInt left, DynamicModInt right)
    {
        return new DynamicModInt(SafeMod(left.Value * right.Value));
    }

    public static DynamicModInt operator /(DynamicModInt left, DynamicModInt right)
    {
        if (right.Value == 0L)
        {
            return Empty;
        }

        DynamicModInt inv = right.Inv();
        return SafeMod(left * inv);
    }

    public static DynamicModInt operator %(DynamicModInt left, DynamicModInt right)
    {
        if (right.Value == 0L)
        {
            return Empty;
        }

        return new DynamicModInt(SafeMod(left.Value % right.Value));
    }

    public static implicit operator DynamicModInt(long v)
    {
        return new DynamicModInt(v);
    }

    public static implicit operator DynamicModInt(int v)
    {
        return new DynamicModInt(v);
    }

    public static implicit operator long(DynamicModInt m)
    {
        return m.Value;
    }

    public static implicit operator int(DynamicModInt m)
    {
        return (int)m.Value;
    }

    public static DynamicModInt Combination(long n, long r)
    {
        DynamicModInt c = 1;
        for (DynamicModInt i = 1; i <= r; i++)
        {
            c = c * (n - i + 1) / i;
        }
        return c;
    }

    public long Raw() => Value;

    public static void SetMod(long mod)
    {
        Mod = mod;
    }
}