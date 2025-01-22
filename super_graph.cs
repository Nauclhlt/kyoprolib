public abstract class GraphBase<T> where T : struct, INumber<T>, IMinMaxValue<T>
{
    protected List<List<Edge<T>>> _adjList;
    protected List<Edge<T>> _directionAwareEdges;
    protected int _vertexCount;

    public int VertexCount => _vertexCount;
    public List<List<Edge<T>>> AdjList => _adjList;
    public List<Edge<T>> DirectionAwareEdges => _directionAwareEdges;

    protected void Initialize(int vertexCount)
    {
        _vertexCount = vertexCount;
        _adjList = new (vertexCount);
        for (int i = 0; i < vertexCount; i++)
        {
            _adjList.Add(new());
        }
        _directionAwareEdges = new();
    }

    protected void Sync(int vertexCount, List<List<Edge<T>>> adjList, List<Edge<T>> daEdges)
    {
        _vertexCount = vertexCount;
        _adjList = adjList;
        _directionAwareEdges = daEdges;
    }

    public UnionFind CreateDSU()
    {
        UnionFind uf = new(_vertexCount);
        for (int i = 0; i < _directionAwareEdges.Count; i++)
        {
            uf.Unite(_directionAwareEdges[i].From, _directionAwareEdges[i].To);
        }
        

        return uf;
    }

    public T[] DijkstraFrom(int n)
    {
        if (!Validate(n)) return null;

        bool[] seen = new bool[_vertexCount];
        T[] map = new T[_vertexCount];
        Array.Fill(map, T.MaxValue);

        map[n] = T.Zero;

        PriorityQueue<int, T> pq = new();

        pq.Enqueue(n, T.Zero);

        while (pq.Count > 0)
        {
            int p = pq.Dequeue();

            if (seen[p]) continue;

            seen[p] = true;

            List<Edge<T>> children = _adjList[p];
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

        return map;
    }

    public T[,] WarshallFloyd()
    {
        if (_vertexCount > 800)
        {
            throw new InvalidOperationException("Too large graph.");
        }

        T[,] map = new T[_vertexCount, _vertexCount];

        for (int i = 0; i < _vertexCount; i++)
        {
            for (int j = 0; j < _vertexCount; j++)
            {
                map[i, j] = T.MaxValue;
            }
        }

        for (int i = 0; i < _vertexCount; i++)
        {
            map[i, i] = T.Zero;
        }

        for (int i = 0; i < _directionAwareEdges.Count; i++)
        {
            Edge<T> e = _directionAwareEdges[i];
            map[e.From, e.To] = T.Min(e.Weight, map[e.From, e.To]);
        }

        for (int k = 0; k < _vertexCount; k++)
        {
            for (int j = 0; j < _vertexCount; j++)
            {
                for (int i = 0; i < _vertexCount; i++)
                {
                    if (map[i, k] != T.MaxValue && map[k, j] != T.MaxValue)
                    {
                        if (map[i, k] + map[k, j] < map[i, j])
                        {
                            map[i, j] = map[i, k] + map[k, j];
                        }
                    }
                }
            }
        }

        return map;
    }

    public abstract void AddEdge(int a, int b, T weight);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected bool Validate(int n)
    {
        return 0 <= n && n < _vertexCount;
    }
}

public class Graph<T> : GraphBase<T> where T : struct, INumber<T>, IMinMaxValue<T>
{
    private List<Edge<T>> _edges;

    public List<Edge<T>> Edges => _edges;

    public Graph(int vertexCount)
    {
        Initialize(vertexCount);
        _edges = new();
    }

    public override void AddEdge(int a, int b, T weight)
    {
        if (!Validate(a) || !Validate(b)) return;

        if (a > b)
        {
            (a, b) = (b, a);
        }

        Edge<T> right = new Edge<T>(a, b, weight);
        Edge<T> left = new Edge<T>(b, a, weight);

        _adjList[a].Add(right);
        _adjList[b].Add(left);
        _edges.Add(right);
        _directionAwareEdges.Add(left);
        _directionAwareEdges.Add(right);
    }

    public DfsTimeLabel DfsLabelFrom(int from)
    {
        if (!Validate(from)) return null;

        bool[] seen = new bool[_vertexCount];

        int timestamp = 0;

        int[] inLabel = new int[_vertexCount];
        int[] outLabel = new int[_vertexCount];

        void dfs(int n, int prev)
        {
            inLabel[n] = timestamp;
            timestamp++;

            seen[n] = true;

            var ch = _adjList[n];
            
            for (int i = 0; i < ch.Count; i++)
            {
                if (ch[i].To == prev || seen[ch[i].To]) continue;

                dfs(ch[i].To, n);
            }

            outLabel[n] = timestamp;
            timestamp++;
        }

        dfs(from, -1);

        return new DfsTimeLabel(inLabel, outLabel);
    }

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

    public bool IsBipartite()
    {
        bool[] seen = new bool[_vertexCount];

        Stack<(int, bool)> stack = new();

        bool[] memo = new bool[_vertexCount];

        for (int i = 0; i < _vertexCount; i++)
        {
            stack.Push((i, false));

            while (stack.Count > 0)
            {
                (int n, bool c) = stack.Pop();

                if (seen[n])
                {
                    if (memo[n] != !c) return false;
                    continue;
                }

                seen[n] = true;
                memo[n] = !c;

                var ch = _adjList[n];
                for (int j = 0; j < ch.Count; j++)
                {
                    stack.Push((ch[j].To, !c));
                }
            }
        }

        return true;
    }

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
}

public class DirectedGraph<T> : GraphBase<T> where T : struct, INumber<T>, IMinMaxValue<T>
{
    private List<List<Edge<T>>> _reverseAdjList;
    private List<Edge<T>> _reverseEdges;

    public List<List<Edge<T>>> ReverseAdjList => _reverseAdjList;
    public List<Edge<T>> ReverseEdges => _reverseEdges;
    public List<Edge<T>> Edges => _directionAwareEdges;

    public DirectedGraph(int vertexCount)
    {
        Initialize(vertexCount);
        _reverseAdjList = new(vertexCount);
        for (int i = 0; i < vertexCount; i++)
        {
            _reverseAdjList.Add(new());
        }
        _reverseEdges = new();
    }

    public override void AddEdge(int a, int b, T weight)
    {
        if (!Validate(a) || !Validate(b)) return;

        Edge<T> e = new Edge<T>(a, b, weight);
        Edge<T> rev = new Edge<T>(b, a, weight);

        _adjList[a].Add(e);
        _reverseAdjList[b].Add(rev);
        _directionAwareEdges.Add(e);
        _reverseEdges.Add(rev);
    }

    public bool TryTopologicalSort(out List<int> sorted)
    {
        sorted = new List<int>(_vertexCount);

        int[] deg = new int[_vertexCount];
        for (int i = 0; i < _directionAwareEdges.Count; i++)
        {
            deg[_directionAwareEdges[i].To]++;
        }

        Queue<int> queue = new();
        for (int i = 0; i < _vertexCount; i++)
        {
            if (deg[i] == 0) queue.Enqueue(i);
        }

        while (queue.Count > 0)
        {
            int next = queue.Dequeue();
            sorted.Add(next);

            List<Edge<T>> p = _adjList[next];
            for (int i = 0; i < p.Count; i++)
            {
                deg[p[i].To]--;
                if (deg[p[i].To] < 0) return false;

                if (deg[p[i].To] == 0)
                {
                    queue.Enqueue(p[i].To);
                }
            }
        }

        return sorted.Count == _vertexCount;
    }

    public bool TryUniqueTopologicalSort(out List<int> sorted)
    {
        sorted = new List<int>(_vertexCount);

        int[] deg = new int[_vertexCount];
        for (int i = 0; i < _directionAwareEdges.Count; i++)
        {
            deg[_directionAwareEdges[i].To]++;
        }

        Queue<int> queue = new();
        for (int i = 0; i < _vertexCount; i++)
        {
            if (deg[i] == 0) queue.Enqueue(i);
        }

        while (queue.Count > 0)
        {
            if (queue.Count > 1) return false;
            
            int next = queue.Dequeue();
            sorted.Add(next);

            List<Edge<T>> p = _adjList[next];
            for (int i = 0; i < p.Count; i++)
            {
                deg[p[i].To]--;
                if (deg[p[i].To] < 0) return false;

                if (deg[p[i].To] == 0)
                {
                    queue.Enqueue(p[i].To);
                }
            }

            
        }

        return sorted.Count == _vertexCount;
    }

    public List<List<int>> SplitSCC()
    {
        bool[] seen = new bool[_vertexCount];

        int[] t = new int[_vertexCount];

        int seenIndex = 1;
        void dfs1(int n)
        {
            seen[n] = true;

            var ch = _adjList[n];
            for (int i = 0; i < ch.Count; i++)
            {
                if (!seen[ch[i].To])
                    dfs1(ch[i].To);
            }

            t[n] = seenIndex;
            seenIndex++;
        }

        for (int i = 0; i < _vertexCount; i++)
        {
            if (!seen[i])
            {
                dfs1(i);
            }
        }

        Array.Clear(seen);

        PriorityQueue<int, int> pq = new();
        for (int i = 0; i < _vertexCount; i++)
        {
            pq.Enqueue(i, -t[i]);
        }

        List<List<int>> res = new();

        Stack<int> stack = new();
        while (pq.Count > 0)
        {
            int p = pq.Dequeue();
            if (seen[p]) continue;

            List<int> list = new();

            stack.Push(p);

            while (stack.Count > 0)
            {
                int n = stack.Pop();

                if (seen[n]) continue;

                seen[n] = true;
                list.Add(n);

                var ch = _reverseAdjList[n];
                for (int j = 0; j < ch.Count; j++)
                {
                    stack.Push(ch[j].To);
                }
            }

            res.Add(list);
        }

        return res;
    }
}

public abstract class GraphDecorator<T> : GraphBase<T> where T : struct, INumber<T>, IMinMaxValue<T>
{
    protected Graph<T> _graph;

    public Graph<T> Graph => _graph;
    
    public GraphDecorator(Graph<T> graph)
    {
        _graph = graph;
        Sync(graph.VertexCount, graph.AdjList, graph.DirectionAwareEdges);
    }
}

public abstract class DirectedGraphDecorator<T> : GraphBase<T> where T : struct, INumber<T>, IMinMaxValue<T>
{
    protected DirectedGraph<T> _graph;

    public DirectedGraph<T> Graph => _graph;
    
    public DirectedGraphDecorator(DirectedGraph<T> graph)
    {
        _graph = graph;
        Sync(graph.VertexCount, graph.AdjList, graph.DirectionAwareEdges);
    }
}

public class TreeDecorator<T> : GraphDecorator<T> where T : struct, INumber<T>, IMinMaxValue<T>
{
    public TreeDecorator(Graph<T> graph) : base(graph)
    {
        if (graph.Edges.Count != graph.VertexCount - 1)
        {
            throw new InvalidOperationException("Not a tree graph.");
        }
    }

    public T[] BfsFrom(int n)
    {
        if (!Validate(n)) return null;

        bool[] seen = new bool[_vertexCount];

        T[] map = new T[_vertexCount];
        map[n] = T.Zero;

        Queue<(int, T)> queue = new();

        queue.Enqueue((n, T.Zero));

        while (queue.Count > 0)
        {
            (int p, T w) = queue.Dequeue();

            if (seen[p]) continue;

            seen[p] = true;
            map[p] = w;

            List<Edge<T>> children = _adjList[p];
            for (int i = 0; i < children.Count; i++)
            {
                queue.Enqueue((children[i].To, w + children[i].Weight));
            }
        }

        return map;
    }

    public T GetDiameter()
    {
        T[] dist = BfsFrom(0);

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

        dist = BfsFrom(v);

        return dist.Max();
    }

    public override void AddEdge(int a, int b, T weight)
    {
        _graph.AddEdge(a, b, weight);
    }
}

public class FunctionalGraphDecorator<T> : DirectedGraphDecorator<T> where T : struct, INumber<T>, IMinMaxValue<T>
{
    public FunctionalGraphDecorator(DirectedGraph<T> graph) : base(graph)
    {
        for (int i = 0; i < _vertexCount; i++)
        {
            if (_adjList[i].Count != 1)
            {
                throw new InvalidOperationException("Not a functional graph.");
            }
        }
    }

    public (List<List<int>> cycles, Graph<T> trees) SplitCycleTree(bool sortCycle = false)
    {
        List<List<int>> scc = _graph.SplitSCC();

        List<List<int>> cycles = new();
        Graph<T> tree = new(_vertexCount);

        for (int i = 0; i < scc.Count; i++)
        {
            if (scc[i].Count == 1 && _adjList[scc[i][0]][0].To != scc[i][0])
            {
                // part of the trees
                int u = scc[i][0];
                tree.AddEdge(u, _adjList[u][0].To, _adjList[u][0].Weight);
            }
            else
            {
                // cycle
                if (sortCycle)
                {
                    List<int> sorted = new(scc[i].Count);
                    sorted.Add(scc[i][0]);
                    for (int j = 1; j < scc[i].Count; j++)
                    {
                        sorted.Add(_adjList[sorted[^1]][0].To);
                    }

                    cycles.Add(sorted);
                }
                else
                {
                    cycles.Add(scc[i]);
                }
            }
        }

        return (cycles, tree);
    }

    public override void AddEdge(int a, int b, T weight)
    {
        _graph.AddEdge(a, b, weight);
    }
}

public sealed class DfsTimeLabel
{
    private int[] _in;
    private int[] _out;

    public DfsTimeLabel(int[] @in, int[] @out)
    {
        _in = @in;
        _out = @out;
    }

    public int GetInTime(int n)
    {
        return _in[n];
    }

    public int GetOutTime(int n)
    {
        return _out[n];
    }
}