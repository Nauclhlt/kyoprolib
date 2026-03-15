// 平衡二分探索木set.
public sealed class Set<T> where T : IComparable<T>
{
    private sealed class Node
    {
        public T Key;
        public double Priority;
        public Node Left;
        public Node Right;
        public int Size;

        public Node(T key, double priority)
        {
            Key = key;
            Priority = priority;
            Left = null;
            Right = null;
            Size = 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LeftSize() => Left is not null ? Left.Size : 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int RightSize() => Right is not null ? Right.Size : 0;
    }

    private Node _rootNode = null;
    private Random _random;

    public int Count => _rootNode is not null ? _rootNode.Size : 0;

    public Set()
    {
        _rootNode = null;
        _random = new();
    }

    public void Add(T value)
    {
        _rootNode = AddRecursive(_rootNode, new Node(value, _random.NextDouble()));
    }

    public void Remove(T value)
    {
        _rootNode = RemoveRecursive(_rootNode, value);
    }

    private Node RemoveRecursive(Node current, T key)
    {
        if (current is null)
        {
            return null;
        }

        int comp = key.CompareTo(current.Key);
        if (comp == -1)
        {
            current.Left = RemoveRecursive(current.Left, key);
        }
        else if (comp == 1)
        {
            current.Right = RemoveRecursive(current.Right, key);
        }
        else
        {
            if (current.Left is null) return current.Right;
            if (current.Right is null) return current.Left;

            if (current.Left.Priority > current.Right.Priority)
            {
                current = RotateRight(current);
                current.Right = RemoveRecursive(current.Right, key);
            }
            else
            {
                current = RotateLeft(current);
                current.Left = RemoveRecursive(current.Left, key);
            }
        }

        Update(current);

        return current;
    }

    private Node AddRecursive(Node current, Node node)
    {
        if (current is null)
        {
            return node;
        }

        if (node.Key.CompareTo(current.Key) == -1)
        {
            current.Left = AddRecursive(current.Left, node);

            if (current.Left.Priority > current.Priority)
            {
                current = RotateRight(current);
            }
        }
        else
        {
            current.Right = AddRecursive(current.Right, node);

            if (current.Right.Priority > current.Priority)
            {
                current = RotateLeft(current);
            }
        }

        Update(current);

        return current;
    }

    private Node GetMaxNode(Node node)
    {
        Node cur = node;
        while (cur.Right is not null) cur = cur.Right;

        return cur;
    }

    private Node GetMinNode(Node node)
    {
        Node cur = node;
        while (cur.Left is not null) cur = cur.Left;

        return cur;
    }

    private Node RotateLeft(Node node)
    {
        Node right = node.Right;
        node.Right = right.Left;
        right.Left = node;

        Update(right.Left);
        Update(right.Right);
        Update(right);

        return right;
    }

    private Node RotateRight(Node node)
    {
        Node left = node.Left;
        node.Left = left.Right;
        left.Right = node;

        Update(left.Left);
        Update(left.Right);
        Update(left);


        return left;
    }

    [MethodImpl(256)]
    private void Update(Node node)
    {
        if (node is null) return;

        int left = node.LeftSize();
        int right = node.RightSize();

        node.Size = left + right + 1;
    }

    public void PrintTree()
    {
        int calls = 0;
        void PrintNode(Node node, int depth)
        {
            if (node is null) return;
            calls++;
            PrintNode(node.Right, depth + 1);
            Console.WriteLine(new string('\t', depth) + node.Key + $"({node.Size})");
            PrintNode(node.Left, depth + 1);
        }

        PrintNode(_rootNode, 0);
    }


    public T Max
    {
        get
        {
            if (_rootNode is null)
            {
                throw new InvalidOperationException("No item in the set.");
            }
            else
            {
                return GetMaxNode(_rootNode).Key;
            }
        }
    }
    public T Min
    {
        get
        {
            if (_rootNode is null)
            {
                throw new InvalidOperationException("No item in the set.");
            }
            else
            {
                return GetMinNode(_rootNode).Key;
            }
        }
    }

    public bool Contains(T value)
    {
        Node current = _rootNode;

        while (current is not null)
        {
            if (current.Key.CompareTo(value) == 0) return true;

            if (value.CompareTo(current.Key) == -1) current = current.Left;
            else current = current.Right;
        }

        return false;
    }

