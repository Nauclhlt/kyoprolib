public sealed class CostFlowGraph<T, C> where T : struct, INumber<T>, IMinMaxValue<T> where C : struct, INumber<C>, IMinMaxValue<C>
{
    public readonly struct CostFlowEdge
    {
        public readonly int From;
        public readonly int To;
        public readonly int Number;
        public readonly T Capacity;
        public readonly C Cost;

        public CostFlowEdge(int from, int to, T capacity, C cost, int number)
        {
            From = from;
            To = to;
            Number = number;
            Capacity = capacity;
            Cost = cost;
        }
    }

    private int _n;
    private T _maxCapacity;
    private List<CostFlowEdge>[] _graph;
    private List<CostFlowEdge> _edges;
    private List<int>[] _buckets;

    public int VertexCount => _n;

    public CostFlowGraph(int n)
    {
        _edges = new();
        _graph = new List<CostFlowEdge>[n];
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

    public void AddEdge(int from, int to, T capacity, C cost)
    {
        int num = _edges.Count * 2;
        CostFlowEdge edge = new(from, to, capacity, cost, num);
        CostFlowEdge rev = new(to, from, T.Zero, -cost, num + 1);

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

    public (C mincost, List<CostFlowEdge> edges) MinCostFlowWithEdges(int source, int sink, T sourceFlow, MinCostFlowAlgo algo = MinCostFlowAlgo.DijkstraPrimalDual)
    {
        if (algo == MinCostFlowAlgo.DijkstraPrimalDual)
            return InternalMinCostFlowDijkstraPrimalDual(source, sink, sourceFlow);
        else if (algo == MinCostFlowAlgo.BellmanFordDijkstraPrimalDual)
            return InternalMinCostFlowBellmanFordDijkstraPrimalDual(source, sink, sourceFlow);
        return (C.MaxValue, null);
    }

    private (C mincost, List<CostFlowEdge> edges) InternalMinCostFlowDijkstraPrimalDual(int source, int sink, T sourceFlow)
    {
        int E = _edges.Count;
        T[] remain = new T[E << 1];
        for (int i = 0; i < E; i++)
        {
            remain[i << 1] = _edges[i].Capacity;
            remain[(i << 1) + 1] = T.Zero;
        }

        C[] potential = new C[_n];
        C[] dist = new C[_n];
        bool[] seen = new bool[_n];
        CostFlowEdge[] prev = new CostFlowEdge[_n];
        PriorityQueue<int, C> pq = new();

        // initial cost dijkstra
        {
            pq.Enqueue(source, C.Zero);
            Array.Fill(dist, C.MaxValue);
            dist[source] = C.Zero;

            while (pq.Count > 0)
            {
                int n = pq.Dequeue();

                if (seen[n]) continue;

                seen[n] = true;

                for (int i = 0; i < _graph[n].Count; i++)
                {
                    if (_graph[n][i].Cost >= C.Zero)
                    {
                        C w = dist[n] + _graph[n][i].Cost;
                        if (w < dist[_graph[n][i].To])
                        {
                            dist[_graph[n][i].To] = w;
                            pq.Enqueue(_graph[n][i].To, w);
                        }
                    }
                }
            }

            Array.Copy(dist, potential, dist.Length);
        }

        C mincost = C.Zero;

        while (sourceFlow > T.Zero)
        {
            // find min-cost path (dijkstra)
            Array.Fill(dist, C.MaxValue);
            Array.Clear(seen);
            pq.Enqueue(source, C.Zero);
            dist[source] = C.Zero;

            while (pq.Count > 0)
            {
                int n = pq.Dequeue();

                if (seen[n]) continue;

                seen[n] = true;

                for (int i = 0; i < _graph[n].Count; i++)
                {
                    if (remain[_graph[n][i].Number] > T.Zero)
                    {
                        CostFlowEdge e = _graph[n][i];
                        C w = dist[n] + e.Cost + potential[n] - potential[e.To];
                        if (w < dist[e.To])
                        {
                            dist[e.To] = w;
                            prev[e.To] = e;
                            pq.Enqueue(e.To, w);
                        }
                    }
                }
            }

            if (dist[sink] == C.MaxValue)
            {
                return (C.MaxValue, null);
            }

            // restore path
            T flow = T.MaxValue;
            int current = sink;
            while (true)
            {
                CostFlowEdge e = prev[current];
                flow = T.Min(remain[e.Number], flow);
                current = e.From;
                if (e.From == source) break;
            }
            flow = T.Min(flow, sourceFlow);
            current = sink;
            C cost = C.Zero;
            while (true)
            {
                CostFlowEdge e = prev[current];
                remain[e.Number] -= flow;
                remain[ReverseNumber(e.Number)] += flow;
                cost += e.Cost;
                current = e.From;
                if (e.From == source) break;
            }

            sourceFlow -= flow;
            mincost += C.CreateChecked(flow) * cost;

            for (int i = 0; i < _n; i++)
            {
                if (dist[i] == C.MaxValue) continue;
                potential[i] += dist[i];
            }
        }

        List<CostFlowEdge> edges = new(E);
        for (int i = 0; i < E; i++)
        {
            edges.Add(new CostFlowEdge(_edges[i].From, _edges[i].To, remain[ReverseNumber(_edges[i].Number)], _edges[i].Cost, _edges[i].Number));
        }

        return (mincost, edges);
    }

    private (C mincost, List<CostFlowEdge> edges) InternalMinCostFlowBellmanFordDijkstraPrimalDual(int source, int sink, T sourceFlow)
    {
        int E = _edges.Count;
        T[] remain = new T[E << 1];
        for (int i = 0; i < E; i++)
        {
            remain[i << 1] = _edges[i].Capacity;
            remain[(i << 1) + 1] = T.Zero;
        }

        C[] potential = new C[_n];
        C[] dist = new C[_n];
        bool[] seen = new bool[_n];
        CostFlowEdge[] prev = new CostFlowEdge[_n];
        PriorityQueue<int, C> pq = new();

        // initial cost bellman-ford
        {
            Array.Fill(dist, C.MaxValue);
            dist[source] = C.Zero;
            bool negativeCycle = false;
            for (int i = 0; i < _n; i++)
            {
                for (int j = 0; j < _edges.Count; j++)
                {
                    CostFlowEdge e = _edges[j];
                    if (dist[e.From] != C.MaxValue && dist[e.From] + e.Cost < dist[e.To])
                    {
                        dist[e.To] = dist[e.From] + e.Cost;
                        if (i == _n - 1) negativeCycle = true;
                    }
                }
            }

            if (negativeCycle)
            {
                return (C.MaxValue, null);
            }

            Array.Copy(dist, potential, dist.Length);
        }

        C mincost = C.Zero;

        while (sourceFlow > T.Zero)
        {
            // find min-cost path (dijkstra)
            Array.Fill(dist, C.MaxValue);
            Array.Clear(seen);
            pq.Enqueue(source, C.Zero);
            dist[source] = C.Zero;

            while (pq.Count > 0)
            {
                int n = pq.Dequeue();

                if (seen[n]) continue;

                seen[n] = true;

                for (int i = 0; i < _graph[n].Count; i++)
                {
                    if (remain[_graph[n][i].Number] > T.Zero)
                    {
                        CostFlowEdge e = _graph[n][i];
                        C w = dist[n] + e.Cost + potential[n] - potential[e.To];
                        if (w < dist[e.To])
                        {
                            dist[e.To] = w;
                            prev[e.To] = e;
                            pq.Enqueue(e.To, w);
                        }
                    }
                }
            }

            if (dist[sink] == C.MaxValue)
            {
                return (C.MaxValue, null);
            }

            // restore path
            T flow = T.MaxValue;
            int current = sink;
            while (true)
            {
                CostFlowEdge e = prev[current];
                flow = T.Min(remain[e.Number], flow);
                current = e.From;
                if (e.From == source) break;
            }
            flow = T.Min(flow, sourceFlow);
            current = sink;
            C cost = C.Zero;
            while (true)
            {
                CostFlowEdge e = prev[current];
                remain[e.Number] -= flow;
                remain[ReverseNumber(e.Number)] += flow;
                cost += e.Cost;
                current = e.From;
                if (e.From == source) break;
            }

            sourceFlow -= flow;
            mincost += C.CreateChecked(flow) * cost;

            for (int i = 0; i < _n; i++)
            {
                if (dist[i] == C.MaxValue) continue;
                potential[i] += dist[i];
            }
        }

        List<CostFlowEdge> edges = new(E);
        for (int i = 0; i < E; i++)
        {
            edges.Add(new CostFlowEdge(_edges[i].From, _edges[i].To, remain[ReverseNumber(_edges[i].Number)], _edges[i].Cost, _edges[i].Number));
        }

        return (mincost, edges);
    }
}

public enum MinCostFlowAlgo
{
    DijkstraPrimalDual,
    BellmanFordDijkstraPrimalDual
}