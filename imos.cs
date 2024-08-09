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

    public void AddQueryLen(int start, int length, T value)
    {
        this.AddQuery(start, start + length, value);
    }

    public void AddQuery(int start, int end, T value)
    {
        _data[start] += value;
        if (end < _data.Length)
        {
            _data[end] -= value;
        }
    }

    public void Simulate()
    {
        for (int i = 1; i < _data.Length; i++)
        {
            _data[i] += _data[i - 1];
        }
    }

    public T[] GetData()
    {
        return _data;
    }
}