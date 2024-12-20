template <typename T>
class Graph
{
private:
    vector<vector<Edge<T>>> _graph;
    vector<Edge<T>> _edges;
    vector<bool> _seen;
    unique_ptr<UnionFind> _uf;
    int _vertexCount;

public:
    Graph(int vertexCount)
    {
        _vertexCount = vertexCount;
        _graph.resize(_vertexCount);
        _edges.reserve(_vertexCount);
        _seen.resize(_vertexCount);
        _uf = NULL;
    }

    int VertexCount()
    {
        return _vertexCount;
    }

    vector<vector<Edge<T>>>& RawGraph()
    {
        return _graph;
    }

    vector<Edge<T>>& Edges()
    {
        return _edges;
    }

    UnionFind DSU()
    {
        return *_uf;
    }

    void AddEdge(int a, int b, T weight)
    {
        if (!Validate(a) || !Validate(b))
        {
            return;
        }

        if (a > b)
        {
            swap(a, b);
        }

        _graph[a].push_back(Edge<T>(a, b, weight));
        _graph[b].push_back(Edge<T>(b, a, weight));
        _edges.push_back(Edge<T>(a, b, weight));
    }

    void SetupDSU()
    {
        _uf.reset(new UnionFind(_vertexCount));
        for (int i = 0; i < (int)_edges.size(); i++)
        {
            _uf->Unite(_edges[i].From, _edges[i].To);
        }
    }

    bool Same(int a, int b)
    {
        if (_uf == nullptr)
        {
            throw exception();
        }
        return _uf->Same(a, b);
    }

    unordered_map<int, vector<int>> GetConnectedComponents()
    {
        if (_uf == nullptr)
        {
            throw exception();
        }
        return _uf->FindAll();
    }

    void DijkstraFrom(int n, vector<T>& map)
    {
        if (!Validate(n))
        {
            return;
        }

        fill(_seen.begin(), _seen.end(), false);

        fill(map.begin(), map.end(), numeric_limits<T>::max());
        map[n] = 0;

        priority_queue<pair<T, int>, vector<pair<T, int>>, greater<pair<T, int>>> pq;
        pq.emplace(0, n);

        while (!pq.empty())
        {
            T c = pq.top().first;
            int p = pq.top().second;
            pq.pop();

            if (_seen[p])
            {
                continue;
            }

            _seen[p] = true;

            vector<Edge<T>>& ch = _graph[p];
            for (int i = 0; i < (int)ch.size(); i++)
            {
                T w = map[p] + ch[i].Weight;
                if (w < map[ch[i].To])
                {
                    map[ch[i].To] = w;
                    pq.emplace(w, ch[i].To);
                }
            }
        }
    }

    vector<vector<T>> WarshallFloyd()
    {
        if (_vertexCount > 800)
        {
            throw exception();
        }

        T inf = numeric_limits<T>::max();

        vector<vector<T>> map(_vertexCount, vector<T>(_vertexCount, inf));

        for (int i = 0; i < _vertexCount; i++)
        {
            map[i][i] = 0;
        }

        for (int i = 0; i < (int)_edges.size(); i++)
        {
            map[_edges[i].From][_edges[i].To] = min(_edges[i].Weight, map[_edges[i].From][_edges[i].To]);
            map[_edges[i].To][_edges[i].From] = min(_edges[i].Weight, map[_edges[i].To][_edges[i].From]);
        }

        for (int k = 0; k < _vertexCount; k++)
        {
            for (int j = 0; j < _vertexCount; j++)
            {
                for (int i = 0; i < _vertexCount; i++)
                {
                    if (map[i][k] != inf && map[k][j] != inf)
                    {
                        if (map[i][k] + map[k][j] < map[i][j])
                        {
                            map[i][j] = map[i][k] + map[k][j];
                        }
                    }
                }
            }
        }

        return map;
    }

