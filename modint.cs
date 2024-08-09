// 静的な底のmodint.
// Constantsクラス内にある静的な定数Modを参照する.
// @author Nauclhlt.
public readonly struct ModInt
{
    private readonly long Value;

    public static readonly ModInt Empty = new ModInt(0L);

    public ModInt(long value)
    {
        Value = SafeMod(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long SafeMod(long a)
    {
        a %= Constants.Mod;
        if (a < 0) a += Constants.Mod;
        return a;
    }

    public ModInt Power(long exp)
    {
        if (exp <= -1) return this;
        if (exp == 0) return 1;
        if (exp == 1) return this;

        ModInt m = Power(exp / 2);
        m *= m;
        if (exp % 2 == 1) m *= this;

        return m;
    }

    public ModInt Inv()
    {
        return this.Power(Constants.Mod - 2L);
    }

    public static ModInt operator +(ModInt left, ModInt right)
    {
        return new ModInt(SafeMod(left.Value + right.Value));
    }

    public static ModInt operator -(ModInt left, ModInt right)
    {
        return new ModInt(SafeMod(left.Value - right.Value));
    }

    public static ModInt operator *(ModInt left, ModInt right)
    {
        return new ModInt(SafeMod(left.Value * right.Value));
    }

    public static ModInt operator /(ModInt left, ModInt right)
    {
        if (right.Value == 0L)
        {
            return Empty;
        }

        ModInt inv = right.Inv();
        return SafeMod(left * inv);
    }

    public static ModInt operator %(ModInt left, ModInt right)
    {
        if (right.Value == 0L)
        {
            return Empty;
        }

        return new ModInt(SafeMod(left.Value % right.Value));
    }

    public static implicit operator ModInt(long v)
    {
        return new ModInt(v);
    }

    public static implicit operator ModInt(int v)
    {
        return new ModInt(v);
    }

    public static implicit operator long(ModInt m)
    {
        return m.Value;
    }

    public static implicit operator int(ModInt m)
    {
        return (int)m.Value;
    }

    public static ModInt Combination(long n, long r)
    {
        ModInt c = 1;
        for (ModInt i = 1; i <= r; i++)
        {
            c = c * (n - i + 1) / i;
        }
        return c;
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public long Raw() => Value;
}