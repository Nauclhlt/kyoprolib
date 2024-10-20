// 平衡二分探索木を利用してソートされた集合を管理する.
public sealed class Set<T> where T : IComparable<T>
{
    private AVLTree<T> _tree;

    public T Max => _tree.Max();
    public T Min => _tree.Min();

    public int Count => _tree.Count;

    public Set()
    {
        _tree = new();
    }

    // 要素を追加する.
    // O(logN)
    public void Add(T item)
    {
        _tree.Add(item);
    }

    // 要素を削除する.
    // O(logN)
    public void Remove(T item)
    {
        _tree.Remove(item);
    }

    // 要素が含まれているかを返す.
    // O(logN)
    public bool Contains(T item)
    {
        return _tree.Contains(item);
    }

    // itemのインデックスを返す.
    // O(logN)
    public int IndexOf(T item)
    {
        return _tree.IndexOf(item);
    }

    // 値がvalue以上となる最初のインデックスを返す.
    public int LowerBound(T value)
    {
        return _tree.LowerBound(value);
    }

    public T LowerBoundValue(T value, T fallback)
    {
        return _tree.LowerBoundValue(value, fallback);
    }

    // インデックスで値を取得する.
    public T GetByIndex(int index)
    {
        return _tree.GetByIndex(index);
    }

    public void RemoveByIndex(int index)
    {
        _tree.RemoveByIndex(index);
    }

    // 昇順ソートされたリストを返す.
    public List<T> OrderAscending()
    {
        return _tree.OrderAscending();
    }

    // 降順ソートされたリストを返す.
    public List<T> OrderDescending()
    {
        return _tree.OrderDescending();
    }

    public void DebugPrintTree()
    {
        _tree.PrintTree();
    }
}

// AVL木, 平衡二分探索木.
// 参考: https://zenn.dev/student_blog/articles/670eee14e04d46
public sealed class AVLTree<T> where T : IComparable<T>
{
    private sealed class Node
    {
        private T _value;
        private Node _left;
        private Node _right;
        private int _bias;
        private int _height;
        private int _size;

        public bool Has2Children => _left is not null && _right is not null;
        public bool HasOnlyLeft => _left is not null && _right is null;
        public bool HasOnlyRight => _left is null && _right is not null;
        public bool HasNoChild => _left is null && _right is null;

        public Node(T value)
        {
            _value = value;
            _left = null;
            _right = null;
            _bias = 0;
            _height = 1;
            _size = 1;
        }

        public T Value
        {
            get => _value;
            set => _value = value;
        }

        public Node Left
        {
            get => _left;
            set => _left = value;
        }

        public Node Right
        {
            get => _right;
            set => _right = value;
        }

        public int Bias
        {
            get => _bias;
            set => _bias = value;
        }

        public int Height
        {
            get => _height;
            set => _height = value;
        }

        public int Size
        {
            get => _size;
            set => _size = value;
        }

        public int LeftHeight() => _left is not null ? _left.Height : 0;
        public int RightHeight() => _right is not null ? _right.Height : 0;
        public int LeftSize() => _left is not null ? _left.Size : 0;
        public int RightSize() => _right is not null ? _right.Size : 0;
    }

    private Node _rootNode = null;

    public int Count => SizeOf(_rootNode);

    public AVLTree()
    {
        _rootNode = null;
    }

    public void Add(T value)
    {
        _rootNode = AddRecursive(_rootNode, value);
    }

    public void Remove(T value)
    {
        _rootNode = RemoveRecursive(_rootNode, value);
    }

    private Node RemoveRecursive(Node current, T value)
    {
        if (current is null)
        {
            return null;
        }

        if (current.Value.CompareTo(value) == 0)
        {
            // 消す
            return InternalRemoveNode(current);
        }

        if (value.CompareTo(current.Value) == -1)
        {
            current.Left = RemoveRecursive(current.Left, value);

            Update(current.Left);
            Update(current);
            current = Balance(current);

            Update(current);

            return current;
        }
        else
        {
            current.Right = RemoveRecursive(current.Right, value);

            Update(current.Right);
            Update(current);

            current = Balance(current);

            Update(current);

            return current;
        }
    }

    private Node InternalRemoveNode(Node target)
    {
        if (target.Has2Children)
        {
            Node max = GetMaxNode(target.Left);
            T val = max.Value;
            
            if (target.Left == max)
            {
                target.Left = target.Left.Left;
            }
            else
            {
                target.Left = DeleteRightNode(target.Left, max);
            }
            
            target.Value = val;

            Update(target.Left);
            Update(target.Right);

            Update(target);
            target = Balance(target);

            Update(target);

            return target;
        }
        else if (target.HasOnlyLeft)
        {
            target = target.Left;

            Update(target.Left);
            Update(target.Right);
            Update(target);

            target = Balance(target);

            Update(target);

            return target;
        }
        else if (target.HasOnlyRight)
        {
            target = target.Right;

            Update(target.Left);
            Update(target.Right);
            Update(target);

            target = Balance(target);

            Update(target);

            return target;
        }
        else
        {
            return null;
        }
    }

    private Node AddRecursive(Node current, T value)
    {
        if (current is null)
        {
            current = new Node(value);
            
            Update(current);

            return current;
        }

        if (value.CompareTo(current.Value) == -1)
        {
            current.Left = AddRecursive(current.Left, value);

            Update(current.Left);
            Update(current);

            current = Balance(current);

            Update(current);
        }
        else
        {
            current.Right = AddRecursive(current.Right, value);

            Update(current.Right);
            Update(current);

            current = Balance(current);

            Update(current);
        }

        return current;
    }

