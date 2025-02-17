public sealed class Eratosthenes
{
    private bool[] _isPrime;
    private int[] _minFactor;
    private int[] _mobius;
    private int _n;

    public Eratosthenes(int max)
    {
        _n = max;

        _isPrime = new bool[max + 1];
        _minFactor = new int[max + 1];
        _mobius = new int[max + 1];

        Array.Fill(_isPrime, true);
        Array.Fill(_minFactor, -1);
        Array.Fill(_mobius, 1);

        _isPrime[1] = false;
        _minFactor[1] = 1;

        for (int i = 2; i <= _n; i++)
        {
            if (!_isPrime[i]) continue;

            _minFactor[i] = i;
            _mobius[i] = -1;

            for (int j = i + i; j <= _n; j += i)
            {
                _isPrime[j] = false;

                if (_minFactor[j] == -1) _minFactor[j] = i;
                if (j / i % i == 0) _mobius[j] = 0;
                else _mobius[j] = -_mobius[j];
            }
        }
    }

    public int Mobius(int n)
    {
        if (n > _n) throw new InvalidOperationException();
        return _mobius[n];
    }

    public T MobiusInverse<T>(int n, T[] f, Func<int, T> itot) where T : IAdditiveIdentity<T, T>, IAdditionOperators<T, T, T>, IMultiplyOperators<T, T, T>
    {
        if (n > _n) throw new InvalidOperationException();
        List<int> divs = GetDivisors(n);
        T res = T.AdditiveIdentity;
        for (int i = 0; i < divs.Count; i++)
        {
            res += itot(Mobius(divs[i])) * f[n / divs[i]];
        }

        return res;
    }

    public int MinFactor(int n)
    {
        if (n > _n) throw new InvalidOperationException();
        return _minFactor[n];
    }

    public List<int> GetDivisors(int n)
    {
        if (n > _n) throw new InvalidOperationException();
        List<int> divs = new();
        var factors = PrimeFactorize(n);

        divs.Add(1);
        
        for (int i = 0; i < factors.Count; i++)
        {
            int len = divs.Count;
            for (int j = 0; j < len; j++)
            {
                int f = factors[i].Item1;
                for (int k = 0; k < factors[i].Item2; k++)
                {
                    divs.Add(divs[j] * f);
                    f *= factors[i].Item1;
                }
            }
        }

        return divs;
    }

    public List<(int, int)> PrimeFactorize(int n)
    {
        if (n > _n) throw new InvalidOperationException();
        List<(int, int)> result = new();
        while (_minFactor[n] != 1)
        {
            int p = _minFactor[n];
            int c = 0;
            while (n % p == 0)
            {
                n /= p;
                c++;
            }

            result.Add((p, c));
        }

        return result;
    }

    public bool IsPrime(int n)
    {
        if (n > _n) throw new InvalidOperationException();
        return _isPrime[n];
    }
}