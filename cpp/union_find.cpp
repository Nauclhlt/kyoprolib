class UnionFind
{
private:
    vector<int> _parents;
    vector<int> _size;
    int _vertexCount;

public:
    UnionFind(int n)
    {
        _vertexCount = n;
        _parents.resize(n);
        _size.resize(n);
        for (int i = 0; i < n; i++)
        {
            _size[i] = 1;
            _parents[i] = i;
        }
    }

    int Root(int x)
    {
        if (_parents[x] == x) return x;
        
        return _parents[x] = Root(_parents[x]);
    }

    int Size(int x)
    {
        return _size[Root(x)];
    }

    void Unite(int x, int y)
    {
        int rootX = Root(x);
        int rootY = Root(y);

        if (rootX == rootY) return;

        int from = rootX;
        int to = rootY;

        if (_size[from] > _size[to])
        {
            swap(from, to);
        }

        _size[to] += _size[from];
        _parents[from] = to;
    }

    vector<int> Find(int x)
    {
        int root = Root(x);
        vector<int> set;
        for (int i = 0; i < _vertexCount; i++)
        {
            if (Root(i) == root)
            {
                set.push_back(i);
            }
        }

        return set;
    }

    unordered_map<int, vector<int>> FindAll()
    {
        unordered_map<int, vector<int>> sets;
        for (int i = 0; i < _vertexCount; i++)
        {
            int root = Root(i);
            if (sets.contains(root))
            {
                sets[root].push_back(i);
            }
            else
            {
                sets.emplace(root, vector<int>());
                sets[root].push_back(i);
            }
        }

        return sets;
    }

    bool Same(int x, int y)
    {
        int rootX = Root(x);
        int rootY = Root(y);
        return rootX == rootY;
    }

    void Clear()
    {
        for (int i = 0; i < _vertexCount; i++)
        {
            _parents[i] = i;
            _size[i] = 1;
        }
    }

    int VertexCount()
    {
        return _vertexCount;
    }
};