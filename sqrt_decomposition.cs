// クエリ平方分割の抽象化.
// O(QlogQ + N√Q)だけど定数倍が激ヤバなのでTLEするときはクエリのソートだけして
// 区間の伸縮は普通に書いたほうが良い.(デリゲートの呼び出しを排除する)
// @author Nauclhlt.
public sealed class QuerySqrtDecomposition<TQuery>
{
    private int _n;
    private Action<int, int> _initialize;
    private Action _subLeft;
    private Action _addLeft;
    private Action _subRight;
    private Action _addRight;
    private Func<TQuery> _get;

    private List<(int l, int r, int number)> _queries = new();

    public QuerySqrtDecomposition(int n, Action<int, int> initialize, Action subLeft, Action addLeft, Action subRight, Action addRight, Func<TQuery> get)
    {
        _n = n;
        _initialize = initialize;
        _subLeft = subLeft;
        _addLeft = addLeft;
        _subRight = subRight;
        _addRight = addRight;
        _get = get;
    }

    public void AddQuery(int l, int r)
    {
        _queries.Add((l, r, _queries.Count));
    }

    public (int l, int r, int number)[] GetSortedQueries()
    {
        if (_queries.Count == 0) return Array.Empty<(int, int, int)>();

        int q = _queries.Count;
        TQuery[] res = new TQuery[q];

        int width = int.Max(1, (int)(1.0 * _n / double.Max(1.0, Math.Sqrt(q * 2.0 / 3.0))));

        _queries.Sort((a, b) =>
        {
            if (a.l / width != b.l / width)
            {
                return (a.l / width).CompareTo(b.l / width);
            }
            else
            {
                if (a.l == b.l && a.r == b.r)
                {
                    return a.number.CompareTo(b.number);
                }

                int n = a.l / width;
                if (n % 2 == 0) return a.r.CompareTo(b.r);
                else return b.r.CompareTo(a.r);
            }
        });

        return _queries.ToArray();
    }

    public TQuery[] Run()
    {
        if (_queries.Count == 0) return Array.Empty<TQuery>();

        int q = _queries.Count;
        TQuery[] res = new TQuery[q];

        int width = int.Max(1, (int)(1.0 * _n / double.Max(1.0, Math.Sqrt(q * 2.0 / 3.0))));

        _queries.Sort((a, b) =>
        {
            if (a.l / width != b.l / width)
            {
                return (a.l / width).CompareTo(b.l / width);
            }
            else
            {
                if (a.l == b.l && a.r == b.r)
                {
                    return a.number.CompareTo(b.number);
                }

                int n = a.l / width;
                if (n % 2 == 0) return a.r.CompareTo(b.r);
                else return b.r.CompareTo(a.r);
            }
        });

        int left = _queries[0].l;
        int right = _queries[0].r;

        _initialize(left, right);
        res[_queries[0].number] = _get();

        for (int i = 1; i < q; i++)
        {
            while (left < _queries[i].l)
            {
                _addLeft();
                left++;
            }

            while (left > _queries[i].l)
            {
                _subLeft();
                left--;
            }

            while (right < _queries[i].r)
            {
                _addRight();
                right++;
            }

            while (right > _queries[i].r)
            {
                _subRight();
                right--;
            }

            res[_queries[i].number] = _get();
        }

        return res;
    }
}