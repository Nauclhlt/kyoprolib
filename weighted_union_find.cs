// 重み(ポテンシャル)付きUnion-Find.
public sealed class WeightedUnionFind<T> where T : struct, IAdditionOperators<T, T, T>, ISubtractionOperators<T, T, T>
{
    private int[] _parents;
    private T[] _weights;
    private int _size;

    public int Size => _size;

    public WeightedUnionFind(int n)
    {
        _size = n;
        _parents = new int[n];
        _weights = new T[n];
        for (int i = 0; i < n; i++)
        {
            _parents[i] = i;
        }
    }

    public int Root(int x)
    {
        if (_parents[x] == x)
            return x;
        
        int root = Root(_parents[x]);
        _weights[x] += _weights[_parents[x]];
        return _parents[x] = root;
    }

    public T Weight(int x)
    {
        Root(x);
        return _weights[x];
    }

    public T WeightDifference(int x, int y)
    {
        return Weight(y) - Weight(x);
    }

    public void Unite(int x, int y, T weight)
    {
        weight += Weight(x);
        weight -= Weight(y);

        int rootX = Root(x);
        int rootY = Root(y);
        if (rootX == rootY)
            return;
        
        _parents[rootY] = rootX;
        _weights[rootY] = weight;
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

    public void Clear()
    {
        for (int i = 0; i < _size; i++)
        {
            _parents[i] = i;
        }
    }
}