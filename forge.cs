public static class Forge
{
    public static long FractionFloorSum(long n)
    {
        if (n <= 0) throw new InvalidOperationException();

        long result = 0L;

        long root = (long)Math.Sqrt(n);

        for (long i = 1L; i <= root; i++)
        {
            result += (n / i - n / (i + 1)) * i;
        }

        for (long i = 1L; i <= (n / (root + 1)); i++)
        {
            result += n / i;
        }

        return result;
    }

    public static string ReverseString(string s)
    {
        char[] chars = s.ToCharArray();
        Array.Reverse(chars);
        return new(chars);
    }

    public static int[] CalcZArray(string s)
    {
        int length = s.Length;
        int[] z = new int[length];
        z[0] = length;
        int l = 0;
        int r = 0;
        for (int i = 1; i < length; i++)
        {
            if (z[i - l] < r - i)
            {
                z[i] = z[i - l];
            }
            else
            {
                r = int.Max(r, i);
                while (r < length && s[r] == s[r - i])
                    r += 1;
                z[i] = r - i;
                l = i;
            }
        }

        return z;
    }
}