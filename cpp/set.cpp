template <typename T>
class AVLTree
{
private:
    class Node
    {
    private:
        T _value;
        shared_ptr<Node> _left;
        shared_ptr<Node> _right;
        int _bias;
        int _height;
        int _size;

    public:
        Node(T value)
        {
            _value = value;
            _left = shared_ptr<Node>();
            _right = shared_ptr<Node>();
            _bias = 0;
            _height = 1;
            _size = 1;
        }

        inline bool Has2Children()
        {
            return _left && _right;
        }

        inline bool HasOnlyLeft()
        {
            return _left && !_right;
        }

        inline bool HasOnlyRight()
        {
            return !_left && _right;
        }

        inline bool HasNoChild()
        {
            return !_left && !_right;
        }

        inline bool HasRight()
        {
            return (bool)_right;
        }

        inline bool HasLeft()
        {
            return (bool)_left;
        }

        inline void SetValue(T value)
        {
            _value = value;
        }

        inline T GetValue()
        {
            return _value;
        }

        inline void SetLeft(shared_ptr<Node> left)
        {
            _left = left;
        }

        inline shared_ptr<Node> GetLeft()
        {
            return _left;
        }

        inline void SetRight(shared_ptr<Node> right)
        {
            _right = right;
        }

        inline shared_ptr<Node> GetRight()
        {
            return _right;
        }

        inline void SetBias(int bias)
        {
            _bias = bias;
        }

        inline int GetBias()
        {
            return _bias;
        }

        inline void SetHeight(int height)
        {
            _height = height;
        }

        inline int GetHeight()
        {
            return _height;
        }

        inline void SetSize(int size)
        {
            _size = size;
        }

        inline int GetSize()
        {
            return _size;
        }

        inline int LeftHeight()
        {
            return _left ? _left->GetHeight() : 0;
        }

        inline int RightHeight()
        {
            return _right ? _right->GetHeight() : 0;
        }

        inline int LeftSize()
        {
            return _left ? _left->GetSize() : 0;
        }

        inline int RightSize()
        {
            return _right ? _right->GetSize() : 0;
        }
    };

    shared_ptr<Node> _rootNode;


public:
    AVLTree()
    {
        _rootNode = shared_ptr<Node>();
    }

    int Count()
    {
        return SizeOf(_rootNode);
    }

    void Add(T value)
    {
        _rootNode = AddRecursive(_rootNode, value);
    }

    void Remove(T value)
    {
        _rootNode = RemoveRecursive(_rootNode, value);
    }

    void _PrintTree()
    {
        auto printNode = [&](shared_ptr<Node> node, int depth, auto self)
        {
            if (!node)
                return;
            self(node->GetRight(), depth + 1, self);
            for (int i = 0; i < depth; i++)
            {
                cout << "\t";
            }
            cout << node->GetValue() << endl;
            self(node->GetLeft(), depth + 1, self);
        };

        printNode(_rootNode, 0, printNode);
    }

private:
    shared_ptr<Node> RemoveRecursive(shared_ptr<Node> current, T value)
    {
        if (!current)
        {
            return shared_ptr<Node>();
        }

        if (current->GetValue() == value)
        {
            return InternalRemoveNode(current);
        }

        if (value < current->GetValue())
        {
            current->SetLeft(RemoveRecursive(current->GetLeft(), value));

            Update(current->GetLeft());
            Update(current);

            current = Balance(current);
        }
        else
        {
            current->SetRight(RemoveRecursive(current->GetRight(), value));

            Update(current->GetRight());
            Update(current);

            current = Balance(current);
        }

        return current;
    }

    shared_ptr<Node> InternalRemoveNode(shared_ptr<Node> target)
    {
        if (target->Has2Children())
        {
            shared_ptr<Node> max = GetMaxNode(target);
            T val = max->GetValue();

            if (target->GetLeft() == max)
            {
                target->SetLeft(target->GetLeft()->GetLeft());
            }
            else
            {
                target->SetLeft(DeleteRightNode(target->GetLeft(), max));
            }

            target->SetValue(val);

            Update(target->GetLeft());
            Update(target->GetLeft());

            Update(target);
            target = Balance(target);

            return target;
        }
        else if (target->HasOnlyLeft())
        {
            target = target->GetLeft();

            Update(target->GetLeft());
            Update(target->GetRight());
            Update(target);

            target = Balance(target);

            return target;
        }
        else if (target->HasOnlyRight())
        {
            target = target->GetRight();

            Update(target->GetLeft());
            Update(target->GetRight());
            Update(target);

            target = Balance(target);

            return target;
        }
        else
        {
            return shared_ptr<Node>();
        }
    }

