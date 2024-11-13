// 素集合データ構造(Disjoint Set Union)を管理する.
// @author Nauclhlt.
public sealed class UnionFind
{
    private int[] _parents;
    private int[] _size;
    private int _vertexCount;

    public int VertexCount => _vertexCount;

    public UnionFind(int n)
    {
        _vertexCount = n;
        _parents = new int[n];
        _size = new int[n];
        for (int i = 0; i < n; i++)
        {
            _parents[i] = i;
            _size[i] = 1;
        }
    }

    // xが属する木の根を返す.
    public int Root(int x)
    {
        if (_parents[x] == x)
            return x;
        return _parents[x] = Root(_parents[x]);
    }

    // xが属する連結成分のサイズを返す.
    public int Size(int x)
    {
        return _size[Root(x)];
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
        // merge from to to
        if (_size[from] > _size[to])
        {
            (from, to) = (to, from);
        }

        _size[to] += _size[from];
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
            _size[i] = i;
        }
    }
}