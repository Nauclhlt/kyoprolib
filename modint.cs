/// <summary>
/// 自動でmodをとる整数。必要ないならあんまり使いすぎないように。
/// パフォーマンスはあまり良くないかもしれない。
/// </summary>
/// <typeparam name="T"></typeparam>
public readonly struct ModInt<T> : INumber<ModInt<T>>
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

    public static int Radix => 10;
    public static ModInt<T> MinValue => CreateFast(0L);
    public static ModInt<T> MaxValue => CreateFast(_mod.Mod - 1);

    /// <summary>
    /// 加法単位元つまり0を返す。計算量: O(1)
    /// </summary>
    public static ModInt<T> AdditiveIdentity { get; } = CreateFast(0L);
    public static ModInt<T> MultiplicativeIdentity { get; } = CreateFast(1L);

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

        return new(x);
    }

    [MethodImpl(256)]
    public static ModInt<T> operator +(ModInt<T> left, ModInt<T> right) => CreateFast((left.Value + right.Value) % _mod.Mod);

    [MethodImpl(256)]
    public static ModInt<T> operator +(ModInt<T> self) => self;

    [MethodImpl(256)]
    public static ModInt<T> operator -(ModInt<T> left, ModInt<T> right) => CreateFast((left.Value - right.Value + _mod.Mod) % _mod.Mod);

    [MethodImpl(256)]
    public static ModInt<T> operator -(ModInt<T> self) => CreateFast(_mod.Mod - self.Value);

    [MethodImpl(256)]
    public static ModInt<T> operator *(ModInt<T> left, ModInt<T> right) => CreateFast(left.Value * right.Value % _mod.Mod);

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

    public static ModInt<T> operator %(ModInt<T> left, ModInt<T> right)
    {
        throw new NotImplementedException();
    }

    public static bool operator <(ModInt<T> left, ModInt<T> right)
    {
        throw new NotImplementedException();
    }

    public static bool operator >(ModInt<T> left, ModInt<T> right)
    {
        throw new NotImplementedException();
    }

    public static bool operator <=(ModInt<T> left, ModInt<T> right)
    {
        throw new NotImplementedException();
    }

    public static bool operator >=(ModInt<T> left, ModInt<T> right)
    {
        throw new NotImplementedException();
    }

    [MethodImpl(256)]
    public static bool operator ==(ModInt<T> left, ModInt<T> right) => left.Value == right.Value;

    [MethodImpl(256)]
    public static bool operator !=(ModInt<T> left, ModInt<T> right) => !(left == right);

    [MethodImpl(256)]
    public static ModInt<T> operator ++(ModInt<T> self) => CreateFast((self.Value + 1) % _mod.Mod);

    [MethodImpl(256)]
    public static ModInt<T> operator --(ModInt<T> self) => CreateFast((self.Value - 1 + _mod.Mod) % _mod.Mod);

    [MethodImpl(256)]
    public bool Equals(ModInt<T> other) => Value == other.Value;

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
    public override int GetHashCode() => Value.GetHashCode();

    [MethodImpl(256)]
    public static implicit operator ModInt<T>(long v) => new ModInt<T>(v);

    [MethodImpl(256)]
    public static implicit operator ModInt<T>(int v) => new ModInt<T>(v);

    [MethodImpl(256)]
    public static implicit operator long(in ModInt<T> m) => m.Value;

    [MethodImpl(256)]
    public static implicit operator int(in ModInt<T> m) => (int)m.Value;

    public override string ToString() => Value.ToString();

    public string ToString(string format, IFormatProvider provider) => Value.ToString(format, provider);

    #region INumberBase<TSelf> Implementation

    public static ModInt<T> Abs(ModInt<T> value) => value;
    public static bool IsCanonical(ModInt<T> value) => true;
    public static bool IsComplexNumber(ModInt<T> value) => false;
    public static bool IsFinite(ModInt<T> value) => true;
    public static bool IsImaginaryNumber(ModInt<T> value) => false;
    public static bool IsInfinity(ModInt<T> value) => false;
    public static bool IsInteger(ModInt<T> value) => true;
    public static bool IsNaN(ModInt<T> value) => false;
    public static bool IsNegative(ModInt<T> value) => false;
    public static bool IsPositive(ModInt<T> value) => value.Value != 0L;
    public static bool IsRealNumber(ModInt<T> value) => true;
    public static bool IsZero(ModInt<T> value) => value.Value == 0L;
    public static bool IsEvenInteger(ModInt<T> value) => (value.Value & 1) == 0;
    public static bool IsOddInteger(ModInt<T> value) => (value.Value & 1) == 1;
    public static bool IsPositiveInfinity(ModInt<T> value) => false;
    public static bool IsNegativeInfinity(ModInt<T> value) => false;
    public static bool IsNormal(ModInt<T> value) => false;
    public static bool IsSubnormal(ModInt<T> value) => false;


    public static ModInt<T> MaxMagnitude(ModInt<T> x, ModInt<T> y)
    {
        if (x.Value > y.Value) return x;
        else return y;
    }
    public static ModInt<T> MaxMagnitudeNumber(ModInt<T> x, ModInt<T> y) => MaxMagnitude(x, y);
    public static ModInt<T> MinMagnitude(ModInt<T> x, ModInt<T> y)
    {
        if (x.Value < y.Value) return x;
        else return y;
    }
    public static ModInt<T> MinMagnitudeNumber(ModInt<T> x, ModInt<T> y) => MinMagnitude(x, y);

    public static ModInt<T> CreateChecked<TOther>(TOther value) where TOther : INumberBase<TOther>
        => new(long.CreateChecked(value));
    public static ModInt<T> CreateSaturating<TOther>(TOther value) where TOther : INumberBase<TOther>
        => new(long.CreateSaturating(value));
    public static ModInt<T> CreateTruncating<TOther>(TOther value) where TOther : INumberBase<TOther>
        => new(long.CreateTruncating(value));

    public static ModInt<T> Parse(string s, NumberStyles style, IFormatProvider provider) => Parse(s.AsSpan(), style, provider);

    public static ModInt<T> Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider provider) => new(long.Parse(s, style, provider));

    public static ModInt<T> Parse(ReadOnlySpan<byte> s, NumberStyles style, IFormatProvider provider) => new(long.Parse(s, style, provider));

    public static ModInt<T> Parse(string s, IFormatProvider provider) => new(long.Parse(s, provider));

    public static ModInt<T> Parse(ReadOnlySpan<char> s, IFormatProvider provider) => new(long.Parse(s, provider));

    public bool TryFormat(Span<byte> dest, out int bytesWritten, ReadOnlySpan<char> format, IFormatProvider provider)
    {
        return Value.TryFormat(dest, out bytesWritten, format, provider);
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider provider)
    {
        return Value.TryFormat(destination, out charsWritten, format, provider);
    }

    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider provider, out ModInt<T> result)
    {
        if (long.TryParse(s, style, provider, out long inner))
        {
            result = new(inner);
            return true;
        }
        else
        {
            result = Zero;
            return false;
        }

    }

    public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out ModInt<T> result)
    {
        if (long.TryParse(s, style, provider, out long inner))
        {
            result = new(inner);
            return true;
        }
        else
        {
            result = Zero;
            return false;
        }
    }



    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider provider, out ModInt<T> result)
    {
        if (long.TryParse(s, provider, out long inner))
        {
            result = new(inner);
            return true;
        }
        else
        {
            result = Zero;
            return false;
        }
    }

    public static bool TryParse(string s, IFormatProvider provider, out ModInt<T> result)
    {
        if (long.TryParse(s, provider, out long inner))
        {
            result = new(inner);
            return true;
        }
        else
        {
            result = Zero;
            return false;
        }
    }

    public static bool TryConvertFromChecked<TOther>(TOther value, out ModInt<T> result) where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertFromSaturating<TOther>(TOther value, out ModInt<T> result) where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertFromTruncating<TOther>(TOther value, [MaybeNullWhen(false)] out ModInt<T> result) where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertToChecked<TOther>(ModInt<T> value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertToSaturating<TOther>(ModInt<T> value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertToTruncating<TOther>(ModInt<T> value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }


    #endregion

    #region INumber<TSelf> Implementation

    public int CompareTo(ModInt<T> other) => Value.CompareTo(other.Value);
    public int CompareTo(object other) => Value.CompareTo(other);
    

    #endregion
}