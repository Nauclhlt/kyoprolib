public struct Matrix<T> where T : struct, IAdditionOperators<T, T, T>, IMultiplyOperators<T, T, T>, ISubtractionOperators<T, T, T>
{
    private T[,] _data;
    private int _size;

    public T this[int r, int c]
    {
        get
        {
            return _data[r, c];
        }
        set
        {
            _data[r, c] = value;
        }
    }

    public T[,] Data => _data;
    public int Size => _size;

    public Matrix(int size)
    {
        _size = size;
        _data = new T[size, size];
    }

    public static Matrix<T> operator +(Matrix<T> left, Matrix<T> right)
    {
        if (left.Size != right.Size)
        {
            throw new InvalidOperationException();
        }
        Matrix<T> dest = new(left.Size);
        for (int r = 0; r < left.Size; r++)
        {
            for (int c = 0; c < left.Size; c++)
            {
                dest[r, c] = left[r, c] + right[r, c];
            }
        }

        return dest;
    }

    public static Matrix<T> operator -(Matrix<T> left, Matrix<T> right)
    {
        if (left.Size != right.Size)
        {
            throw new InvalidOperationException();
        }
        Matrix<T> dest = new(left.Size);
        for (int r = 0; r < left.Size; r++)
        {
            for (int c = 0; c < left.Size; c++)
            {
                dest[r, c] = left[r, c] - right[r, c];
            }
        }

        return dest;
    }

    public static Matrix<T> operator *(Matrix<T> left, Matrix<T> right)
    {
        if (left.Size != right.Size)
        {
            throw new InvalidOperationException();
        }

        Matrix<T> dest = new(left.Size);

        for (int r = 0; r < dest.Size; r++)
        {
            for (int c = 0; c < dest.Size; c++)
            {
                for (int i = 0; i < dest.Size; i++)
                {
                    dest[r, c] += right[i, c] * left[r, i];
                }
            }
        }

        return dest;
    }

    public Matrix<T> Power(long exp)
    {
        if (exp == 1) return this;

        Matrix<T> half = Power(exp / 2);
        Matrix<T> res = half * half;
        if (exp % 2 == 1) res *= this;

        return res;
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        for (int r = 0; r < _size; r++)
        {
            sb.Append('|');
            for (int c = 0; c < _size; c++)
            {
                sb.Append(_data[r, c]);
                if (c != _size - 1)
                {
                    sb.Append(',');
                    sb.Append('\t');
                }
                
            }
            sb.Append('|');
            sb.Append(Environment.NewLine);
        }

        return sb.ToString();
    }
}