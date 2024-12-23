template <typename T>
class WeightedUnionFind
{
private:
    vector<int> _parents;
    vector<T> _weights;
    int _vertexCount;

public:
    WeightedUnionFind(int n)
    {
        _parents.resize(n);
        _weights.resize(n);
        _vertexCount = n;
        for (int i = 0; i < n; i++)
        {
            _parents[i] = i;
            _weights[i] = 0;
        }
    }

    int VertexCount()
    {
        return _vertexCount;
    }

    int Root(int x)
    {
        if (_parents[x] == x)
            return x;

        int root = Root(_parents[x]);
        _weights[x] += _weights[_parents[x]];
        return _parents[x] = root;
    }

    T Weight(int x)
    {
        Root(x);
        return _weights[x];
    }

    T WeightDifference(int x, int y)
    {
        return Weight(y) - Weight(x);
    }

    void Unite(int x, int y, T weight)
    {
        weight += Weight(x);
        weight -= Weight(y);

        int rootX = Root(x);
        int rootY = Root(y);
        if (rootX == rootY)
            return;
        
        _parents[rootY] = rootX;
        _weights[rootY] = weight;
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
            if (sets.find(root) != sets.end())
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
        }
    }
};