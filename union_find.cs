// 素集合データ構造(Disjoint Set Union)を管理する.
// @author Nauclhlt.
public sealed class UnionFind
{
    private int[] _parents;
    private int _size;

    public UnionFind(int n)
    {
        _size = n;
        _parents = new int[n];
        for (int i = 0; i < n; i++)
        {
            _parents[i] = i;
        }
    }

    public int Root(int x)
    {
        if (_parents[x] == x)
            return x;
        return _parents[x] = Root(_parents[x]);
    }

    public void Unite(int x, int y)
    {
        int rootX = Root(x);
        int rootY = Root(y);
        if (rootX == rootY)
            return;
        _parents[rootX] = rootY;
    }

    public List<int> Find(int x)
    {
        int rootX = Root(x);
        List<int> set = new List<int>();
        for (int i = 0; i < _size; i++)
        {
            if (Root(i) == rootX)
                set.Add(i);
        }

        return set;
    }

    public Dictionary<int, List<int>> FindAll()
    {
        Dictionary<int, List<int>> sets = new Dictionary<int, List<int>>();
        for (int i = 0; i < _size; i++)
        {
            int root = Root(i);
            if (sets.ContainsKey(root))
                sets[root].Add(i);
            else
                sets[root] = new List<int>() { i };
        }

        return sets;
    }

    public bool Same(int x, int y)
    {
        int rootX = Root(x);
        int rootY = Root(y);
        return rootX == rootY;
    }
}