    void BfsFrom(int n, vector<T>& map)
    {
        if (!Validate(n))
        {
            return;
        }

        fill(_seen.begin(), _seen.end(), false);

        map[n] = 0;

        queue<pair<int, T>> queue;

        queue.emplace(n, 0);

        while (!queue.empty())
        {
            int p = queue.front().first;
            T w = queue.front().second;
            queue.pop();

            if (_seen[p])
                continue;

            _seen[p] = true;
            map[p] = w;

            vector<Edge<T>>& ch = _graph[p];
            for (int i = 0; i < (int)ch.size(); i++)
            {
                queue.emplace(ch[i].To, w + ch[i].Weight);
            }
        }
    }

    Graph<T> CreateComplement() 
    {
        struct pair_hash {
            inline std::size_t operator()(const std::pair<int,int> & v) const {
                return v.first ^ v.second;
            }
        };

        unordered_set<pair<int, int>, pair_hash> edgeSet;
        for (int i = 0; i < (int)_edges.size(); i++)
        {
            edgeSet.emplace(_edges[i].From, _edges[i].To);
        }

        Graph<T> g(_vertexCount);

        for (int i = 0; i < _vertexCount - 1; i++)
        {
            for (int j = i + 1; j < _vertexCount; j++)
            {
                if (edgeSet.find(make_pair(i, j)) == edgeSet.end())
                {
                    cout << i << ", " << j << endl;
                    g.AddEdge(i, j, 0);
                }
            }
        }

        return g;
    }
    
    bool IsBipartite()
    {
        fill(_seen.begin(), _seen.end(), false);

        stack<pair<int, bool>> stack;

        vector<bool> memo(_vertexCount);

        for (int i = 0; i < _vertexCount; i++)
        {
            if (_seen[i]) continue;

            stack.emplace(i, false);

            while (!stack.empty())
            {
                int n = stack.top().first;
                bool c = stack.top().second;
                stack.pop();

                if (_seen[n])
                {
                    if (memo[n] != c)
                    {
                        return false;
                    }

                    continue;
                }

                _seen[n] = true;
                memo[n] = c;

                vector<Edge<T>>& ch = _graph[n];
                for (int j = 0; j < (int)ch.size(); j++)
                {
                    stack.emplace(ch[j].To, !c);
                }
            }
        }

        return true;
    }

    T TreeDiameter()
    {
        if ((int)_edges.size() != _vertexCount - 1)
        {
            throw exception();
        }

        vector<T> dist(_vertexCount);

        BfsFrom(0, dist);

        T max = 0;
        int v = 0;
        for (int i = 0; i < _vertexCount; i++)
        {
            if (dist[i] > max)
            {
                max = dist[i];
                v = i;
            }
        }

        fill(dist.begin(), dist.end(), 0);

        BfsFrom(v, dist);

        max = 0;
        for (int i = 0; i < _vertexCount; i++)
        {
            if (dist[i] > max)
            {
                max = dist[i];
            }
        }

        return max;
    }

    T MaxSpanningTreeWeight()
    {
        UnionFind unionFind(_vertexCount);

        T ans = 0;

        auto cmp = [](const Edge<T> &a, const Edge<T> &b)
        {
            return a.Weight > b.Weight;
        };

        vector<Edge<T>>& edges = _edges;

        sort(edges.begin(), edges.end(), cmp);

        for (int i = 0; i < (int)edges.size(); i++)
        {
            if (!unionFind.Same(edges[i].From, edges[i].To))
            {
                unionFind.Unite(edges[i].From, edges[i].To);
                ans += edges[i].Weight;
            }
        }

        return ans;
    }

    T MinSpanningTreeWeight()
    {
        UnionFind unionFind(_vertexCount);

        T ans = 0;

        auto cmp = [](const Edge<T> &a, const Edge<T> &b)
        {
            return a.Weight < b.Weight;
        };

        vector<Edge<T>>& edges = _edges;

        sort(edges.begin(), edges.end(), cmp);

        for (int i = 0; i < (int)edges.size(); i++)
        {
            if (!unionFind.Same(edges[i].From, edges[i].To))
            {
                unionFind.Unite(edges[i].From, edges[i].To);
                ans += edges[i].Weight;
            }
        }

        return ans;
    }

private:
    inline bool Validate(int n)
    {
        return 0 <= n && n < _vertexCount;
    }
};