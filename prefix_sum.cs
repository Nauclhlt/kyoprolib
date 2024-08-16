// 累積和を管理する.
// @author Nauclhlt.
public sealed class PrefixSum<T> where T : struct, INumber<T>
{
    private T[] _sums;

    // sequenceの累積和を計算する.
    // O(N)
    public PrefixSum(T[] sequence)
    {
        _sums = new T[sequence.Length + 1];

        _sums[0] = T.CreateChecked(0);
        for (int i = 0; i < sequence.Length; i++)
        {
            _sums[i + 1] = _sums[i] + sequence[i];
        }
    }

    // 区間[a, b)の和を計算する.
    // O(1)
    public T Sum(int aIncl, int bExcl)
    {
        return _sums[bExcl] - _sums[aIncl];
    }

    // 全体の和を返す.
    // O(1)
    public T AllSum()
    {
        return _sums[^1];
    }

    // 累積和の配列を返す.
    public T[] GetArray()
    {
        return _sums;
    }
}