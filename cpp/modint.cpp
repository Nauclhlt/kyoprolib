#define CONST_MOD 998244353L
// #define CONST_MOD 1000000007L
struct ModInt
{
    long long Value;

public:
    ModInt()
    {
        Value = 0L;
    }

    ModInt(long long value)
    {
        Value = value;
    }

    ModInt Power(long long exp) const
    {
        if (exp <= -1L)
        {
            return ModInt(1L) / Power(-exp);
        }
        if (exp == 0L)
            return 1L;
        if (exp == 1L)
            return *this;

        ModInt m = Power(exp / 2L);
        m = m * m;
        if (exp % 2L == 1L)
        {
            m = m * (*this);
        }

        return m;
    }

    ModInt Inv() const
    {
        return this->Power(CONST_MOD - 2L);
    }

    ModInt operator+() const
    {
        return *this;
    }

    ModInt operator-() const
    {
        return ModInt(-Value);
    }

    friend ModInt operator+(const ModInt& left, const ModInt& right)
    {
        return ModInt(SafeMod(left.Value + right.Value));
    }

    friend ModInt operator+(const ModInt& left, const long long& right)
    {
        return ModInt(SafeMod(left.Value + right));
    }

    friend ModInt operator+(const long long& left, const ModInt& right)
    {
        return ModInt(SafeMod(left + right.Value));
    }

    ModInt& operator+=(const ModInt& x)
    {
        Value += x.Value;
        Value = SafeMod(Value);

        return *this;
    }

    ModInt& operator+=(const long long& x)
    {
        Value += x;
        Value = SafeMod(Value);

        return *this;
    }

    friend ModInt operator-(const ModInt& left, const ModInt& right)
    {
        return ModInt(SafeMod(left.Value - right.Value));
    }

    friend ModInt operator-(const ModInt& left, const long long& right)
    {
        return ModInt(SafeMod(left.Value - right));
    }

    friend ModInt operator-(const long long& left, const ModInt& right)
    {
        return ModInt(SafeMod(left - right.Value));
    }

    ModInt& operator-=(const ModInt& x)
    {
        Value -= x.Value;
        Value = SafeMod(Value);

        return *this;
    }

    ModInt& operator-=(const long long& x)
    {
        Value -= x;
        Value = SafeMod(Value);

        return *this;
    }

    friend ModInt operator*(const ModInt& left, const ModInt& right)
    {
        return ModInt(SafeMod(left.Value * right.Value));
    }

    friend ModInt operator*(const ModInt& left, const long long& right)
    {
        return ModInt(SafeMod(left.Value * right));
    }

    friend ModInt operator*(const long long& left, const ModInt& right)
    {
        return ModInt(SafeMod(left * right.Value));
    }

    ModInt& operator*=(const ModInt& x)
    {
        Value *= x.Value;
        Value = SafeMod(Value);

        return *this;
    }

    ModInt& operator*=(const long long& x)
    {
        Value *= x;
        Value = SafeMod(Value);

        return *this;
    }

    friend ModInt operator /(const ModInt& left, const ModInt& right)
    {
        ModInt inv = right.Inv();
        return ModInt(SafeMod(left.Value * inv.Value));
    }

    friend ModInt operator/(const ModInt& left, const long long& right)
    {
        return ModInt(SafeMod(left.Value * ModInt(right).Inv().Value));
    }

    friend ModInt operator/(const long long& left, const ModInt& right)
    {
        return ModInt(SafeMod(left * right.Inv().Value));
    }

    ModInt& operator/=(const ModInt& x)
    {
        Value *= x.Inv().Value;
        Value = SafeMod(Value);

        return *this;
    }

    ModInt& operator/=(const long long& x)
    {
        Value *= ModInt(x).Inv().Value;
        Value = SafeMod(Value);

        return *this;
    }

    ModInt& operator++()
    {
        ++Value;
        Value = SafeMod(Value);
        return *this;
    }

    ModInt operator++(int)
    {
        ModInt temp = *this;
        Value++;
        Value = SafeMod(Value);
        return temp;
    }

    ModInt& operator--()
    {
        --Value;
        Value = SafeMod(Value);
        return *this;
    }

    ModInt operator--(int)
    {
        ModInt temp = *this;
        Value--;
        Value = SafeMod(Value);
        return temp;
    }

    inline static ModInt One()
    {
        return ModInt(1L);
    }

    static ModInt Combination(long long n, long long r)
    {
        ModInt c = 1L;
        for (ModInt i = 1; i.Value <= r; i++)
        {
            c = c * (ModInt(n) - i + ModInt::One()) / i;
        }
        return c;
    }

private:
    inline static long long SafeMod(long long a)
    {
        a %= CONST_MOD;
        if (a < 0)
        {
            a += CONST_MOD;
        }
        return a;
    }
};