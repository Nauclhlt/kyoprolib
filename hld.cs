/// <summary>
/// HL分解(Heavy-Light Decomposition)でパスクエリや更新を処理する。
/// </summary>
/// <typeparam name="TV"></typeparam>
/// <typeparam name="TE"></typeparam>
public sealed class HeavyLightDecomposition<TV, TE> where TV : struct where TE : struct
{
    private int _n;
    private TV[] _v;
    private List<List<HLEdge<TE>>> _graph;
    private List<HLEdge<TE>> _edges;
    // HL分解後の頂点番号の列
    private List<int> _hld;
    // 頂点の値を_hldの順番に並べた列
    private List<TV> _vertexArray;
    // 頂点と辺の値を_hldの順番に並べた列。頂点の部分には_edgeIdentityが入っている。
    private List<TE> _vertexEdgeArray;
    // 頂点クエリのモノイドの演算
    private Monoid<TV> _vmonoid;
    // 辺クエリのモノイドの演算
    private Monoid<TE> _emonoid;
    // 頂点クエリのモノイドの単位元
    private TV _vertexIdentity;
    // 辺クエリのモノイドの単位元
    private TE _edgeIdentity;
    // 各頂点が_vertexArrayのどこにあるか
    private int[] _vertexPositions;
    // 各頂点が_vertexEdgeArrayのどこにあるか
    private int[] _edgeVertexPositions;
    // 各辺が_vertexEdgeArrayのどこにあるか(lightな辺は-1)
    private int[] _edgePositions;
    // 各頂点が属する成分のうち一番浅いもの
    private int[] _root;
    // 各頂点を根とする部分木の頂点数
    private int[] _size;
    // 各頂点の親
    private int[] _parent;
    // 各頂点とその親をつなぐ辺の番号
    private int[] _parentEdge;
    // 各頂点の深さ
    private int[] _depth;
    private SegmentTree<TV> _vertexSeg;
    private SegmentTree<TE> _edgeSeg;

    public HeavyLightDecomposition( int n, List<List<HLEdge<TE>>> graph, List<HLEdge<TE>> edges, 
                                    TV[] v, Monoid<TV> vmonoid, TV vertexIdentity, Monoid<TE> emonoid, 
                                    TE edgeIdentity )
    {
        _n = n;
        _graph = graph;
        _edges = edges;
        _v = v;
        _vmonoid = vmonoid;
        _emonoid = emonoid;
        _vertexArray = new(_n);
        _vertexEdgeArray = new(_n);
        _vertexPositions = new int[_n];
        _edgeVertexPositions = new int[_n];
        _edgePositions = new int[_edges.Count];
        Array.Fill(_edgePositions, -1);
        _hld = new(_n);
        _root = new int[_n];
        _edgeIdentity = edgeIdentity;
        _vertexIdentity = vertexIdentity;
        _size = new int[_n];
        _parent = new int[_n];
        _parentEdge = new int[_n];
        _depth = new int[_n];

        RecursiveFreeSizeParent();
        RecursiveFreeHLD();

        _vertexSeg = new(_n, _vmonoid, (x, a) => a, _vertexIdentity);
        _vertexSeg.Build(_vertexArray.ToArray());
        _edgeSeg = new(_vertexEdgeArray.Count, _emonoid, (x, a) => a, _edgeIdentity);
        _edgeSeg.Build(_vertexEdgeArray.ToArray());
    }

    private void RecursiveFreeSizeParent()
    {
        Stack<(int, int, int, HLEdge<TE>)> stack = new();
        Stack<(int, int)> backstack = new();
        stack.Push((0, -1, 0, new (-1, -1, -1, _edgeIdentity)));

        while (stack.Count > 0)
        {
            (int v, int prev, int depth, HLEdge<TE> edge) = stack.Pop();

            if (edge.From != -1)
            {
                _parentEdge[v] = edge.Number;
            }
            _parent[v] = prev;
            _depth[v] = depth;
            _size[v] = 1;

            for (int i = 0; i < _graph[v].Count; i++)
            {
                if(_graph[v][i].To != prev)
                {
                    stack.Push((_graph[v][i].To, v, depth + 1, _graph[v][i]));
                    backstack.Push((_graph[v][i].To, v));
                }
            }
        }

        while (backstack.Count > 0)
        {
            (int from, int to) = backstack.Pop();
            _size[to] += _size[from];
        }
    }

    private void RecursiveFreeHLD()
    {
        Stack<(int, int, HLEdge<TE>)> stack = new();
        stack.Push((0, 0, new(-1, -1, -1, _edgeIdentity)));

        while (stack.Count > 0)
        {
            (int v, int root, HLEdge<TE> edge) = stack.Pop();
            if (edge.From != -1)
            {
                _edgePositions[edge.Number] = _vertexEdgeArray.Count;
                _vertexEdgeArray.Add(edge.Weight);
            }
            _vertexPositions[v] = _vertexArray.Count;
            _vertexArray.Add(_v[v]);
            _edgeVertexPositions[v] = _vertexEdgeArray.Count;
            _vertexEdgeArray.Add(_edgeIdentity);

            _hld.Add(v);
            _root[v] = root;

            if (_graph[v].Count == 0 || (_graph[v].Count == 1 && _depth[_graph[v][0].To] < _depth[v])) 
            {
                continue;
            }

            int max = 0;
            int idx = -1;
            for (int i = 0; i < _graph[v].Count; i++)
            {
                if (_depth[_graph[v][i].To] < _depth[v]) continue;
                int size = _size[_graph[v][i].To];
                if (size > max)
                {
                    max = size;
                    idx = i;
                }
            }

            for (int i = 0; i < _graph[v].Count; i++)
            {
                if (_depth[_graph[v][i].To] < _depth[v]) continue;
                if (i != idx)
                {
                    stack.Push((_graph[v][i].To, _graph[v][i].To, new (-1, -1, -1, _edgeIdentity)));
                }
            }

            stack.Push((_graph[v][idx].To, root, _graph[v][idx]));
        }
    }

