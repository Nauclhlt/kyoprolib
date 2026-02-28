class ModCache
{
private:
    vector<ModInt> _factorial;
    vector<ModInt> _inverseFactorial;
    vector<ModInt> _inverse;

public:
    ModCache(int max)
    {
        _factorial.resize(max + 1);
        _inverseFactorial.resize(max + 1);
        _inverse.resize(max + 1);

        _factorial[0] = 1;
        _inverseFactorial[0] = 1LL;
        _inverse[1] = 1LL;

        for (long p = 1; p <= max; p++)
        {
            _factorial[p] = _factorial[p - 1] * p;
            if (p > 1)
            {
                _inverse[p] = -(CONST_MOD / p) * _inverse[CONST_MOD % p];
            }
            _inverseFactorial[p] = _inverseFactorial[p - 1] * _inverse[p];
        }
    }

    ModInt Combination(int n, int r)
    {
        if (n < 0 || r < 0 || n < r) return 0;
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

    ModInt Inverse(int n)
    {
        return _inverse[n];
    }
};