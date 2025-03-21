/// <summary>
/// 2次元のsparse table。処理できる演算は結合法則および冪等性を満たす必要がある。
/// 割と遅い。
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class SparseTable2D<T>
{
    private Func<T, T, T> _op;
    private T _identity;
    private SparseTable<T>[][] _table;
    private int[] _lookup;
    private int _height;
    private int _width;
    private int _maxLength;

    public int Height => _height;
    public int Width => _width;

    /// <summary>
    /// 構築する。計算量: O(HWlogHlogW)
    /// </summary>
    /// <param name="source"></param>
    /// <param name="identity"></param>
    /// <param name="op"></param>
    public SparseTable2D(T[,] source, T identity, Func<T, T, T> op)
    {
        _height = source.GetLength(0);
        _width = source.GetLength(1);

        _identity = identity;
        _op = op;
        _maxLength = int.Max(_height, _width);

        _lookup = new int[_maxLength + 1];
        for (int i = 2; i <= _maxLength; i++)
        {
            _lookup[i] = _lookup[i >> 1] + 1;
        }

        int log = _lookup[_maxLength] + 1;

        _table = new SparseTable<T>[log][];
        for (int i = 0; i < log; i++)
        {
            _table[i] = new SparseTable<T>[_height];
        }

        for (int y = 0; y < _height; y++)
        {
            T[] v = new T[_width];
            for (int x = 0; x < _width; x++)
            {
                v[x] = source[y, x];
            }

            _table[0][y] = new (v, _identity, _op);
        }

        for (int h = 1; h < log; h++)
        {
            for (int i = 0; i + (1 << h) <= _height; i++)
            {
                T[] v = new T[_width];
                for (int j = 0; j < _width; j++)
                {
                    v[j] = _op(_table[h - 1][i].Query(j, j + 1), _table[h - 1][i + (1 << (h - 1))].Query(j, j + 1));
                }

                _table[h][i] = new(v, _identity, _op);
            }
        }
    }

    /// <summary>
    /// (x1, y1)を左上、(x2, y2)を左下(含まない)とした矩形領域の積を返す。計算量: O(1)
    /// </summary>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    /// <returns></returns>
    public T Query(int x1, int y1, int x2, int y2)
    {
        if (x1 == x2 || y1 == y2) return _identity;
        int h = _lookup[y2 - y1];
        return _op(_table[h][y1].Query(x1, x2), _table[h][y2 - (1 << h)].Query(x1, x2));
    }
}