// modintの階乗とその逆元を前計算して高速化.
// 前計算O(最大値). 階乗, 順列, 二項係数それぞれ定数時間.
// Depends on: ModInt
// @author Nauclhlt.
public sealed class ModFactorialCache
{
    private ModInt[] _factorial;
    private ModInt[] _inverseFactorial;

    // 階乗とその逆元を前計算する.
    // O(max)
    public ModFactorialCache(long max)
    {
        _factorial = new ModInt[max + 1];
        _inverseFactorial = new ModInt[max + 1];

        _factorial[0] = 1;
        _inverseFactorial[0] = ((ModInt)1).Inv();

        for (long p = 1; p <= max; p++)
        {
            _factorial[p] = _factorial[p - 1] * p;
            _inverseFactorial[p] = _inverseFactorial[p - 1] * ((ModInt)p).Inv();
        }
    }

    // 二項係数nCrを計算する.
    // O(1)
    public ModInt Combination(long n, long r)
    {
        return _factorial[n] * (_inverseFactorial[n - r] * _inverseFactorial[r]);
    }

    // 順列の個数nPrを計算する.
    // O(1)
    public ModInt Permutation(long n, long r)
    {
        return _factorial[n] * _inverseFactorial[n - r];
    }

    // n!を計算する.
    // O(1)
    public ModInt Factorial(long n)
    {
        return _factorial[n];
    }
}