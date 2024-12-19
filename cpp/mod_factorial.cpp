class ModFactorialCache
{
private:
    vector<ModInt> _factorial;
    vector<ModInt> _inverseFactorial;

public:
    ModFactorialCache(int max)
    {
        _factorial.resize(max + 1);
        _inverseFactorial.resize(max + 1);

        _factorial[0] = 1;
        _inverseFactorial[0] = (ModInt(1LL)).Inv();

        for (long long p = 1; p <= max; p++)
        {
            ModInt pm = ModInt(p);
            _factorial[p] = _factorial[p - 1] * pm;
            _inverseFactorial[p] = _inverseFactorial[p - 1] * pm.Inv();
        }
    }

    ModInt Combination(int n, int r)
    {
        return _factorial[n] * (_inverseFactorial[n - r] * _inverseFactorial[r]);
    }

    ModInt Permutation(int n, int r)
    {
        return _factorial[n] * _inverseFactorial[n - r];
    }

    ModInt Factorial(int n)
    {
        return _factorial[n];
    }
};