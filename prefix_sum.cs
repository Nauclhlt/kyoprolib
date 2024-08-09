// 累積和を管理する.
// @author Nauclhlt.
public sealed class PrefixSum<T> where T : struct, INumber<T>
{
    private T[] _sums;

    public PrefixSum(T[] sequence)
    {
        _sums = new T[sequence.Length + 1];

        _sums[0] = T.CreateChecked(0);
        for (int i = 0; i < sequence.Length; i++)
        {
            _sums[i + 1] = _sums[i] + sequence[i];
        }
    }

    // [a, b)
    public T Sum(int aIncl, int bExcl)
    {
        return _sums[bExcl] - _sums[aIncl];
    }

    public T AllSum()
    {
        return _sums[^1];
    }

    public T[] GetArray()
    {
        return _sums;
    }
}