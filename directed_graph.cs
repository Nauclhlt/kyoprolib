// 重み付き(重みなし)の有向グラフを管理する.
// Depends on: Edge<T>
// @author Nauclhlt.
public sealed class DirectedGraph<T> where T : struct, INumber<T>
{
    private List<List<Edge<T>>> _graph;
    private List<Edge<T>> _edges;
    private bool[] _seen;
    private int _vertexCount;

    public int VertexCount => _vertexCount;
    public bool[] Seen => _seen;
    public List<List<Edge<T>>> RawGraph => _graph;
    public List<Edge<T>> Edges => _edges;

    public DirectedGraph(int vertexCount)
    {
        _vertexCount = vertexCount;
        _graph = new(_vertexCount);
        _edges = new List<Edge<T>>(_vertexCount);
        for (int i = 0; i < _vertexCount; i++)
        {
            _graph.Add(new());
        }
    }

    public void AddEdge(int from, int to, T weight)
    {
        if (!Validate(from) || !Validate(to)) return;

        Edge<T> edge = new Edge<T>(from, to, weight);
        _graph[from].Add(edge);
        _edges.Add(edge);
    }

    public void SetupSearch()
    {
        _seen = new bool[_vertexCount];
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

    // トポロジカルソートをする.
    // 戻り地がtrueなら成功, falseなら失敗, つまり有向閉路が含まれる.
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
    // 戻り地がtrueなら成功, falseなら失敗, 有向閉路が含まれるまたは順序が一通りに定まらない.
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool Validate(int n)
    {
        return 0 <= n && n < _vertexCount;
    }
}