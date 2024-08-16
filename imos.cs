// いもす法を使って区間和クエリをまとめて処理する.
// 区間は半開区間.
// @author Nauclhlt.
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

    // startを左端とする長さlengthの区間にvalueを加算する.
    // O(1)
    public void AddQueryLen(int start, int length, T value)
    {
        this.AddQuery(start, start + length, value);
    }

    // 区間[start, end)にvalueを加算する.
    // O(1)
    public void AddQuery(int start, int end, T value)
    {
        _data[start] += value;
        if (end < _data.Length)
        {
            _data[end] -= value;
        }
    }

    // 累積和を取って和を計算する.
    // O(N)
    public void Simulate()
    {
        for (int i = 1; i < _data.Length; i++)
        {
            _data[i] += _data[i - 1];
        }
    }

    // 中身の配列を取得する.
    public T[] GetData()
    {
        return _data;
    }
}