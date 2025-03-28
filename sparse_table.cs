/// <summary>
/// sparse table。処理できる演算は結合法則および冪等性を満たす必要がある。
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class SparseTable<T>
{
    private Func<T, T, T> _op;
    private T[][] _table;
    private int[] _lookup;
    private int _length;
    private T _identity;

    /// <summary>
    /// 構築する。計算量: O(NlogN)
    /// </summary>
    /// <param name="array"></param>
    /// <param name="identity"></param>
    /// <param name="op"></param>
    public SparseTable(T[] array, T identity, Func<T, T, T> op)
    {
        _op = op;
        _identity = identity;
        _length = array.Length;
        int exp = 0;
        while (1 << (exp + 1) <= array.Length) exp++;
        _table = new T[exp + 1][];
        for (int i = 0; i <= exp; i++)
        {
            _table[i] = new T[_length];
        }

        for (int i = 0; i <= exp; i++)
        {
            int width = 1 << i;
            for (int j = 0; j <= _length - width; j++)
            {
                if (width == 1) _table[i][j] = array[j];
                else _table[i][j] = _op(_table[i - 1][j], _table[i - 1][j + (1 << (i - 1))]);
            }
        }

        _lookup = new int[_length + 1];
        for (int i = 2; i <= _length; i++)
        {
            _lookup[i] = _lookup[i / 2] + 1;
        }
    }

    /// <summary>
    /// 区間[l, r)の積を返す。計算量: O(1)
    /// </summary>
    /// <param name="l"></param>
    /// <param name="r"></param>
    /// <returns></returns>
    public T Query(int l, int r)
    {
        if (l >= r) return _identity;
        int len = r - l;
        int x = _lookup[len];
        return _op(_table[x][l], _table[x][r - (1 << x)]);
    }
}