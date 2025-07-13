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
}