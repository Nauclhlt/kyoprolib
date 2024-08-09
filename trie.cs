// Trie木.
// 各操作O(|S|).
// @author Nauclhlt.
public sealed class Trie
{
    private sealed class TrieNode
    {
        private char _character;
        private bool _terminate;
        private SortedList<char, TrieNode> _children;
        private TrieNode _parent;
        private int _count;

        public char Character => _character;
        public bool Terminate
        {
            get => _terminate;
            set => _terminate = value;
        }
        public SortedList<char, TrieNode> Children => _children;
        public TrieNode Parent
        {
            get => _parent;
            set => _parent = value;
        }
        public int Count
        {
            get => _count;
            set => _count = value;
        }

        public TrieNode(char c)
        {
            _character = c;
            _children = new SortedList<char, TrieNode>();
            _terminate = false;
            _parent = null;
            _count = 0;
        }

        public bool HasChild(char c)
        {
            return _children.ContainsKey(c);
        }

        public TrieNode GetChild(char c)
        {
            return _children[c];
        }

        public void AddChild(char c, TrieNode node)
        {
            node.Parent = this;
            _children[c] = node;
        }

        public void RemoveChild(char c)
        {
            _children.Remove(c);
        }    
    }

    private TrieNode _root;

    public Trie()
    {
        _root = new TrieNode('_');
    }

    public void Add(string item)
    {
        if (item is null || item.Length == 0) return;

        ReadOnlySpan<char> span = item.AsSpan();

        TrieNode current = _root;

        for (int i = 0; i < span.Length; i++)
        {
            if (current.HasChild(span[i]))
            {
                current = current.GetChild(span[i]);
            }
            else
            {
                TrieNode node = new TrieNode(span[i]);
                current.AddChild(span[i], node);
                current = node;
            }

            current.Count++;
        }

        current.Terminate = true;
    }

    public void Remove(string item)
    {
        if (item is null || item.Length == 0) return;

        if (!Contains(item)) return;

        ReadOnlySpan<char> span = item.AsSpan();

        TrieNode current = _root;

        for (int i = 0; i < span.Length; i++)
        {
            if (current.HasChild(span[i]))
            {
                current = current.GetChild(span[i]);
            }
            else
            {
                // item not contained
                return;
            }

            current.Count--;
        }

        current.Terminate = false;

        TrieNode p = current;
        while (!p.Terminate && p.Children.Count == 0 && p.Character != '_')
        {
            TrieNode parent = p.Parent;
            parent.RemoveChild(p.Character);

            p = parent;
        }
    }

    public bool Contains(string item)
    {
        if (item is null || item.Length == 0) return false;

        ReadOnlySpan<char> span = item.AsSpan();

        TrieNode current = _root;

        for (int i = 0; i < span.Length; i++)
        {
            if (current.HasChild(span[i]))
            {
                current = current.GetChild(span[i]);
            }
            else
            {
                return false;
            }
        }

        return current.Terminate;
    }

    public int LcpLength(string s)
    {
        if (s is null || s.Length == 0) return 0;

        ReadOnlySpan<char> span = s.AsSpan();
        int res = 0;

        TrieNode current = _root;

        for (int i = 0; i < span.Length; i++)
        {
            if (current.HasChild(span[i]))
            {
                res++;
                current = current.GetChild(span[i]);
            }
            else
            {
                return res;
            }
        }

        return res;
    }

    private List<string> AllEntriesFrom(string head, TrieNode from)
    {
        Stack<(string, TrieNode)> stack = new();

        stack.Push((head, from));

        List<string> res = new();

        while (stack.Count > 0)
        {
            (string prefix, TrieNode node) = stack.Pop();

            if (node.Terminate)
            {
                res.Add(prefix);
            }

            foreach (char c in node.Children.Keys)
            {
                stack.Push((prefix + c, node.Children[c]));
            }
        }

        return res;
    }

    public List<string> PrefixEntries(string prefix)
    {
        if (prefix is null || prefix.Length == 0)
        {
            return AllEntriesFrom(string.Empty, _root);
        }

        ReadOnlySpan<char> span = prefix.AsSpan();

        TrieNode current = _root;

        for (int i = 0; i < span.Length; i++)
        {
            if (current.HasChild(span[i]))
            {
                current = current.GetChild(span[i]);
            }
            else
            {
                return new List<string>();
            }
        }

        return AllEntriesFrom(prefix, current);
    }

    public List<string> AllEntries()
    {
        return AllEntriesFrom(string.Empty, _root);
    }

    public int CountPrefix(string prefix)
    {
        if (prefix is null || prefix.Length == 0) return 0;

        ReadOnlySpan<char> span = prefix.AsSpan();
        TrieNode current = _root;

        for (int i = 0; i < span.Length; i++)
        {
            if (current.HasChild(span[i]))
            {
                current = current.GetChild(span[i]);
            }
            else
            {
                return 0;
            }
        }

        return current.Count;
    }
}