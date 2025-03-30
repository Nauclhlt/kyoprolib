/// <summary>
/// ModIntの階乗、階乗の逆元、逆元を前計算。
/// </summary>
public sealed class ModCache<T> where T : struct, IMod
{
    private ModInt<T>[] _factorial;
    private ModInt<T>[] _inverseFactorial;
    private ModInt<T>[] _inverse;

    // 階乗(&階乗逆元)と逆元を前計算する.
    // O(max)
    public ModCache(long max)
    {
        _factorial = new ModInt<T>[max + 1];
        _inverseFactorial = new ModInt<T>[max + 1];

        _factorial[0] = 1;
        _inverseFactorial[0] = ModInt<T>.One;

        _inverse = new ModInt<T>[max + 1];
        _inverse[1] = ModInt<T>.CreateFast(1);

        for (long p = 1; p <= max; p++)
        {
            _factorial[p] = _factorial[p - 1] * p;
            if (p > 1)
            {
                _inverse[p] = -(ModInt<T>.Mod / p) * _inverse[ModInt<T>.Mod % p];
            }
            _inverseFactorial[p] = _inverseFactorial[p - 1] * _inverse[p];
        }
    }

    /// <summary>
    /// binom(n, r)を返す。r<0またはr>nのとき0を返す。計算量: O(1)
    /// </summary>
    /// <param name="n"></param>
    /// <param name="r"></param>
    /// <returns></returns>
    public ModInt<T> Combination(long n, long r)
    {
        if (r < 0 || r > n) return 0;
        return _factorial[n] * (_inverseFactorial[n - r] * _inverseFactorial[r]);
    }

    /// <summary>
    /// nPrを返す。r<0またはr>nのとき0を返す。計算量: O(1)
    /// </summary>
    /// <param name="n"></param>
    /// <param name="r"></param>
    /// <returns></returns>
    public ModInt<T> Permutation(long n, long r)
    {
        if (r < 0 || r > n) return 1;
        return _factorial[n] * _inverseFactorial[n - r];
    }

    /// <summary>
    /// n!の値を返す。計算量: O(1)
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public ModInt<T> Factorial(long n)
    {
        Debug.Assert(0 <= n && n < _factorial.Length);
        return _factorial[n];
    }

    /// <summary>
    /// n!の乗法逆元(n!)^{-1}の値を返す。計算量: O(1)
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public ModInt<T> InverseFactorial(int n)
    {
        Debug.Assert(0 <= n && n < _inverseFactorial.Length);
        return _inverseFactorial[n];
    }

    /// <summary>
    /// nの乗法逆元n^{-1}を返す。計算量: O(1)
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public ModInt<T> Inverse(long n)
    {
        Debug.Assert(0 <= n && n < _inverse.Length);
        return _inverse[n];
    }
}