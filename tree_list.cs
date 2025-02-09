// AVL木, 平衡二分探索木を使ったいい感じのlist.
// 参考: https://zenn.dev/student_blog/articles/670eee14e04d46
public sealed class TreeList<T> where T : IComparable<T>
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

    public TreeList()
    {
        _rootNode = null;
    }

    public int Count => SizeOf(_rootNode);

    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= Count)
            {
                throw new IndexOutOfRangeException();
            }
            return GetElementAt(index);
        }
        set
        {
            if (index < 0 || index >= Count)
            {
                throw new IndexOutOfRangeException();
            }
            SetValue(index, value);
        }
    }

    public T First
    {
        get
        {
            if (_rootNode is null)
            {
                throw new InvalidOperationException("No item in the set.");
            }
            else
            {
                return GetMaxNode(_rootNode).Value;
            }
        }
    }
    public T Last
    {
        get
        {
            if (_rootNode is null)
            {
                throw new InvalidOperationException("No item in the set.");
            }
            else
            {
                return GetMinNode(_rootNode).Value;
            }
        }
    }

    public void Insert(int index, T value)
    {
        if (index < 0 || index > Count)
        {
            throw new IndexOutOfRangeException();
        }

        _rootNode = InsertRecursive(_rootNode, index, value);
    }

    public void Append(T value)
    {
        Insert(Count, value);
    }

    public void Prepend(T value)
    {
        Insert(0, value);
    }

    public void RemoveAt(int index)
    {
        if (_rootNode is null) return;
        if (index < 0 || index >= Count)
        {
            throw new IndexOutOfRangeException();
        }

        _rootNode = RemoveByIndexRecursive(_rootNode, index);
    }

    public void SetValue(int index, T value)
    {
        if (index < 0 || index >= Count)
        {
            throw new IndexOutOfRangeException();
        }

        SetByIndexRecursive(_rootNode, index, value);
    }

    public T GetElementAt(int index)
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

    public List<T> AsList()
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

        extract(_rootNode);

        return res;
    }

    public List<T> AsReversedList()
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

        extract(_rootNode);

        return res;
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

    private Node InsertRecursive(Node current, int offset, T value)
    {
        if (current is null)
        {
            current = new Node(value);

            Update(current);

            return current;
        }
        int left = current.LeftSize();
        if (offset <= left)
        {
            current.Left = InsertRecursive(current.Left, offset, value);

            Update(current.Left);
            Update(current);

            current = Balance(current);

            Update(current);
        }
        else
        {
            current.Right = InsertRecursive(current.Right, offset - left - 1, value);

            Update(current.Right);
            Update(current);

            current = Balance(current);

            Update(current);
        }

        return current;
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

    private void SetByIndexRecursive(Node current, int offset, T value)
    {
        if (current is null)
        {
            Console.WriteLine($"! current was null. offset={offset}  count={Count}");
            return;
        }
        int left = current.LeftSize();
        if (left == offset)
        {
            current.Value = value;
        }
        if (offset < left)
        {
            SetByIndexRecursive(current.Left, offset, value);
        }
        else
        {
            SetByIndexRecursive(current.Right, offset - left - 1, value);
        }
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
}