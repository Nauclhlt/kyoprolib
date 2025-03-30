/// <summary>
/// FPSを扱う。定数倍はあんまり良くない。
/// Depends on: convolution
/// </summary>
public sealed class FPS<T> where T : struct, IMod
{
    private static readonly Convolution<T> _convolution = new();
    private ModInt<T>[] _coef;
    public ModInt<T>[] Coef => _coef;
    public int Length => _coef.Length;
    public int MaxExponent => _coef.Length - 1;

    public ModInt<T> this[int index]
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
        _coef = new ModInt<T>[sequence.Length];
        for (int i = 0; i < _coef.Length; i++)
        {
            _coef[i] = ModInt<T>.CreateFast(sequence[i]);
        }
    }

    public FPS(ModInt<T>[] sequence)
    {
        _coef = sequence;
    }

    public FPS(int length)
    {
        _coef = new ModInt<T>[length];
    }

    public static FPS<T> CopyCreate(ModInt<T>[] sequence)
    {
        ModInt<T>[] copy = new ModInt<T>[sequence.Length];
        Array.Copy(sequence, copy, sequence.Length);
        return new(copy);
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
    public FPS<T> GetResized(int length)
    {
        ModInt<T>[] res = new ModInt<T>[length];
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
    public static FPS<T> ResizedMultiply(FPS<T> a, int asize, FPS<T> b, int bsize)
    {
        ModInt<T>[] res = _convolution.CalcConvolution(a._coef.AsSpan(0, asize), b._coef.AsSpan(0, bsize));
        return new FPS<T>(res);
    }

    public static FPS<T> operator -(FPS<T> target)
    {
        ModInt<T>[] res = new ModInt<T>[target.Length];
        for (int i = 0; i < target.Length; i++) res[i] = -target[i];

        return new(res);
    }

    public static FPS<T> operator +(FPS<T> left, FPS<T> right)
    {
        int len = int.Max(left.Length, right.Length);
        ModInt<T>[] res = new ModInt<T>[len];
        for (int i = 0; i < left.Length; i++) res[i] += left[i];
        for (int i = 0; i < right.Length; i++) res[i] += right[i];

        return new (res);
    }

    public static FPS<T> operator -(FPS<T> left, FPS<T> right)
    {
        int len = int.Max(left.Length, right.Length);
        ModInt<T>[] res = new ModInt<T>[len];
        for (int i = 0; i < left.Length; i++) res[i] += left[i];
        for (int i = 0; i < right.Length; i++) res[i] -= right[i];

        return new (res);
    }

    public static FPS<T> operator *(FPS<T> left, FPS<T> right)
    {
        ModInt<T>[] res = _convolution.CalcConvolution(left.Coef, right.Coef);
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
    public FPS<T> Inv(int n)
    {
        Debug.Assert(_coef[0] != 0);
        FPS<T> g = new(new ModInt<T>[]{ _coef[0].Inv() });
        int k = 1;
        while (k < n)
        {
            k *= 2;
            FPS<T> fg = ResizedMultiply(this, int.Min(Length, k), g, g.Length);
            fg.Resize(k);
            for (int i = 0; i < fg.Length; i++) fg[i] = -fg[i];
            fg[0] += 2;
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
    public FPS<T> Exp(int n)
    {
        FPS<T> g = new(new long[] {1});
        int k = 1;
        while (k < n)
        {
            k <<= 1;
            FPS<T> logG = g.Log(k);
            for (int i = 0; i < k; i++) logG[i] = this[i] - logG[i];
            logG[0] += 1;
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
    public FPS<T> Diff()
    {
        FPS<T> result = new (Length);
        for (int i = 1; i < Length; i++)
        {
            result[i - 1] = _coef[i] * i;
        }
        return result;
    }

    /// <summary>
    /// 積分を求める。計算量: O(n)
    /// </summary>
    /// <returns></returns>
    public FPS<T> Integral()
    {
        FPS<T> result = new(Length + 1);
        for (int i = 1; i <= Length; i++)
        {
            result[i] = _coef[i - 1] / i;
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
            _coef[i] = _coef[i - 1] / i;
        }
        _coef[0] = 0;
    }

    /// <summary>
    /// logの先頭n項を求める。計算量: O(nlogn)
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public FPS<T> Log(int n)
    {
        if (_coef[0] != 1)
        {
            throw new InvalidOperationException("Cannot define log(f(x)) for f(x) such that [x^0]f(x) != 1.");
        }

        FPS<T> df = Diff();
        FPS<T> inv = Inv(n);

        //FPS log = df * inv;
        FPS<T> log = ResizedMultiply(df, int.Min(df.Length, n), inv, inv.Length);
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
    public FPS<T> Power(long exponent, int n)
    {
        FPS<T> log = Log(n);
        for (int i = 0; i < log.Length; i++)
        {
            log[i] *= exponent;
        }
        return log.Exp(n);
    }
}