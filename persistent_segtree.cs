// 遅延セグメント木. Applyが更新後のセグ木を返す.
// 各操作O(logN)
//public delegate T Monoid<T>(T a, T b);
//public delegate T Apply<T, M>(T x, M m, int len);
public sealed class PersistentSegmentTree<T>
{
    private sealed class Node
    {
        public T Data { get; set; }
        public Node LeftNode{ get; set; }
        public Node RightNode{ get; set; }

        public Node(T data)
        {
            Data = data;
            LeftNode = null;
            RightNode = null;
        }
    }

    private int _treeSize;
    private int _size;
    private Monoid<T> _operator;
    private Monoid<T> _apply;
    private T _identity;
    private List<Node> _snapshots;

    public int Size => _size;
    public int TreeSize => _treeSize;
    public T Identity => _identity;

    public PersistentSegmentTree(int n, Monoid<T> op, Monoid<T> apply, T identity)
    {
        _size = n;
        _treeSize = 2 * _size - 1;

        _identity = identity;
        _operator = op;
        _apply = apply;

        _snapshots = new();
    }

    public int Build(T[] array)
    {
        if (_size != array.Length)
        {
            throw new InvalidOperationException("Size of the specified array does not match with the data size passed in the constructor.");
        }

        return RegisterNode(BuildRange(0, array.Length, array));
    }

    public int BuildClear(T value)
    {
        return RegisterNode(BuildClearRange(0, _size, value));
    }

    private Node BuildClearRange(int l, int r, T value)
    {
        if (l + 1 >= r) return new Node(value);
        else return MergeNode(BuildClearRange(l, (l + r) / 2, value), BuildClearRange((l + r) / 2, r, value));
    }

    private Node BuildRange(int l, int r, T[] array)
    {
        if (l + 1 >= r) return new Node(array[l]);
        else return MergeNode(BuildRange(l, (l + r) / 2, array), BuildRange((l + r) / 2, r, array));
    }

    private Node MergeNode(Node l, Node r)
    {
        Node res = new (_operator(l.Data, r.Data));
        res.LeftNode = l;
        res.RightNode = r;
        return res;
    }

    public void ClearSnapshots()
    {
        _snapshots.Clear();
    }

    private int RegisterNode(Node node)
    {
        _snapshots.Add(node);
        return _snapshots.Count - 1;
    }

    private Node GetRootAt(int time)
    {
        return _snapshots[time];
    }

    public int Apply(int time, int index, T value)
    {
        return RegisterNode(ApplyRec(index, value, GetRootAt(time), 0, _size));
    }

    private Node ApplyRec(int index, T value, Node node, int l, int r)
    {
        if (r <= index || index + 1 <= l)
        {
            return node;
        }
        else if (index <= l && r <= index + 1)
        {
            return new Node(_apply(node.Data, value));
        }
        else
        {
            return MergeNode(ApplyRec(index, value, node.LeftNode, l, (l + r) / 2), ApplyRec(index, value, node.RightNode, (l + r) / 2, r));
        }
    }

    public T Query(int time, int left, int right)
    {
        return QueryRec(left, right, GetRootAt(time), 0, _size);
    }

    private T QueryRec(int left, int right, Node node, int l, int r)
    {
        if (r <= left || right <= l)
        {
            return _identity;
        }
        else if (left <= l && r <= right)
        {
            return node.Data;
        }
        else
        {
            return _operator(QueryRec(left, right, node.LeftNode, l, (l + r) / 2), QueryRec(left, right, node.RightNode, (l + r) / 2, r));
        }
    }

    public T GetByIndex(int time, int index)
    {
        Node current = GetRootAt(time);
        int l = 0;
        int r = _size;
        while (true)
        {
            if (l == index && l + 1 == r) return current.Data;

            if (index < (l + r) / 2)
            {
                r = (l + r) / 2;
                current = current.LeftNode;
            }
            else
            {
                l = (l + r) / 2;
                current = current.RightNode;
            }
        }
    }
}