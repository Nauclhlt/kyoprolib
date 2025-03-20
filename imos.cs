/// <summary>
/// imos法。区間加算クエリをまとめて処理する。
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class Imos<T> where T : struct, INumber<T>
{
    private T[] _data;
    

    public Imos(T[] array)
    {
        _data = array;
    }

    public Imos(int len)
    {
        _data = new T[len];
    }

    /// <summary>
    /// [start, start + length)にvalueを加算する。計算量: O(1)
    /// </summary>
    /// <param name="start"></param>
    /// <param name="length"></param>
    /// <param name="value"></param>
    public void AddQueryLen(int start, int length, T value)
    {
        this.AddQuery(start, start + length, value);
    }

    /// <summary>
    /// [start, end)にvalueを加算する。計算量: O(1)
    /// </summary>
    /// <param name="start"></param>
    /// <param name="length"></param>
    /// <param name="value"></param>
    public void AddQuery(int start, int end, T value)
    {
        _data[start] += value;
        if (end < _data.Length)
        {
            _data[end] -= value;
        }
    }

    /// <summary>
    /// 累積和を取って、区間加算を反映する。計算量: O(n)
    /// </summary>
    public void Accumulate()
    {
        for (int i = 1; i < _data.Length; i++)
        {
            _data[i] += _data[i - 1];
        }
    }

    /// <summary>
    /// 中身の配列を返す。計算量: O(1)
    /// </summary>
    /// <returns></returns>
    public T[] GetData()
    {
        return _data;
    }
}