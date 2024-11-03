// 永続遅延セグメント木. Applyが更新後のセグ木を返す.
// 各操作O(logN)
//public delegate T Monoid<T>(T a, T b);
//public delegate T Apply<T, M>(T x, M m, int len);
public sealed class PersistentLazySegmentTree<T, M> where T : struct, IEquatable<T>  where M : struct, IEquatable<M>
{
    private sealed class Node
    {
        public T Data { get; set; }
        public M? Lazy { get; set; }
        public Node LeftNode { get; set; }
        public Node RightNode { get; set; }

        public Node(T data)
        {
            Data = data;
            Lazy = null;
            LeftNode = null;
            RightNode = null;
        }

        public Node(T data, M? lazy)
        {
            Data = data;
            Lazy = lazy;
            LeftNode = null;
            RightNode = null;
        }

        public void Print(int depth = 0)
        {
            if (RightNode is not null) RightNode.Print(depth + 1);
            Console.WriteLine(new string('\t', depth) + " " + Data + " (" + Lazy + ")");
            if (LeftNode is not null) LeftNode.Print(depth + 1);
        }
    }

    private int _size;
    private Monoid<T> _operator;
    private Apply<T, M> _mapping;
    private Monoid<M> _composition;
    private T _identity;
    private List<Node> _snapshots;

    public int Size => _size;
    public T Identity => _identity;

    public PersistentLazySegmentTree(int n, Monoid<T> op, Apply<T, M> mapping, Monoid<M> composition, T identity)
    {
        _size = n;

        _identity = identity;
        _operator = op;
        _mapping = mapping;
        _composition = composition;

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

    public void DebugPrint(int time)
    {
        _snapshots[time].Print();
    }

    private Node GetRootAt(int time)
    {
        return _snapshots[time];
    }

    private void Evaluate(Node node, int l, int r)
    {
        if (node.Lazy is not null)
        {
            if (r - l == 1)
            {
                node.Data = _mapping(node.Data, (M)node.Lazy, r - l);
            }
            else
            {
                node.Data = _mapping(node.Data, (M)node.Lazy, r - l);

                Node lnode = node.LeftNode;

                Node newLeft = new (lnode.Data, GuardComposition(lnode.Lazy, node.Lazy));
                newLeft.LeftNode = lnode.LeftNode;
                newLeft.RightNode = lnode.RightNode;

                node.LeftNode = newLeft;

                Node rnode = node.RightNode;
                Node newRight = new(rnode.Data, GuardComposition(rnode.Lazy, node.Lazy));
                newRight.LeftNode = rnode.LeftNode;
                newRight.RightNode = rnode.RightNode;

                node.RightNode = newRight;
            }

            node.Lazy = null;
        }
    }

    public int Apply(int time, int left, int right, M value)
    {
        return RegisterNode(ApplyRec(left, right, value, GetRootAt(time), 0, _size));
    }

    private Node ApplyRec(int left, int right, M value, Node node, int l, int r)
    {
        Evaluate(node, l, r);

        if (left <= l && r <= right)
        {
            Node n = new (node.Data, GuardComposition(node.Lazy, value));
            n.LeftNode = node.LeftNode;
            n.RightNode = node.RightNode;
            Evaluate(n, l, r);
            return n;
        }
        else if (left < r && l < right)
        {
            Node n = MergeNode(ApplyRec(left, right, value, node.LeftNode, l, (l + r) / 2), ApplyRec(left, right, value, node.RightNode, (l + r) / 2, r));
            return n;
        }
        else
        {
            return node;
        }
    }

    public T Query(int time, int left, int right)
    {
        return QueryRec(left, right, GetRootAt(time), 0, _size);
    }

    private T QueryRec(int left, int right, Node node, int l, int r)
    {
        Evaluate(node, l, r);
        

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

    private M GuardComposition(M? a, M? b)
    {
        if (a is null)
        {
            return (M)b;
        }
        else
        {
            return _composition((M)a, (M)b);
        }
    }

    public T GetByIndex(int time, int index)
    {
        Node current = GetRootAt(time);
        int l = 0;
        int r = _size;
        while (true)
        {
            Evaluate(current, l, r);
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