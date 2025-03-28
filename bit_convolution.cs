public static class BitConvolution
{
    public static T[] AndConvolution<T>(int n, T[] a, T[] b) where T : struct, IAdditionOperators<T, T, T>, IAdditiveIdentity<T, T>, ISubtractionOperators<T, T, T>, IMultiplyOperators<T, T, T>
    {
        System.Diagnostics.Contracts.Contract.Assert((1 << n) == a.Length || (1 << n) == b.Length);

        T[] c = new T[1 << n];
        Array.Copy(a, c, a.Length);
        T[] d = new T[1 << n];
        Array.Copy(b, d, b.Length);

        InplaceSuperZetaTransform(n, c);
        InplaceSuperZetaTransform(n, d);
        
        for (int i = 0; i < (1 << n); i++)
        {
            c[i] *= d[i];
        }

        InplaceSuperMobiusTransform(n, c);

        return c;
    }

    public static T[] OrConvolution<T>(int n, T[] a, T[] b) where T : struct, IAdditionOperators<T, T, T>, IAdditiveIdentity<T, T>, ISubtractionOperators<T, T, T>, IMultiplyOperators<T, T, T>
    {
        System.Diagnostics.Contracts.Contract.Assert((1 << n) == a.Length || (1 << n) == b.Length);

        T[] c = new T[1 << n];
        Array.Copy(a, c, a.Length);
        T[] d = new T[1 << n];
        Array.Copy(b, d, b.Length);

        InplaceSubsetZetaTransform(n, c);
        InplaceSubsetZetaTransform(n, d);
        
        for (int i = 0; i < (1 << n); i++)
        {
            c[i] *= d[i];
        }

        InplaceSubsetMobiusTransform(n, c);

        return c;
    }

    public static T[] XorConvolution<T>(int n, T[] a, T[] b) where T : struct, INumber<T>
    {
        System.Diagnostics.Contracts.Contract.Assert((1 << n) == a.Length || (1 << n) == b.Length);

        T[] c = new T[1 << n];
        Array.Copy(a, c, a.Length);
        T[] d = new T[1 << n];
        Array.Copy(b, d, b.Length);

        InplaceHadamardTransform(n, c);
        InplaceHadamardTransform(n, d);
        
        for (int i = 0; i < (1 << n); i++)
        {
            c[i] *= d[i];
        }

        InplaceInverseHadamardTransform(n, c);

        return c;
    }

    public static T[] SubsetConvolution<T>(int n, T[] a, T[] b) where T : struct, INumber<T>
    {
        System.Diagnostics.Contracts.Contract.Assert((1 << n) == a.Length || (1 << n) == b.Length);

        int length = 1 << n;

        T[,] c = new T[n + 1, length];
        T[,] d = new T[n + 1, length];

        for (int i = 0; i < length; i++)
        {
            int size = BitOperations.PopCount((uint)i);
            c[size, i] = a[i];
            d[size, i] = b[i];
        }

        for (int k = 0; k <= n; k++)
        {
            for (int bit = 0; bit < n; bit++)
            {
                for (int i = 0; i < length; i++)
                {
                    if ((i & (1 << bit)) != 0)
                    {
                        c[k, i] += c[k, i ^ (1 << bit)];
                        d[k, i] += d[k, i ^ (1 << bit)];
                    }
                }
            }
        }

        T[,] r = new T[n + 1, length];
        for (int s = 0; s < length; s++)
        {
            for (int i = 0; i <= n; i++)
            {
                for (int j = 0; j <= n - i; j++)
                {
                    r[i + j, s] += c[i, s] * d[j, s];
                }
            }
        }

        for (int k = 0; k <= n; k++)
        {
            for (int bit = 0; bit < n; bit++)
            {
                for (int i = 0; i < length; i++)
                {
                    if ((i & (1 << bit)) != 0)
                    {
                        r[k, i] -= r[k, i ^ (1 << bit)];
                    }
                }
            }
        }

        T[] res = new T[length];
        for (int i = 0; i < length; i++)
        {
            int size = BitOperations.PopCount((uint)i);
            res[i] = r[size, i];
        }

        return res;
    }

    private static void InplaceSuperZetaTransform<T>(int n, T[] a) where T : struct, IAdditionOperators<T, T, T>, IAdditiveIdentity<T, T>, ISubtractionOperators<T, T, T>, IMultiplyOperators<T, T, T>
    {
        int length = 1 << n;
        for (int i = 0; (1 << i) < length; i++)
        {
            for (int j = 0; j < length; j++)
            {
                if ((j & (1 << i)) == 0)
                {
                    a[j] += a[j | (1 << i)];
                }
            }
        }
    }

    private static void InplaceSubsetZetaTransform<T>(int n, T[] a) where T : struct, IAdditionOperators<T, T, T>, IAdditiveIdentity<T, T>, ISubtractionOperators<T, T, T>, IMultiplyOperators<T, T, T>
    {
        int length = 1 << n;
        for (int i = 0; (1 << i) < length; i++)
        {
            for (int j = 0; j < length; j++)
            {
                if ((j & (1 << i)) != 0)
                {
                    a[j] += a[j ^ (1 << i)];
                }
            }
        }
    }

    private static void InplaceSuperMobiusTransform<T>(int n, T[] a) where T : struct, IAdditionOperators<T, T, T>, IAdditiveIdentity<T, T>, ISubtractionOperators<T, T, T>, IMultiplyOperators<T, T, T>
    {
        int length = 1 << n;
        for (int i = 0; (1 << i) < length; i++)
        {
            for (int j = 0; j < length; j++)
            {
                if ((j & (1 << i)) == 0)
                {
                    a[j] -= a[j | (1 << i)];
                }
            }
        }
    }

    private static void InplaceSubsetMobiusTransform<T>(int n, T[] a) where T : struct, IAdditionOperators<T, T, T>, IAdditiveIdentity<T, T>, ISubtractionOperators<T, T, T>, IMultiplyOperators<T, T, T>
    {
        int length = 1 << n;
        for (int i = 0; (1 << i) < length; i++)
        {
            for (int j = 0; j < length; j++)
            {
                if ((j & (1 << i)) != 0)
                {
                    a[j] -= a[j ^ (1 << i)];
                }
            }
        }
    }

    private static void InplaceHadamardTransform<T>(int n, T[] a) where T : struct, INumber<T>
    {
        int length = 1 << n;
        for (int l = 1; l < length; l <<= 1)
        {
            for (int i = 0; i < length; i += (l << 1))
            {
                for (int j = 0; j < l; j++)
                {
                    T u = a[i + j];
                    T v = a[i + j + l];
                    a[i + j] = u + v;
                    a[i + j + l] = u - v;
                }
            }
        }
    }

    private static void InplaceInverseHadamardTransform<T>(int n, T[] a) where T : struct, INumber<T>
    {
        InplaceHadamardTransform(n, a);
        int length = 1 << n;
        for (int i = 0; i < a.Length; i++)
        {
            a[i] /= T.CreateChecked(length);
        }
    }
}