    shared_ptr<Node> AddRecursive(shared_ptr<Node> current, T value)
    {
        if (!current)
        {
            current = shared_ptr<Node>(new Node(value));
            
            Update(current);

            return current;
        }

        if (value < current->GetValue())
        {
            current->SetLeft(AddRecursive(current->GetLeft(), value));

            Update(current->GetLeft());
            Update(current);

            current = Balance(current);
        }
        else
        {
            current->SetRight(AddRecursive(current->GetRight(), value));

            Update(current->GetRight());
            Update(current);

            current = Balance(current);
        }

        return current;
    }

    T GetByIndexRecursive(shared_ptr<Node> current, int offset)
    {
        int left = current->LeftSize();
        if (left == offset)
        {
            return current->GetValue();
        }
        if (offset < left)
        {
            return GetByIndexRecursive(current->GetLeft(), offset);
        }
        else
        {
            return GetByIndexRecursive(current->GetRight(), offset - left - 1);
        }
    }

    shared_ptr<Node> Balance(shared_ptr<Node> node)
    {
        int bias = node->GetBias();

        if (bias == 0)
        {
            return node;
        }

        if (bias == 1 || bias == -1)
        {
            return node;
        }

        if (bias >= 2)
        {
            if (node->GetLeft()->GetBias() > 0)
            {
                node = RotateRight(node);
                node->SetBias(0);
                return node;
            }
            else
            {
                node->SetLeft(RotateLeft(node->GetLeft()));
                node = RotateRight(node);
                node->SetBias(0);
                return node;
            }
        }
        else
        {
            if (node->GetRight()->GetBias() < 0)
            {
                node = RotateLeft(node);
                node->SetBias(0);

                return node;
            }
            else
            {
                node->SetRight(RotateRight(node->GetRight()));
                node = RotateLeft(node);
                node->SetBias(0);

                return node;
            }
        }
    }

    shared_ptr<Node> DeleteRightNode(shared_ptr<Node> root, shared_ptr<Node> target)
    {
        if (!root)
            return shared_ptr<Node>();

        if (root->GetRight() == target)
        {
            root->SetRight(root->GetRight()->GetLeft());
            Update(root->GetRight());
            Update(root);

            root = Balance(root);

            return root;
        }
        else
        {
            root->SetRight(DeleteRightNode(root->GetRight(), target));

            Update(root->GetRight());
            Update(root->GetLeft());
            Update(root);

            root = Balance(root);

            return root;
        }
    }

    shared_ptr<Node> GetMaxNode(shared_ptr<Node> node)
    {
        shared_ptr<Node> cur = node;
        while (cur->HasRight())
        {
            cur = cur->GetRight();
        }

        return cur;
    }

    shared_ptr<Node> GetMinNode(shared_ptr<Node> node)
    {
        shared_ptr<Node> cur = node;
        while (cur->HasLeft())
        {
            cur = cur->GetLeft();
        }

        return cur;
    }

    shared_ptr<Node> RotateLeft(shared_ptr<Node> node)
    {
        shared_ptr<Node> right = node->GetRight();
        node->SetRight(right->GetLeft());
        right->SetLeft(node);

        Update(right->GetLeft());
        Update(right->GetRight());
        Update(right);

        return right;
    }

    shared_ptr<Node> RotateRight(shared_ptr<Node> node)
    {
        shared_ptr<Node> left = node->GetLeft();
        node->SetLeft(left->GetRight());
        left->SetRight(node);

        Update(left->GetLeft());
        Update(left->GetRight());
        Update(left);

        return left;
    }

    void Update(shared_ptr<Node> node)
    {
        if (!node)
            return;

        node->SetHeight(HeightOf(node));
        node->SetSize(SizeOf(node));
        node->SetBias(node->LeftHeight() - node->RightHeight());
    }

    int HeightOf(shared_ptr<Node> node)
    {
        if (!node)
            return 0;

        int left = node->LeftHeight();
        int right = node->RightHeight();

        return max(left, right) + 1;
    }

    int SizeOf(shared_ptr<Node> node)
    {
        if (!node)
            return 0;

        int left = node->LeftSize();
        int right = node->RightSize();

        return left + right + 1;
    }

public:
    bool Contains(T value)
    {
        shared_ptr<Node> current = _rootNode;

        while (current)
        {
            if (current->GetValue() == value)
            {
                return true;
            }

            if (value < current->GetValue())
            {
                current = current->GetLeft();
            }
            else
            {
                current = current->GetRight();
            }
        }

        return false;
    }

    T Max()
    {
        return GetMaxNode(_rootNode)->GetValue();
    }

