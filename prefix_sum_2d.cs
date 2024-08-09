// 2次元累積和を管理する.
// @author Nauclhlt.
public sealed class PrefixSum2D<T> where T : struct, INumber<T>
{
    private T[,] _sums;

    public PrefixSum2D(T[,] sequence)
    {
        int height = sequence.GetLength(0);
        int width = sequence.GetLength(1);

        _sums = new T[height + 1, width + 1];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                _sums[y + 1, x + 1] = _sums[y + 1, x] + _sums[y, x + 1] - _sums[y, x] + sequence[y, x];
            }
        }
    }

    public T Sum(int startInclX, int startInclY, int endExclX, int endExclY)
    {
        return _sums[endExclY, endExclX] + _sums[startInclY, startInclX] - _sums[startInclY, endExclX] - _sums[endExclY, startInclX];
    }

    public T AllSum()
    {
        return _sums[_sums.GetLength(0) - 1, _sums.GetLength(1) - 1];
    }
}