    private Node Balance(Node node)
    {
        if (node.Bias == 0)
        {
            return node;
        }

        if (node.Bias == 1 || node.Bias == -1)
        {
            return node;
        }

        if (node.Bias >= 2)
        {
            if (node.Left.Bias > 0)
            {
                node = RotateRight(node);
                node.Bias = 0;
                return node;
            }
            else
            {
                node.Left = RotateLeft(node.Left);
                node = RotateRight(node);
                node.Bias = 0;
                return node;
            }
        }
        else
        {
            if (node.Right.Bias < 0)
            {
                node = RotateLeft(node);
                node.Bias = 0;
                return node;
            }
            else
            {
                node.Right = RotateRight(node.Right);
                node = RotateLeft(node);
                node.Bias = 0;
                return node;
            }
        }
    }

    private Node DeleteRightNode(Node root, Node target)
    {
        if (root is null) return null;

        if (root.Right == target)
        {
            root.Right = root.Right.Left;
            Update(root.Right);
            Update(root);

            root = Balance(root);

            Update(root);

            return root;
        }
        else
        {
            int sz = root.Size;
            root.Right = DeleteRightNode(root.Right, target);
            

            Update(root.Right);
            Update(root.Left);
            Update(root);
            root = Balance(root);

            if (sz - root.Size == 2)
            {
                Console.WriteLine("ERR");
            }

            Update(root);

            return root;
        }
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

    // 新しい部分木の根を返す
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

    // 新しい部分木の根を返す
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

    private void Update(Node node)
    {
        if (node is null) return;

        node.Height = HeightOf(node);
        node.Size = SizeOf(node);
        node.Bias = node.LeftHeight() - node.RightHeight();
    }

    private int HeightOf(Node node)
    {
        if (node is null) return 0;

        int left = node.LeftHeight();
        int right = node.RightHeight();

        return int.Max(left, right) + 1;
    }

    private int SizeOf(Node node)
    {
        if (node is null) return 0;

        int left = node.LeftSize();
        int right = node.RightSize();

        return left + right + 1;
    }

    public void PrintTree()
    {
        static void PrintNode(Node node, int depth)
        {
            if (node is null) return;
            PrintNode(node.Right, depth + 1);
            Console.WriteLine(new string('\t', depth) + node.Value + $"({node.Bias}; {node.Height}; {node.Size})");
            PrintNode(node.Left, depth + 1);
        }

        PrintNode(_rootNode, 0);
    }




    public bool Contains(T value)
    {
        Node current = _rootNode;

        while (current is not null)
        {
            if (current.Value.CompareTo(value) == 0) return true;

            if (value.CompareTo(current.Value) == -1) current = current.Left;
            else current = current.Right;
        }

        return false;
    }

    public T Max()
    {
        return GetMaxNode(_rootNode).Value;
    }

    public T Min()
    {
        return GetMinNode(_rootNode).Value;
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

        return GetByIndexRecursive(_rootNode, index);
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

    private Node RemoveByIndexRecursive(Node current, int offset)
    {
        if (current is null)
        {
            Console.WriteLine($"! current was null. offset={offset}  count={Count}");
            return default;
        }
        
        int left = current.LeftSize();

        if (left == offset)
        {
            return InternalRemoveNode(current);
        }
        if (offset < left)
        {
            current.Left = RemoveByIndexRecursive(current.Left, offset);

            Update(current);

            current = Balance(current);

            Update(current);

            return current;
        }
        else
        {
            current.Right = RemoveByIndexRecursive(current.Right, offset - left - 1);

            Update(current);

            current = Balance(current);

            Update(current);

            return current;
        }
    }

    private T GetByIndexRecursive(Node current, int offset)
    {
        if (current is null)
        {
            Console.WriteLine($"! current was null. offset={offset}  count={Count}");
            return default;
        }
        int left = current.LeftSize();
        if (left == offset)
        {
            return current.Value;
        }
        if (offset < left)
        {
            return GetByIndexRecursive(current.Left, offset);
        }
        else
        {
            return GetByIndexRecursive(current.Right, offset - left - 1);
        }
    }

    public int IndexOf(T value)
    {
        int index = _rootNode.LeftSize();
        Node current = _rootNode;

        while (true)
        {
            // left
            int c = value.CompareTo(current.Value);
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
            int cmp = value.CompareTo(current.Value);
            if (cmp <= 0)
            {
                res = int.Min(res, index);
                lowerbound = current.Value;
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
            int cmp = value.CompareTo(current.Value);
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

        // Node root = _rootNode;

        // var torg = root;
        // if (root is null) return -1;

        // var idx = root.Size - root.RightSize() - 1;
        // var ret = Int32.MaxValue;
        // while (root is not null)
        // {
        //     if (root.Value.CompareTo(value) >= 0)
        //     {
        //         if (root.Value.CompareTo(value) == 0) ret = int.Min(ret, idx);
        //         root = root.Left;
        //         if (root == null) ret = Math.Min(ret, idx);
        //         idx -= root is null ? 0 : (root.RightSize() + 1);
        //     }
        //     else
        //     {
        //         root = root.Right;
        //         idx += (root is null ? 1 : root.LeftSize() + 1);
        //         if (root == null) return idx;
        //     }
        // }

        // return ret == Int32.MaxValue ? torg.Size : ret;
    }

    public List<T> OrderAscending()
    {
        if (_rootNode is null) return new List<T>();

        List<T> res = new(_rootNode.Size);

        void extract(Node node)
        {
            if (node is null) return;
            extract(node.Left);
            res.Add(node.Value);
            extract(node.Right);
        }

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
            res.Add(node.Value);
            extract(node.Left);
        }

        return res;
    }
}