/// <summary>
/// ModIntの階乗、階乗の逆元、逆元を前計算。
/// </summary>
public sealed class ModCache
{
    private ModInt[] _factorial;
    private ModInt[] _inverseFactorial;
    private ModInt[] _inverse;

    // 階乗(&逆元)と逆元を前計算する.
    // O(max)
    public ModCache(long max)
    {
        _factorial = new ModInt[max + 1];
        _inverseFactorial = new ModInt[max + 1];

        _factorial[0] = 1;
        _inverseFactorial[0] = ((ModInt)1).Inv();

        _inverse = new ModInt[max + 1];
        _inverse[1] = 1;

        for (long p = 1; p <= max; p++)
        {
            _factorial[p] = _factorial[p - 1] * p;
            if (p > 1)
            {
                _inverse[p] = -(Constants.Mod / p) * _inverse[Constants.Mod % p];
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
    public ModInt Combination(long n, long r)
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
    public ModInt Permutation(long n, long r)
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
    public ModInt Factorial(long n)
    {
        if (n < 0 || n >= _factorial.Length)
        {
            throw new InvalidOperationException("Invalid N");
        }
        return _factorial[n];
    }

    /// <summary>
    /// n!の乗法逆元(n!)^{-1}の値を返す。計算量: O(1)
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public ModInt InverseFactorial(int n)
    {
        if (n < 0 || n >= _factorial.Length)
        {
            throw new InvalidOperationException("Invalid N");
        }
        return _inverseFactorial[n];
    }

    /// <summary>
    /// nの乗法逆元n^{-1}を返す。計算量: O(1)
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public ModInt Inverse(long n)
    {
        if (n == 0 || n >= _inverse.Length)
        {
            throw new InvalidOperationException("Invalid N");
        }
        return _inverse[n];
    }
}