    public T GetByIndex(int index)
    {
        if (_rootNode is null)
        {
            throw new IndexOutOfRangeException();
        }
        if (index < 0 || index >= Count)
        {
            throw new IndexOutOfRangeException();
        }

        Node current = _rootNode;
        int left = 0;
        while (current is not null)
        {
            int center = left + current.LeftSize();
            if (center == index)
            {
                return current.Key;
            }
            else if (center < index)
            {
                left += current.LeftSize() + 1;
                current = current.Right;
            }
            else
            {
                current = current.Left;
            }
        }

        return default;
    }

    public void RemoveByIndex(int index)
    {
        if (_rootNode is null) return;
        if (index < 0 || index >= Count)
        {
            throw new IndexOutOfRangeException();
        }

        _rootNode = RemoveByIndexRecursive(_rootNode, index);
    }

    private Node RemoveByIndexRecursive(Node current, int index)
    {
        if (current is null)
        {
            return null;
        }

        int left = current.LeftSize();

        if (index < left)
        {
            current.Left = RemoveByIndexRecursive(current.Left, index);
        }
        else if (index > left)
        {
            current.Right = RemoveByIndexRecursive(current.Right, index - left - 1);
        }
        else {
            if (current.Left is null) return current.Right;
            if (current.Right is null) return current.Left;

            if (current.Left.Priority > current.Right.Priority)
            {
                current = RotateRight(current);
                current.Right = RemoveByIndexRecursive(current.Right, index - left);
            }
            else
            {
                current = RotateLeft(current);
                current.Left = RemoveByIndexRecursive(current.Left, index);
            }
        }

        Update(current);

        return current;
    }

    public int IndexOf(T value)
    {
        int index = _rootNode.LeftSize();
        Node current = _rootNode;

        while (true)
        {
            int c = value.CompareTo(current.Key);
            if (c == -1)
            {
                if (current.Left is null) return -1;
                else
                {
                    current = current.Left;
                    index -= current.RightSize() + 1;
                }
            }
            else if (c == 0) return index;
            else
            {
                if (current.Right is null) return -1;
                else
                {
                    current = current.Right;
                    index += current.LeftSize() + 1;
                }
            }
        }
    }

    public T LowerBoundValue(T value, T fallback)
    {
        if (_rootNode is null) return fallback;

        int res = _rootNode.Size;
        Node current = _rootNode;
        T lowerbound = default;
        int index = _rootNode.LeftSize();

        while (true)
        {
            int cmp = value.CompareTo(current.Key);
            if (cmp <= 0)
            {
                res = int.Min(res, index);
                lowerbound = current.Key;
                if (current.Left is null) break;
                index -= current.Left.RightSize() + 1;
                current = current.Left;
            }
            else
            {
                if (current.Right is null) break;
                index += current.Right.LeftSize() + 1;
                current = current.Right;
            }
        }

        return res < Count ? lowerbound : fallback;
    }

    public int LowerBound(T value)
    {
        if (_rootNode is null) return 0;

        int res = _rootNode.Size;
        Node current = _rootNode;
        int index = _rootNode.LeftSize();

        while (true)
        {
            int cmp = value.CompareTo(current.Key);
            if (cmp <= 0)
            {
                res = int.Min(res, index);
                if (current.Left is null) break;
                index -= current.Left.RightSize() + 1;
                current = current.Left;
            }
            else
            {
                if (current.Right is null) break;
                index += current.Right.LeftSize() + 1;
                current = current.Right;
            }
        }

        return res;
    }

    public List<T> OrderAscending()
    {
        if (_rootNode is null) return new List<T>();

        List<T> res = new(_rootNode.Size);

        void extract(Node node)
        {
            if (node is null) return;
            extract(node.Left);
            res.Add(node.Key);
            extract(node.Right);
        }

        extract(_rootNode);

        return res;
    }

    public List<T> OrderDescending()
    {
        if (_rootNode is null) return new List<T>();

        List<T> res = new(_rootNode.Size);

        void extract(Node node)
        {
            if (node is null) return;
            extract(node.Right);
            res.Add(node.Key);
            extract(node.Left);
        }

        extract(_rootNode);

        return res;
    }
}