    /// <summary>
    /// 頂点の値を更新する。計算量: O(logV)
    /// </summary>
    /// <param name="v"></param>
    /// <param name="value"></param>
    public void SetVertex(int v, TV value)
    {
        _v[v] = value;
        _vertexSeg.Apply(_vertexPositions[v], value);
    }

    /// <summary>
    /// 辺の値を更新する。計算量: O(V+E)
    /// </summary>
    /// <param name="e"></param>
    /// <param name="value"></param>
    public void SetEdge(int e, TE value)
    {
        _edges[e] = new(_edges[e].From, _edges[e].To, _edges[e].Number, value);
        if (_edgePositions[e] != -1)
        {
            _edgeSeg.Apply(_edgePositions[e], value);
        }
    }

    /// <summary>
    /// u,v間のパスに含まれる頂点に対するクエリを処理する。計算量: O(log^2V)
    /// </summary>
    /// <param name="u"></param>
    /// <param name="v"></param>
    /// <returns></returns>
    public TV QueryVertexPath(int u, int v)
    {
        if (u == v) return _v[u];

        TV res = _vertexIdentity;

        while (_root[u] != _root[v])
        {
            if (_depth[_root[u]] <= _depth[_root[v]])
            {
                int l = _vertexPositions[_root[v]];
                int r = _vertexPositions[v] + 1;
                v = _parent[_root[v]];
                res = _vmonoid(res, _vertexSeg.Query(l, r));
            }
            else
            {
                int l = _vertexPositions[_root[u]];
                int r = _vertexPositions[u] + 1;
                u = _parent[_root[u]];
                res = _vmonoid(res, _vertexSeg.Query(l, r));
            }
        }

        {
            int l = int.Min(_vertexPositions[u], _vertexPositions[v]);
            int r = int.Max(_vertexPositions[u], _vertexPositions[v]) + 1;
            res = _vmonoid(res, _vertexSeg.Query(l, r));
        }

        return res;
    }

    /// <summary>
    /// u,v間のパスに含まれる辺に対するクエリを処理する。計算量: O(log^2(V+E))
    /// </summary>
    /// <param name="u"></param>
    /// <param name="v"></param>
    /// <returns></returns>
    public TE QueryEdgePath(int u, int v)
    {
        if (u == v) return _edgeIdentity;

        TE res = _edgeIdentity;

        while (_root[u] != _root[v])
        {
            if (_depth[_root[u]] <= _depth[_root[v]])
            {
                int l = _edgeVertexPositions[_root[v]];
                int r = _edgeVertexPositions[v] + 1;
                
                res = _emonoid(res, _edgeSeg.Query(l, r));
                res = _emonoid(res, _edges[_parentEdge[_root[v]]].Weight);
                v = _parent[_root[v]];
            }
            else
            {
                int l = _edgeVertexPositions[_root[u]];
                int r = _edgeVertexPositions[u] + 1;
                res = _emonoid(res, _edgeSeg.Query(l, r));
                res = _emonoid(res, _edges[_parentEdge[_root[u]]].Weight);
                u = _parent[_root[u]];
            }
        }

        {
            int l = int.Min(_edgeVertexPositions[u], _edgeVertexPositions[v]);
            int r = int.Max(_edgeVertexPositions[u], _edgeVertexPositions[v]) + 1;
            res = _emonoid(res, _edgeSeg.Query(l, r));
        }

        return res;
    }

    /// <summary>
    /// uとvのLCA(最小共通祖先)を返す。計算量: O(log^2V)
    /// </summary>
    /// <param name="u"></param>
    /// <param name="v"></param>
    /// <returns></returns>
    public int Lca(int u, int v)
    {
        if (u == v) return u;

        while (_root[u] != _root[v])
        {
            if (_depth[_root[u]] <= _depth[_root[v]])
            {
                int l = _vertexPositions[_root[v]];
                int r = _vertexPositions[v] + 1;
                v = _parent[_root[v]];
            }
            else
            {
                int l = _vertexPositions[_root[u]];
                int r = _vertexPositions[u] + 1;
                u = _parent[_root[u]];
            }
        }

        return int.Min(_vertexPositions[u], _vertexPositions[v]);
    }
}

public struct HLEdge<T> where T : struct
{
    public int From;
    public int To;
    public int Number;
    public T Weight;

    public HLEdge(int from, int to, int number, T weight)
    {
        From = from;
        To = to;
        Number = number;
        Weight = weight;
    }
}