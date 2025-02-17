public sealed class SparseTable<T>
{
    private Func<T, T, T> _op;
    private T[][] _table;
    private int[] _lookup;
    private int _length;

    public SparseTable(T[] array, Func<T, T, T> op)
    {
        _op = op;
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

    public T Query(int l, int r)
    {
        if (l >= r) throw new InvalidOperationException();
        int len = r - l;
        int x = _lookup[len];
        return _op(_table[x][l], _table[x][r - (1 << x)]);
    }
}