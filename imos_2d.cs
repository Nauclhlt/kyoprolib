// いもす法を使って2次元配列に対する区間和クエリをまとめて処理する.
// 区間は半開区間.
// @author Nauclhlt.
public sealed class Imos2D<T> where T : struct, INumber<T>
{
    private T[,] _data;
    private int _width;
    private int _height;

    public Imos2D(T[,] data)
    {
        _data = data;
        _width = data.GetLength(1);
        _height = data.GetLength(0);
    }

    public Imos2D(int h, int w)
    {
        _data = new T[h, w];
        _width = w;
        _height = h;
    }

    public void AddQuery(int startX, int startY, int endX, int endY, T value)
    {
        _data[startY, startX] += value;
        if (endX < _width)
        {
            _data[startY, endX] -= value;
        }
        if (endY < _height)
        {
            _data[endY, startX] -= value;
        }
        if (endX < _width && endY < _height)
        {
            _data[endY, endX] += value;
        }
    }

    public void AddQueryLen(int x, int y, int w, int h, T value)
    {
        this.AddQuery(x, y, x + w, y + h, value);
    }

    public void Simulate()
    {
        for (int x = 1; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                _data[y, x] += _data[y, x - 1];
            }
        }

        for (int y = 1; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                _data[y, x] += _data[y - 1, x];
            }
        }
    }

    public T[,] GetData()
    {
        return _data;
    }
}