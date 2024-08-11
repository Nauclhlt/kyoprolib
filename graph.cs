// 重み付き(重みなし)の無向グラフを管理する.
// Depends on: Edge<T>, UnionFind
// @author Nauclhlt.
public sealed class Graph<T> where T : struct, INumber<T>
{
    private List<List<Edge<T>>> _graph;
    private List<Edge<T>> _edges;
    private bool[] _seen;
    private UnionFind _uf;

    private int _vertexCount;

    public int VertexCount => _vertexCount;
    public bool[] Seen => _seen;
    public UnionFind UnionFind => _uf;
    public List<List<Edge<T>>> RawGraph => _graph;
    public List<Edge<T>> Edges => _edges;

    public Graph(int vertexCount)
    {
        _vertexCount = vertexCount;
        _graph = new(_vertexCount);
        _edges = new List<Edge<T>>(_vertexCount);
        for (int i = 0; i < _vertexCount; i++)
        {
            _graph.Add(new());
        }
    }

    public void AddEdge(int a, int b, T weight)
    {
        if (!Validate(a) || !Validate(b)) return;

        _graph[a].Add(new Edge<T>(a, b, weight));
        _graph[b].Add(new Edge<T>(b, a, weight));
        _edges.Add(new Edge<T>(a, b, weight));
    }

    public void SetupDSU()
    {
        _uf = new UnionFind(_vertexCount);
        for (int i = 0; i < _edges.Count; i++)
        {
            _uf.Unite(_edges[i].From, _edges[i].To);
        }
    }

    public void SetupSearch()
    {
        _seen = new bool[_vertexCount];
    }

    public bool Same(int a, int b)
    {
        if (_uf is null) return false;

        return _uf.Same(a, b);
    }

    public Dictionary<int, List<int>> GetConnectedComponents()
    {
        if (_uf is null) return null;
        return _uf.FindAll();
    }

    public void DijkstraFrom(int n, T[] map)
    {
        if (!Validate(n)) return;

        if (_seen is null) throw new Exception("call SetupSearch.");

        Array.Clear(_seen);

        map[n] = T.Zero;

        PriorityQueue<int, T> pq = new();

        pq.Enqueue(n, T.Zero);

        while (pq.Count > 0)
        {
            int p = pq.Dequeue();

            if (_seen[p]) continue;

            _seen[p] = true;

            List<Edge<T>> children = _graph[p];
            for (int i = 0; i < children.Count; i++)
            {
                T w = map[p] + children[i].Weight;
                if (w < map[children[i].To])
                {
                    map[children[i].To] = w;
                    pq.Enqueue(children[i].To, map[children[i].To]);
                }
            }
        }
    }

    public void BfsFrom(int n, T[] map)
    {
        if (!Validate(n)) return;

        if (_seen is null) throw new Exception("call SetupSearch.");

        Array.Clear(_seen);

        map[n] = T.Zero;

        Queue<(int, T)> queue = new();

        queue.Enqueue((n, T.Zero));

        while (queue.Count > 0)
        {
            (int p, T w) = queue.Dequeue();

            if (_seen[p]) continue;

            _seen[p] = true;
            map[p] = w;

            List<Edge<T>> children = _graph[p];
            for (int i = 0; i < children.Count; i++)
            {
                queue.Enqueue((children[i].To, w + children[i].Weight));
            }
        }
    }

    // 木の直径.
    public T TreeDiameter()
    {
        if (_seen is null) throw new Exception("call SetupSearch.");
        if (_edges.Count != _vertexCount - 1)
        {
            throw new Exception("Not a tree graph.");
        }

        T[] dist = new T[_vertexCount];

        BfsFrom(0, dist);

        T max = T.Zero;
        int v = 0;
        for (int i = 0; i < _vertexCount; i++)
        {
            if (dist[i] > max)
            {
                max = dist[i];
                v = i;
            }
        }

        BfsFrom(v, dist);

        return dist.Max();
    }

    // 最大全域木(クラスカル法)
    public T MaxSpanningTreeWeight()
    {
        UnionFind unionFind = new(_vertexCount);

        T ans = T.Zero;
        foreach (var edge in _edges.OrderByDescending(x => x.Weight))
        {
            if (!unionFind.Same(edge.From, edge.To))
            {
                unionFind.Unite(edge.From, edge.To);
                ans += edge.Weight;
            }
        }

        return ans;
    }

    // 最小全域木(クラスカル法)
    public T MinSpanningTreeWeight()
    {
        UnionFind unionFind = new(_vertexCount);

        T ans = T.Zero;
        foreach (var edge in _edges.OrderBy(x => x.Weight))
        {
            if (!unionFind.Same(edge.From, edge.To))
            {
                unionFind.Unite(edge.From, edge.To);
                ans += edge.Weight;
            }
        }

        return ans;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool Validate(int n)
    {
        return 0 <= n && n < _vertexCount;
    }
}