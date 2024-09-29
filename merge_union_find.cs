// データを乗せた素集合データ構造(Disjoint Set Union)を管理する.
// @author Nauclhlt.
public sealed class MergeUnionFind<T>
{
    private int[] _parents;
    private T[] _data;
    private int _vertexCount;

    private Func<int, T> _init;
    private Func<T, T, T> _merge;
    private Func<T, T, bool> _dir;

    public int VertexCount => _vertexCount;

    // init(i) => 頂点iの初期値を返す
    // merge(a, b) => aをbにマージした結果を返す
    // dir(a, b) => a → bにマージするならtrue, b → aにマージするならfalseを返す
    public MergeUnionFind(int n, Func<int, T> init, Func<T, T, T> merge, Func<T, T, bool> dir)
    {
        _init = init;
        _merge = merge;
        _dir = dir;

        _vertexCount = n;
        _parents = new int[n];
        _data = new T[n];
        for (int i = 0; i < n; i++)
        {
            _data[i] = _init(i);
            _parents[i] = i;
        }
    }

    // xが属する木の根を返す.
    public int Root(int x)
    {
        if (_parents[x] == x)
            return x;
        return _parents[x] = Root(_parents[x]);
    }

    // xが属する連結成分の値を返す
    public T Value(int x)
    {
        return _data[x];
    }

    // xの属する木とyの属する木を併合する.
    public void Unite(int x, int y)
    {
        int rootX = Root(x);
        int rootY = Root(y);
        if (rootX == rootY)
            return;
            
        int from = rootX;
        int to = rootY;

        if (!_dir(_data[from], _data[to]))
        {
            (from, to) = (to, from);
        }

        _data[to] = _merge(_data[from], _data[to]);
        _parents[from] = to;
    }

    // xと同じ連結成分に含まれる頂点のリストを返す.
    // O(N)
    public List<int> Find(int x)
    {
        int rootX = Root(x);
        List<int> set = new List<int>();
        for (int i = 0; i < _vertexCount; i++)
        {
            if (Root(i) == rootX)
                set.Add(i);
        }

        return set;
    }

    // すべての連結成分に対して頂点のリストを求める.
    // O(N)
    public Dictionary<int, List<int>> FindAll()
    {
        Dictionary<int, List<int>> sets = new Dictionary<int, List<int>>();
        for (int i = 0; i < _vertexCount; i++)
        {
            int root = Root(i);
            if (sets.ContainsKey(root))
                sets[root].Add(i);
            else
                sets[root] = new List<int>() { i };
        }

        return sets;
    }

    // xとyが同じ連結成分に属しているかを返す.
    // ほぼ定数時間
    public bool Same(int x, int y)
    {
        int rootX = Root(x);
        int rootY = Root(y);
        return rootX == rootY;
    }

    // クリアする.
    // O(N)
    public void Clear()
    {
        for (int i = 0; i < _vertexCount; i++)
        {
            _parents[i] = i;
        }
    }
}