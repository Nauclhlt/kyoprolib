/// <summary>
/// <para>Convex Hull Trick. 一次関数の最大/最小クエリを処理する。</para>
/// <para>特定の場合での高速化は実装していないため、遅い場合がある。</para>
/// <para>依存: Set</para>
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class ConvexHullTrick<T> where T : INumber<T>
{
    // y = Ax + B
    private struct Line : IComparable<Line>
    {
        public T A;
        public T B;

        public Line(T a, T b)
        {
            A = a;
            B = b;
        }

        public int CompareTo(Line other)
        {
            if (A == other.A)
            {
                return B.CompareTo(other.B);
            }
            else
            {
                return A.CompareTo(other.A);
            }
        } 
    }

    private Set<Line> _lineSet;
    private bool _maxQuery;

    public int ActiveLineCount => _lineSet.Count;


    public ConvexHullTrick(CHTQuery queryType = CHTQuery.Min)
    {
        _lineSet = new();
        _maxQuery = queryType == CHTQuery.Max;
    }

    private T Calc(int index, T x)
    {
        Line l = _lineSet.GetByIndex(index);
        return l.A * x + l.B;
    }

    private bool IsUseless(int index, Line l)
    {
        if (index == 0 || index == _lineSet.Count - 1) return false;
        Line prev = _lineSet.GetByIndex(index - 1);
        Line next = _lineSet.GetByIndex(index + 1);
        if ((l.B - prev.B) * (next.A - l.A) >= (next.B - l.B) * (l.A - prev.A)) return true;
        return false;
    }

    /// <summary>
    /// y=ax+bを追加する。計算量: 分からん。線形時間よりは軽いはず。
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    public void Add(T a, T b)
    {
        if (_maxQuery)
        {
            a = -a;
            b = -b;
        }

        Line line = new Line(a, b);

        _lineSet.Add(line);
        int idx = _lineSet.IndexOf(line);
        if (IsUseless(idx, line))
        {
            _lineSet.Remove(line);
            return;
        }
        while (idx != _lineSet.Count - 1 && IsUseless(idx + 1, _lineSet.GetByIndex(idx + 1))) _lineSet.RemoveAtIndex(idx + 1);
        while (idx != 0 && IsUseless(idx - 1, _lineSet.GetByIndex(idx - 1)))
        {
            _lineSet.RemoveByIndex(idx - 1);
            idx--;
        }
    }

    /// <summary>
    /// x座標から最大/最小の値を計算する。計算量: O(log^2N)
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public T Query(T x)
    {
        int left = 0;
        int right = _lineSet.Count - 1;
        while (right > left)
        {
            int mid = left + (right - left) / 2;

            T r = Calc(mid + 1, x);
            T l = Calc(mid, x);
            if (l < r)
            {
                right = mid;
            }
            else
            {
                left = mid + 1;
            }
        }

        Line p = _lineSet.GetByIndex(left);
        if (_maxQuery)
            return -p.A * x - p.B;
        else
            return p.A * x + p.B;
    }   
}

public enum CHTQuery
{
    Max,
    Min
}