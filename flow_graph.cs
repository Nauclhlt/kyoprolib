/// <summary>
/// 辺に容量が付いた有向グラフ。
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class FlowGraph<T> where T : struct, INumber<T>, IMinMaxValue<T>
{
    public readonly struct FlowEdge
    {
        public readonly int From;
        public readonly int To;
        public readonly int Number;
        public readonly T Capacity;

        public FlowEdge(int from, int to, T capacity, int number)
        {
            From = from;
            To = to;
            Number = number;
            Capacity = capacity;
        }
    }

    private int _n;
    private T _maxCapacity;
    private List<FlowEdge>[] _graph;
    private List<FlowEdge> _edges;
    private List<int>[] _buckets;

    public int VertexCount => _n;

    public FlowGraph(int n)
    {
        _edges = new();
        _graph = new List<FlowEdge>[n];
        for (int i = 0; i < n; i++)
        {
            _graph[i] = new();
        }
        _n = n;
        _buckets = new List<int>[2 * _n];
        for (int i = 0; i < _buckets.Length; i++)
        {
            _buckets[i] = new(_n);
        }
        _maxCapacity = T.Zero;
    }

    /// <summary>
    /// fromからtoに容量capacityの辺を追加する。計算量: O(1)
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="capacity"></param>
    public void AddEdge(int from, int to, T capacity)
    {
        int num = _edges.Count * 2;
        FlowEdge edge = new(from, to, capacity, num);
        FlowEdge rev = new(to, from, T.Zero, num + 1);

        _graph[from].Add(edge);
        _graph[to].Add(rev);
        _edges.Add(edge);

        _maxCapacity = T.Max(_maxCapacity, capacity);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int ReverseNumber(int n)
    {
        return n % 2 == 0 ? n + 1 : n - 1;
    }

    /// <summary>
    /// <para>sourceからsinkに流せる最大のフローと各辺の状態を返す。</para>
    /// <para>計算量(Dinic): O(VE^2), 二部グラフならO(V√E)</para>
    /// <para>計算量(Push-Relabel): O(V^2√E), 二部グラフの場合もこの計算量</para>
    /// </summary>
    /// <param name="source"></param>
    /// <param name="sink"></param>
    /// <param name="algo"></param>
    /// <returns></returns>
    public (T flow, List<FlowEdge> edges) MaxFlowWithEdges(int source, int sink, MaxFlowAlgo algo = MaxFlowAlgo.Dinic)
    {
        if (algo == MaxFlowAlgo.PushRelabel)
            return InternalMaxFlowPushRelabel(source, sink);
        else if (algo == MaxFlowAlgo.Dinic)
            return InternalMaxFlowDinic(source, sink);
        else if (algo == MaxFlowAlgo.CostScalingDinic)
            return InternalMaxFlowCostScalingDinic(source, sink);
        return (T.Zero, null);
    }

    /// <summary>
    /// <para>sourceからsinkに流せる最大のフローを返す。</para>
    /// <para>計算量(Dinic): O(VE^2), 二部グラフならO(V√E)</para>
    /// <para>計算量(Push-Relabel): O(V^2√E), 二部グラフの場合もこの計算量</para>
    /// </summary>
    /// <param name="source"></param>
    /// <param name="sink"></param>
    /// <param name="algo"></param>
    /// <returns></returns>
    public T MaxFlow(int source, int sink, MaxFlowAlgo algo = MaxFlowAlgo.Dinic)
    {
        return MaxFlowWithEdges(source, sink, algo: algo).flow;
    }

    /// <summary>
    /// <para>sourceからsinkへの最小カットを返す。</para>
    /// <para>計算量(Dinic): O(VE^2), 二部グラフならO(V√E)</para>
    /// <para>計算量(Push-Relabel): O(V^2√E), 二部グラフの場合もこの計算量</para>
    /// </summary>
    /// <param name="source"></param>
    /// <param name="sink"></param>
    /// <param name="algo"></param>
    /// <returns></returns>
    public (T mincut, bool[] sets, List<FlowEdge> edges) MinCut(int source, int sink, MaxFlowAlgo algo = MaxFlowAlgo.Dinic)
    {
        (T maxflow, List<FlowEdge> flowEdges) = MaxFlowWithEdges(source, sink, algo: algo);

        bool[] s = new bool[_n];
        bool[] seen = new bool[_n];
        Queue<int> queue = new();
        queue.Enqueue(source);

        while (queue.Count > 0)
        {
            int n = queue.Dequeue();

            if (seen[n]) continue;

            seen[n] = true;
            s[n] = true;

            for (int i = 0; i < _graph[n].Count; i++)
            {
                FlowEdge e = _graph[n][i];
                if ((e.Number & 1) == 0)
                {
                    if ( e.Capacity - flowEdges[e.Number >> 1].Capacity > T.Zero )
                    {
                        queue.Enqueue(e.To);
                    }
                }
                else
                {
                    if (flowEdges[ReverseNumber(e.Number) >> 1].Capacity > T.Zero)
                    {
                        queue.Enqueue(e.To);
                    }
                }
            }
        }

        List<FlowEdge> edges = new();
        for (int i = 0; i < flowEdges.Count; i++)
        {
            if (s[flowEdges[i].From] && !s[flowEdges[i].To])
            {
                edges.Add(flowEdges[i]);
            }
        }

        return (maxflow, s, edges);
    }

    private (T flow, List<FlowEdge> edges) InternalMaxFlowCostScalingDinic(int source, int sink)
    {
        if (_maxCapacity == T.Zero) return (T.Zero, null);

        int E = _edges.Count;
        T[] remain = new T[E << 1];
        for (int i = 0; i < E; i++)
        {
            remain[i << 1] = _edges[i].Capacity;
            remain[(i << 1) + 1] = T.Zero;
        }

        int[] level = new int[_n];
        FlowEdge[] prev = new FlowEdge[_n];
        int[] currentEdge = new int[_n];
        bool bfs(T b)
        {
            Queue<int> q = new();
            Array.Fill(level, -1);
            level[source] = 0;
            q.Enqueue(source);
            while (q.Count > 0)
            {
                int n = q.Dequeue();
                
                for (int i = 0; i < _graph[n].Count; i++)
                {
                    if (level[_graph[n][i].To] < 0 && remain[_graph[n][i].Number] >= b)
                    {
                        level[_graph[n][i].To] = level[n] + 1;
                        q.Enqueue(_graph[n][i].To);
                    }
                }
            }

            return level[sink] >= 0;
        }

        T dfs(int v, T b, T flow)
        {
            if (v == sink) return flow;
            T sum = T.Zero;
            for (int i = currentEdge[v]; i < _graph[v].Count; i++)
            {
                currentEdge[v] = i;
                FlowEdge edge = _graph[v][i];
                if (remain[edge.Number] >= b && level[edge.From] < level[edge.To])
                {
                    T f = dfs(edge.To, b, T.Min(flow - sum, remain[edge.Number]));
                    if (f > T.Zero)
                    {
                        remain[edge.Number] -= f;
                        remain[ReverseNumber(edge.Number)] += f;
                        sum += f;
                        if (flow - sum < b) break;
                    }
                }
            }

            return sum;
        }

        T maxflow = T.Zero;
        T cur = T.One;
        T two = T.CreateChecked(2);
        while (cur < _maxCapacity) cur *= two;

        while (true)
        {
            while (bfs(cur))
            {
                Array.Clear(currentEdge);
                maxflow += dfs(source, cur, T.MaxValue);
            }
            if (cur == T.One) break;
            cur /= two;
        }

        List<FlowEdge> edges = new(_edges.Count);
        for (int i = 0; i < _edges.Count; i++)
        {
            edges.Add(new (_edges[i].From, _edges[i].To, remain[ReverseNumber(_edges[i].Number)], i));
        }

        return (maxflow, edges);
    }

    private (T flow, List<FlowEdge> edges) InternalMaxFlowDinic(int source, int sink)
    {
        int E = _edges.Count;
        T[] remain = new T[E << 1];
        for (int i = 0; i < E; i++)
        {
            remain[i << 1] = _edges[i].Capacity;
            remain[(i << 1) + 1] = T.Zero;
        }

        int[] level = new int[_n];
        FlowEdge[] prev = new FlowEdge[_n];
        int[] currentEdge = new int[_n];
        bool bfs()
        {
            Queue<int> q = new();
            level.AsSpan().Fill(-1);
            level[source] = 0;
            q.Enqueue(source);
            while (q.Count > 0)
            {
                int n = q.Dequeue();
                
                for (int i = 0; i < _graph[n].Count; i++)
                {
                    if (level[_graph[n][i].To] < 0 && remain[_graph[n][i].Number] > T.Zero)
                    {
                        level[_graph[n][i].To] = level[n] + 1;
                        q.Enqueue(_graph[n][i].To);
                    }
                }
            }

            return level[sink] >= 0;
        }

        T dfs(int v, T flow)
        {
            if (v == sink) return flow;
            for (int i = currentEdge[v]; i < _graph[v].Count; i++)
            {
                currentEdge[v] = i;
                FlowEdge edge = _graph[v][i];
                if (remain[edge.Number] > T.Zero && level[edge.From] < level[edge.To])
                {
                    T f = dfs(edge.To, T.Min(flow, remain[edge.Number]));
                    if (f > T.Zero)
                    {
                        remain[edge.Number] -= f;
                        remain[ReverseNumber(edge.Number)] += f;
                        return f;
                    }
                }
            }

            return T.Zero;
        }

        T maxflow = T.Zero;
        while (bfs())
        {
            Array.Clear(currentEdge);
            while (true)
            {
                T f = dfs(source, T.MaxValue);
                if (f == T.Zero) break;
                maxflow += f;
            }
        }

        List<FlowEdge> edges = new(_edges.Count);
        Span<FlowEdge> span = CollectionsMarshal.AsSpan(_edges);
        for (int i = 0; i < span.Length; i++)
        {
            ref FlowEdge e = ref span[i];
            edges.Add(new (e.From, e.To, remain[ReverseNumber(e.Number)], i));
        }

        return (maxflow, edges);
    }

    private (T flow, List<FlowEdge> edges) InternalMaxFlowPushRelabel(int source, int sink)
    {
        int E = _edges.Count;
        T[] remain = new T[2 * E];
        for (int i = 0; i < E; i++)
        {
            remain[2 * i] = _edges[i].Capacity;
            remain[2 * i + 1] = T.Zero;
        }
        T[] excessFlow = new T[_n];
        int[] height = new int[_n];

        void push(FlowEdge edge)
        {
            T flow = T.Min(excessFlow[edge.From], remain[edge.Number]);
            remain[edge.Number] -= flow;
            remain[ReverseNumber(edge.Number)] += flow;
            excessFlow[edge.From] -= flow;
            excessFlow[edge.To] += flow;
        }

        void relabel(int v, int bucketIndex)
        {
            int min = int.MaxValue;
            for (int i = 0; i < _graph[v].Count; i++)
            {
                if (remain[_graph[v][i].Number] > T.Zero)
                {
                    min = int.Min(height[_graph[v][i].To], min);
                }
            }

            if (min == int.MaxValue) return;
            _buckets[height[v]][bucketIndex] = _buckets[height[v]][^1];
            _buckets[height[v]].RemoveAt(_buckets[height[v]].Count - 1);
            height[v] = min + 1;
            _buckets[height[v]].Add(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool isAdmissible(FlowEdge edge)
        {
            return height[edge.From] == height[edge.To] + 1 && remain[edge.Number] > T.Zero;
        }

        height[source] = _n;
        excessFlow[source] = T.MaxValue;

        // global labeling
        {
            Queue<(int, int)> queue = new();
            bool[] seen = new bool[_n];
            queue.Enqueue((sink, 0));
            Array.Fill(height, _n);
            while (queue.Count > 0)
            {
                (int n, int c) = queue.Dequeue();

                if (seen[n]) continue;
                seen[n] = true;
                if (n != source)
                {
                    height[n] = c;
                }

                for (int i = 0; i < _graph[n].Count; i++)
                {
                    queue.Enqueue((_graph[n][i].To, c + 1));
                }
            }

            for (int i = 0; i < _buckets.Length; i++)
            {
                _buckets[i].Clear();
            }

            for (int i = 0; i < _n; i++)
            {
                if (i != source && i != sink)
                    _buckets[height[i]].Add(i);
            }
        }
        

        for (int i = 0; i < _graph[source].Count; i++)
        {
            push(_graph[source][i]);
        }

        int[] current = new int[_n];
        while (true)
        {
            // find active node
            int activeNode = -1;
            int activeNodeBucketIndex = -1;
            for (int k = _buckets.Length - 1; k >= 0; k--)
            {
                for (int i = _buckets[k].Count - 1; i >= 0; i--)
                {
                    if (excessFlow[_buckets[k][i]] > T.Zero && height[_buckets[k][i]] < _n)
                    {
                        activeNode = _buckets[k][i];
                        activeNodeBucketIndex = i;
                        break;
                    }
                }
                if (activeNode != -1) break;
            }
            
            if (activeNode == -1) break;

            // find admissible edge
            FlowEdge edge = new(-1, -1, T.Zero, -1);
            for (int i = current[activeNode]; i < _graph[activeNode].Count; i++)
            {
                current[activeNode] = i;
                if (isAdmissible(_graph[activeNode][i]))
                {
                    edge = _graph[activeNode][i];
                    break;
                }
            }

            if (edge.Number != -1)
            {
                T flow = T.Min(excessFlow[edge.From], remain[edge.Number]);
                remain[edge.Number] -= flow;
                remain[ReverseNumber(edge.Number)] += flow;
                excessFlow[edge.From] -= flow;
                excessFlow[edge.To] += flow;
            }
            else
            {
                relabel(activeNode, activeNodeBucketIndex);
                current[activeNode] = 0;
                bool zero = false;
                for (int k = 1; k < _n; k++)
                {
                    if (_buckets[k].Count == 0) zero = true;
                    else if (zero)
                    {
                        for (int i = 0; i < _buckets[k].Count; i++)
                        {
                            height[_buckets[k][i]] = _n;
                            current[_buckets[k][i]] = 0;
                            _buckets[_n].Add(_buckets[k][i]);
                        }
                        _buckets[k].Clear();
                    }
                }
            }
        }

        T maxflow = T.Zero;
        for (int i = 0; i < _graph[source].Count; i++)
        {
            maxflow += remain[ReverseNumber(_graph[source][i].Number)];
        }

        List<FlowEdge> edges = new(_edges.Count);
        for (int i = 0; i < _edges.Count; i++)
        {
            edges.Add(new (_edges[i].From, _edges[i].To, remain[ReverseNumber(_edges[i].Number)], i));
        }

        return (maxflow, edges);
    }
}

public enum MaxFlowAlgo
{
    PushRelabel,
    Dinic,
    CostScalingDinic
}