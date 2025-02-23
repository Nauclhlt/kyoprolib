public sealed class ZeroOneOnTree
{
    private sealed class CountUnionFind
    {
        private int[] _parents;
        private int[] _size;
        private int[] _min;
        private int[] _c0;
        private int[] _c1;
        private long[] _inv;
        private int _vertexCount;
        public int VertexCount => _vertexCount;
        public CountUnionFind(int n, int[] v)
        {
            _vertexCount = n;
            _parents = new int[n];
            _size = new int[n];
            _min = new int[n];
            _c0 = new int[n];
            _c1 = new int[n];
            _inv = new long[n];
            for (int i = 0; i < n; i++)
            {
                _min[i] = i;
                _parents[i] = i;
                _size[i] = 1;
                _c0[i] = v[i] == 0 ? 1 : 0;
                _c1[i] = 1 - _c0[i];
            }
        }

        public int Root(int x)
        {
            if (_parents[x] == x)
                return x;
            return _parents[x] = Root(_parents[x]);
        }
        public int Min(int x) => _min[Root(x)];
        public int C0(int x) => _c0[Root(x)];

        public int C1(int x) => _c1[Root(x)];
        public long Inv(int x) => _inv[Root(x)];

        public void Unite(int t, int f)
        {
            int rootX = Root(t);
            int rootY = Root(f);
            if (rootX == rootY)
                return;

            long inv = _inv[rootX] + _inv[rootY] + (long)_c1[rootX] * _c0[rootY];

            int from = rootX;
            int to = rootY;
            // merge from to to
            if (_size[from] > _size[to])
            {
                (from, to) = (to, from);
            }

            _size[to] += _size[from];
            _c0[to] += _c0[from];
            _c1[to] += _c1[from];
            _inv[to] = inv;
            _min[to] = int.Min(_min[to], _min[from]);
            _parents[from] = to;
        }
        public int Size(int v) => _size[Root(v)];
    }
    private int[] _p;
    private int _n;
    private int[] _v;

    public int N => _n;
    public int[] P => _p;
    public int[] V => _v;

    public ZeroOneOnTree(int n, int[] p, int[] v)
    {
        _n = n;
        _p = p;
        _v = v;
    }

    public long CalcMinInversion()
    {
        CountUnionFind uf = new (_n, _v);
        
        PriorityQueue<(int n, int size), double> pq = new(ReverseComparer<double>.Default);
        for (int i = 1; i < N; i++)
        {
            if (V[i] == 0)
                pq.Enqueue((i, 1), double.PositiveInfinity);
            else
                pq.Enqueue((i, 1), 0);
        }

        while (uf.Size(0) < N)
        {
            (int n, int size) = pq.Dequeue();
            if (uf.Size(n) != size) continue;

            int p = P[uf.Min(n)];
            uf.Unite(p, uf.Min(n));
            
            if (uf.Min(p) != 0)
            {
                pq.Enqueue((uf.Min(p), uf.Size(p)), (double)uf.C0(p) / (double)uf.C1(p));
            }
        }

        return uf.Inv(0);
    }
}