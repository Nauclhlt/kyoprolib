public sealed class PrefixSum3D<T> where T : struct, IAdditionOperators<T, T, T>, ISubtractionOperators<T, T, T>
{
    private T[,,] _sums;

    public PrefixSum3D(T[,,] sequence)
    {
        int depth = sequence.GetLength(0);
        int height = sequence.GetLength(1);
        int width = sequence.GetLength(2);

        _sums = new T[depth + 1, height + 1, width + 1];
        for (int z = 0; z < depth; z++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    _sums[z + 1, y + 1, x + 1] = _sums[z + 1, y + 1, x] + _sums[z + 1, y, x + 1] + _sums[z, y + 1, x + 1] - _sums[z + 1, y, x] - _sums[z, y + 1, x] - _sums[z, y, x + 1] + _sums[z, y, x] + sequence[z, y, x];
                }
            }
        }
    }

    public T Sum(int x1, int y1, int z1, int x2, int y2, int z2)
    {
        return _sums[z2, y2, x2] - _sums[z1, y2, x2] - _sums[z2, y1, x2] - _sums[z2, y2, x1] + _sums[z2, y1, x1] + _sums[z1, y2, x1] + _sums[z1, y1, x2] - _sums[z1, y1, x1];
    }

    public T AllSum()
    {
        return _sums[_sums.GetLength(0), _sums.GetLength(1), _sums.GetLength(2)];
    }
}