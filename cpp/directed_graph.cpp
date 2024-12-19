template <typename T>
class DirectedGraph
{
private:
    vector<vector<Edge<T>>> _graph;
    vector<vector<Edge<T>>> _reverseGraph;
    vector<Edge<T>> _edges;
    vector<Edge<T>> _reverseEdges;
    vector<bool> _seen;
    int _vertexCount;

public:
    DirectedGraph(int vertexCount)
    {
        _vertexCount = vertexCount;
        _graph.resize(_vertexCount);
        _edges.reserve(_vertexCount);
        _seen.resize(_vertexCount);
        _reverseGraph.resize(_vertexCount);
        _reverseEdges.reserve(_vertexCount);       
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

    void AddEdge(int from, int to, T weight)
    {
        if (!Validate(from) || !Validate(to))
        {
            return;
        }

        Edge<T> edge(from, to, weight);
        _graph[from].push_back(edge);
        _edges.push_back(edge);

        Edge<T> revEdge(to, from, weight);
        _reverseGraph[to].push_back(revEdge);
        _reverseEdges.push_back(revEdge);
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

            vector<Edge<T>> ch = _graph[p];
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

            vector<Edge<T>> ch = _graph[p];
            for (int i = 0; i < (int)ch.size(); i++)
            {
                queue.emplace(ch[i].To, w + ch[i].Weight);
            }
        }
    }

    bool TryTopologicalSort(vector<int>& sorted)
    {
        sorted = vector<int>();
        sorted.reserve(_vertexCount);

        vector<int> deg(_vertexCount);
        for (int i = 0; i < (int)_edges.size(); i++)
        {
            deg[_edges[i].To]++;
        }

        queue<int> q;
        for (int i = 0; i < _vertexCount; i++)
        {
            if (deg[i] == 0)
                q.push(i);
        }

        while (!q.empty())
        {
            int next = q.front();
            q.pop();

            sorted.push_back(next);

            vector<Edge<T>>& p = _graph[next];
            for (int i = 0; i < (int)p.size(); i++)
            {
                deg[p[i].To]--;
                if (deg[p[i].To] < 0)
                {
                    // detected cycle
                    return false;
                }

                if (deg[p[i].To] == 0)
                {
                    q.push(p[i].To);
                }
            }
        }

        return sorted.size() == _vertexCount;
    }

    bool TryUniqueTopologicalSort(vector<int>& sorted)
    {
        sorted = vector<int>();
        sorted.reserve(_vertexCount);

        vector<int> deg(_vertexCount);
        for (int i = 0; i < (int)_edges.size(); i++)
        {
            deg[_edges[i].To]++;
        }

        queue<int> q;
        for (int i = 0; i < _vertexCount; i++)
        {
            if (deg[i] == 0)
                q.push(i);
        }

        while (!q.empty())
        {
            if ((int)q.size() > 1)
            {
                return false;
            }

            int next = q.front();
            q.pop();

            sorted.push_back(next);

            vector<Edge<T>>& p = _graph[next];
            for (int i = 0; i < (int)p.size(); i++)
            {
                deg[p[i].To]--;
                if (deg[p[i].To] < 0)
                {
                    // detected cycle
                    return false;
                }

                if (deg[p[i].To] == 0)
                {
                    q.push(p[i].To);
                }
            }
        }

        return sorted.size() == _vertexCount;
    }

private:
    inline bool Validate(int n)
    {
        return 0 <= n && n < _vertexCount;
    }
};