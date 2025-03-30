/// <summary>
/// 自動でmodをとる整数。必要ないならあんまり使いすぎないように。
/// パフォーマンスはあまり良くないかもしれない。
/// </summary>
/// <typeparam name="T"></typeparam>
public readonly struct ModInt<T> :  IEquatable<ModInt<T>>, IAdditionOperators<ModInt<T>, ModInt<T>, ModInt<T>>, 
                                    ISubtractionOperators<ModInt<T>, ModInt<T>, ModInt<T>>, 
                                    IAdditiveIdentity<ModInt<T>, ModInt<T>>, IMultiplyOperators<ModInt<T>, ModInt<T>, ModInt<T>>, 
                                    IDivisionOperators<ModInt<T>, ModInt<T>, ModInt<T>>
                                    where T : struct, IMod
{
    public static long Mod => _mod.Mod;

    private static readonly T _mod = default;

    public readonly long Value;

    /// <summary>
    /// 1を返す。計算量: O(1)
    /// </summary>
    public static ModInt<T> One { get; } = CreateFast(1L);

    /// <summary>
    /// 0を返す。計算量: O(1)
    /// </summary>
    public static ModInt<T> Zero { get; } = CreateFast(0L);

    /// <summary>
    /// 加法単位元つまり0を返す。計算量: O(1)
    /// </summary>
    public static ModInt<T> AdditiveIdentity { get; } = CreateFast(0L);

    public ModInt(long value)
    {
        Value = SafeMod(value);
    }

    private ModInt(long value, bool dummy)
    {
        Value = value;
    }

    /// <summary>
    /// modintを構築する。0 <= value < MODのとき限定。速いはず。計算量: O(1)
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ModInt<T> CreateFast(long value)
    {
        return new ModInt<T>(value, false); 
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long SafeMod(long a)
    {
        return a % _mod.Mod + ((a >> 63) & _mod.Mod);
    }

    /// <summary>
    /// exp乗を計算する。計算量: O(log(exp))
    /// </summary>
    /// <param name="exp"></param>
    /// <returns></returns>
    public readonly ModInt<T> Power(long exp)
    {
        if (exp < 0)
        {
            return Power(-exp).Inv();
        }
        else
        {
            long res = 1L;
            long b = Value;
            while (exp > 0)
            {
                if ((exp & 1) == 1) res = res * b % _mod.Mod;
                b = b * b % _mod.Mod;
                exp >>= 1;
            }
            
            return CreateFast(res);
        }
    }

    /// <summary>
    /// 乗法逆元を返す。0は渡さない。計算量: O(logN)
    /// </summary>
    /// <returns></returns>
    public readonly ModInt<T> Inv()
    {
        long x = 1, y = 0;
        long x1 = 0, y1 = 1;
        long b = _mod.Mod;
        long a = Value;

        while (b != 0)
        {
            long q = a / b;
            long t = a % b;
            a = b;
            b = t;

            long tx = x - q * x1;
            long ty = y - q * y1;
            x = x1;
            y = y1;
            x1 = tx;
            y1 = ty;
        }

        return new (x);
    }

    [MethodImpl(256)]
    public static ModInt<T> operator +(ModInt<T> left, ModInt<T> right)
    {
        return CreateFast((left.Value + right.Value) % _mod.Mod);
    }

    [MethodImpl(256)]
    public static ModInt<T> operator -(ModInt<T> left, ModInt<T> right)
    {
        return CreateFast((left.Value - right.Value + _mod.Mod) % _mod.Mod);
    }

    [MethodImpl(256)]
    public static ModInt<T> operator *(ModInt<T> left, ModInt<T> right)
    {
        return CreateFast(left.Value * right.Value % _mod.Mod);
    }

    [MethodImpl(256)]
    public static ModInt<T> operator /(ModInt<T> left, ModInt<T> right)
    {
        if (right.Value == 0L)
        {
            throw new DivideByZeroException();
        }

        ModInt<T> inv = right.Inv();
        return CreateFast(left.Value * inv.Value % _mod.Mod);
    }

    [MethodImpl(256)]
    public static bool operator ==(in ModInt<T> left, in ModInt<T> right)
    {
        return left.Value == right.Value;
    }

    [MethodImpl(256)]
    public static bool operator != (in ModInt<T> left, in ModInt<T> right)
    {
        return !(left == right);
    }

    [MethodImpl(256)]
    public bool Equals(ModInt<T> other)
    {
        return Value == other.Value;
    }

    [MethodImpl(256)]
    public override bool Equals(object other)
    {
        if (other is ModInt<T> m)
        {
            return this == m;
        }
        else return false;
    }

    [MethodImpl(256)]
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    [MethodImpl(256)]
    public static implicit operator ModInt<T>(long v)
    {
        return new ModInt<T>(v);
    }

    [MethodImpl(256)]
    public static implicit operator ModInt<T>(int v)
    {
        return new ModInt<T>(v);
    }

    [MethodImpl(256)]
    public static implicit operator long(in ModInt<T> m)
    {
        return m.Value;
    }

    [MethodImpl(256)]
    public static implicit operator int(in ModInt<T> m)
    {
        return (int)m.Value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}

/// <summary>
/// 法を指定するやつ。
/// </summary>
public interface IMod
{
    public long Mod { get; }
}

public readonly struct Mod998244353 : IMod { public long Mod => 998244353L; }
public readonly struct Mod1000000007 : IMod { public long Mod => 1000000007L; }
public readonly struct Mod897581057 : IMod { public long Mod => 897581057; }
public readonly struct Mod880803841 : IMod { public long Mod => 880803841; }