    T Min()
    {
        return GetMinNode(_rootNode)->GetValue();
    }

    T GetByIndex(int index)
    {
        if (!_rootNode)
        {
            throw out_of_range("The specified index is out of range.");
        }

        if (index < 0 || index >= Count())
        {
            throw out_of_range("The specified index is out of range.");
        }

        return GetByIndexRecursive(_rootNode, index);
    }

    int IndexOf(T value)
    {
        if (!_rootNode)
        {
            return -1;
        }

        int index = _rootNode->LeftSize();
        shared_ptr<Node> current = _rootNode;

        while (true)
        {
            if (value < current->GetValue())
            {
                if (!current->HasLeft())
                {
                    return -1;
                }
                else
                {
                    current = current->GetLeft();
                    index -= current->RightSize() + 1;
                }
            }
            else if (value == current->GetValue())
            {
                return index;
            }
            else
            {
                if (!current->HasRight())
                {
                    return -1;
                }
                else
                {
                    current = current->GetRight();
                    index += current->LeftSize() + 1;
                }
            }
        }
    }

    int LowerBound(T value)
    {
        if (!_rootNode)
        {
            return 0;
        }

        int res = _rootNode->GetSize();
        shared_ptr<Node> current = _rootNode;
        int index = _rootNode->LeftSize();

        while (true)
        {
            if (value <= current->GetValue())
            {
                res = min(res, index);
                if (!current->HasLeft())
                {
                    break;
                }
                index -= current->GetLeft()->RightSize() + 1;
                current = current->GetLeft();
            }
            else
            {
                if (!current->HasRight())
                {
                    break;
                }
                index += current->GetRight()->LeftSize() + 1;
                current = current->GetRight();
            }
        }

        return res;
    }

    T LowerBoundValue(T value, T fallback)
    {
        if (!_rootNode)
        {
            return fallback;
        }

        int res = _rootNode->GetSize();
        shared_ptr<Node> current = _rootNode;
        int index = _rootNode->LeftSize();
        T lowerbound = fallback;

        while (true)
        {
            if (value <= current->GetValue())
            {
                res = min(res, index);
                lowerbound = current->GetValue();
                if (!current->HasLeft())
                {
                    break;
                }
                index -= current->GetLeft()->RightSize() + 1;
                current = current->GetLeft();
            }
            else
            {
                if (!current->HasRight())
                {
                    break;
                }
                index += current->GetRight()->LeftSize() + 1;
                current = current->GetRight();
            }
        }

        return res < Count() ? lowerbound : fallback;
    }

    vector<T> OrderAscending()
    {
        if (!_rootNode)
        {
            return vector<T>();
        }

        vector<T> res;
        res.reserve(Count());

        auto extract = [&](shared_ptr<Node> node, auto self)
        {
            if (!node)
                return;
            self(node->GetLeft(), self);
            res.push_back(node->GetValue());
            self(node->GetRight(), self);
        };

        return res;
    }

    vector<T> OrderDescending()
    {
        if (!_rootNode)
        {
            return vector<T>();
        }

        vector<T> res;
        res.reserve(Count());

        auto extract = [&](shared_ptr<Node> node, auto self)
        {
            if (!node)
                return;
            self(node->GetRight(), self);
            res.push_back(node->GetValue());
            self(node->GetLeft(), self);
        };

        return res;
    }
};

template <typename T>
class NauclhltSet
{
private:
    AVLTree<T> _tree;

public:
    inline int Count()
    {
        return _tree.Count();
    }

    inline T Max()
    {
        return _tree.Max();
    }

    inline T Min()
    {
        return _tree.Min();
    }

    inline void Add(T item)
    {
        _tree.Add(item);
    }

    inline void Remove(T item)
    {
        _tree.Remove(item);
    }

    inline bool Contains(T item)
    {
        return _tree.Contains(item);
    }

    inline int IndexOf(T item)
    {
        return _tree.IndexOf(item);
    }

    inline int LowerBound(T value)
    {
        return _tree.LowerBound(value);
    }

    inline T LowerBoundValue(T value, T fallback)
    {
        return _tree.LowerBoundValue(value, fallback);
    }

    inline T GetByIndex(int index)
    {
        return _tree.GetByIndex(index);
    }

    inline vector<T> OrderAscending()
    {
        return _tree.OrderAscending();
    }

    inline vector<T> OrderDescending()
    {
        return _tree.OrderDescending();
    }

    const T operator[](size_t index) const
    {
        return GetByIndex((int)index);
    }

    T operator[](size_t index)
    {
        return GetByIndex((int)index);
    }

    inline void _DebugPrintTree()
    {
        _tree._PrintTree();
    }
};