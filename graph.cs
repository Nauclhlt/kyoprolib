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

    // 頂点fromとtoの間に重みweightの無向辺を追加する.
    // O(1)
    public void AddEdge(int a, int b, T weight)
    {
        if (!Validate(a) || !Validate(b)) return;

        if (a > b) (a, b) = (b, a);

        _graph[a].Add(new Edge<T>(a, b, weight));
        _graph[b].Add(new Edge<T>(b, a, weight));
        _edges.Add(new Edge<T>(a, b, weight));
    }

    // このグラフの連結の状態を表すDSUを生成する.
    // O(V+E)
    public void SetupDSU()
    {
        _uf = new UnionFind(_vertexCount);
        for (int i = 0; i < _edges.Count; i++)
        {
            _uf.Unite(_edges[i].From, _edges[i].To);
        }
    }

    // 探索用の配列を作成する.
    public void SetupSearch()
    {
        _seen = new bool[_vertexCount];
    }

    // 頂点a, bが同じ連結成分に属するかを判定する.
    // ほぼ定数時間
    public bool Same(int a, int b)
    {
        if (_uf is null) throw new Exception("Call SetupSearch.");

        return _uf.Same(a, b);
    }

    // 連結成分ごとに頂点のリストを返す.
    // O(V)
    public Dictionary<int, List<int>> GetConnectedComponents()
    {
        if (_uf is null) throw new Exception("Call SetupSearch.");
        return _uf.FindAll();
    }

    // 頂点nからDijkstra法を用いてそれぞれの頂点に対する最短経路を求める.
    // O(VlogE)
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

    // 頂点nからBFSをしてそれぞれの頂点にはじめに訪問したときのパスの重みの合計を配列に格納する.
    // O(V+E)
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

    // 補グラフを作成する.
    // O(V^2)
    public Graph<T> CreateComplement()
    {
        HashSet<(int, int)> edgeSet = new();
        for (int i = 0; i < _edges.Count; i++)
        {
            edgeSet.Add((_edges[i].From, _edges[i].To));
        }

        Graph<T> g = new(_vertexCount);

        for (int i = 0; i < _vertexCount - 1; i++)
        {
            for (int j = i + 1; j < _vertexCount; j++)
            {
                if (!edgeSet.Contains((i, j)))
                {
                    g.AddEdge(i, j, default);
                }
            }
        }

        return g;
    }

    // 二部グラフか判定する.
    // O(V+E)
    public bool IsBipartite()
    {
        if (_seen is null) throw new Exception("Call SetupSearch.");

        Array.Clear(_seen);

        Stack<(int, bool)> stack = new();

        bool[] memo = new bool[_vertexCount];

        for (int i = 0; i < _vertexCount; i++)
        {
            stack.Push((i, false));

            while (stack.Count > 0)
            {
                (int n, bool c) = stack.Pop();

                if (_seen[n])
                {
                    if (memo[n] != !c) return false;
                    continue;
                }

                _seen[n] = true;
                memo[n] = !c;

                var ch = _graph[n];
                for (int j = 0; j < ch.Count; j++)
                {
                    stack.Push((ch[j].To, !c));
                }
            }
        }

        return true;
    }

    // 木の直径を求める.
    // O(V+E)
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

    // Kruskal法で最大全域木の重みの総和を求める.
    // O(V+E)
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

    // Kruskal法で最小全域木の重みの総和を求める
    // O(V+E)
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