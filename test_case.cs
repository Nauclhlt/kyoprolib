// テストケースをランダム生成する.
// 
public static class TestCase
{
    private static Random _random = new Random();

    public static void Seed(int seed)
    {
        _random = new Random(seed);
    }
    
    public static string RandomStringLower(int length)
    {
        Span<char> span = stackalloc char[length];
        for (int i = 0; i < length; i++)
        {
            char ch = (char)('a' + _random.Next(0, 26));
            span[i] = ch;
        }
        return new string(span);
    }

    public static string RandomStringUpper(int length)
    {
        return RandomStringLower(length).ToUpperInvariant();
    }

    public static int[] RandomPermutation(int length)
    {
        int[] d = new int[length];
        for (int i = 0; i < length; i++) d[i] = i + 1;
        for (int i = d.Length - 1; i > 0; i--)
        {
            int index = random.Next(0, i + 1);
            (d[index], d[i]) = (d[i], d[index]);
        }
        return d;
    }

    public static long[] RandomArrayLong(int length, long min, long max)
    {
        long[] d = new long[length];
        for (int i = 0; i < length; i++)
        {
            d[i] = _random.NextInt64(min, max + 1);
        }

        return d;
    }

    public static int[] RandomArray(int length, int min, int max)
    {
        int[] d = new int[length];
        for (int i = 0; i < length; i++)
        {
            d[i] = _random.Next(min, max + 1);
        }

        return d;
    }

    public static int[,] Random2DArray(int height, int width, int min, int max)
    {
        int[,] d = new int[height, width];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                d[y, x] = _random.Next(min, max + 1);
            }
        }

        return d;
    }

    public static long[,] Random2DArrayLong(int height, int width, int min, int max)
    {
        long[,] d = new long[height, width];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                d[y, x] = _random.NextInt64(min, max + 1);
            }
        }

        return d;
    }

    public static char[,] RandomCharGrid(int height, int width, params char[] tiles)
    {
        char[,] d = new char[height, width];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                d[y, x] = tiles[_random.Next(0, tiles.Length)];
            }
        }

        return d;
    }
}