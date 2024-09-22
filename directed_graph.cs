// 重み付き(重みなし)の有向グラフを管理する.
// Depends on: Edge<T>
// @author Nauclhlt.
public sealed class DirectedGraph<T> where T : struct, INumber<T>, IMinMaxValue<T>
{
    private List<List<Edge<T>>> _graph;
    private List<Edge<T>> _edges;
    private bool[] _seen;
    private int _vertexCount;
    private List<List<Edge<T>>> _reverseGraph;
    private List<Edge<T>> _reverseEdges;
    private bool _hasReverse = false;

    public int VertexCount => _vertexCount;
    public bool[] Seen => _seen;
    public List<List<Edge<T>>> RawGraph => _graph;
    public List<Edge<T>> Edges => _edges;
    public List<List<Edge<T>>> ReverseGraph => _reverseGraph;
    public List<Edge<T>> ReverseEdges => _reverseEdges;

    public DirectedGraph(int vertexCount)
        : this (vertexCount, false)
    {
        
    }

    public DirectedGraph(int vertexCount, bool hasReverse = false)
    {
        _hasReverse = hasReverse;
        _vertexCount = vertexCount;
        _graph = new(_vertexCount);
        _edges = new List<Edge<T>>(_vertexCount);
        if (_hasReverse)
        {
            _reverseGraph = new(_vertexCount);
            _reverseEdges = new(_vertexCount);
        }
        for (int i = 0; i < _vertexCount; i++)
        {
            if (_hasReverse)
                _reverseGraph.Add(new());
            _graph.Add(new());
        }
    }

    // 頂点from -> toに重みweightの有向辺を追加する.
    // O(1)
    public void AddEdge(int from, int to, T weight)
    {
        if (!Validate(from) || !Validate(to)) return;

        Edge<T> edge = new Edge<T>(from, to, weight);
        _graph[from].Add(edge);
        _edges.Add(edge);

        if (_hasReverse)
        {
            Edge<T> revEdge = new(to, from, weight);
            _reverseGraph[to].Add(revEdge);
            _reverseEdges.Add(revEdge);
        }
    }

    // 探索用の配列を作成する.
    public void SetupSearch()
    {
        _seen = new bool[_vertexCount];
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

    // Warshall-Floyd法を用いて各2頂点間の最短経路の重みの総和を求める.
    // O(V^3)
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

        for (int i = 0; i < _edges.Count; i++)
        {
            map[_edges[i].From, _edges[i].To] = T.Min(_edges[i].Weight, map[_edges[i].From, _edges[i].To]);
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

    // トポロジカルソートをする.
    // 戻り値がtrueなら成功, falseなら失敗, つまり有向閉路が含まれる.
    // O(V+E)
    public bool TryTopologicalSort(out List<int> sorted)
    {
        sorted = new List<int>(_vertexCount);

        int[] deg = new int[_vertexCount];
        for (int i = 0; i < _edges.Count; i++)
        {
            deg[_edges[i].To]++;
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

            List<Edge<T>> p = _graph[next];
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

    // Uniqueにトポロジカルソートをする.
    // 戻り値がtrueなら成功, falseなら失敗, 有向閉路が含まれるまたは順序が一通りに定まらない.
    // O(V+E)
    public bool TryUniqueTopologicalSort(out List<int> sorted)
    {
        sorted = new List<int>(_vertexCount);

        int[] deg = new int[_vertexCount];
        for (int i = 0; i < _edges.Count; i++)
        {
            deg[_edges[i].To]++;
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

            List<Edge<T>> p = _graph[next];
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

    // グラフを強連結成分分解する.
    // O(V+E)
    public List<List<int>> SplitSCC()
    {
        if (_seen is null) throw new Exception("call SetupSearch.");
        if (!_hasReverse) throw new Exception("Reverse graph required.");

        Array.Clear(_seen);

        int[] t = new int[_vertexCount];

        int seenIndex = 1;
        void dfs1(int n)
        {
            _seen[n] = true;

            var ch = _graph[n];
            for (int i = 0; i < ch.Count; i++)
            {
                if (!_seen[ch[i].To])
                    dfs1(ch[i].To);
            }

            t[n] = seenIndex;
            seenIndex++;
        }

        for (int i = 0; i < _vertexCount; i++)
        {
            if (!_seen[i])
            {
                dfs1(i);
            }
        }

        

        Array.Clear(_seen);

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
            if (_seen[p]) continue;

            List<int> list = new();

            stack.Push(p);

            while (stack.Count > 0)
            {
                int n = stack.Pop();

                if (_seen[n]) continue;

                _seen[n] = true;
                list.Add(n);

                var ch = _reverseGraph[n];
                for (int j = 0; j < ch.Count; j++)
                {
                    stack.Push(ch[j].To);
                }
            }

            res.Add(list);
        }

        return res;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool Validate(int n)
    {
        return 0 <= n && n < _vertexCount;
    }
}