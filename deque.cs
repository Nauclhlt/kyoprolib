public sealed class Deque<T> : IEnumerable<T>
{
    private const int DefaultCapacity = 32;

    private T[] _buffer;
    private int _capacity;

    private int _head;
    private int _length;

    public int Count => _length;
    public int Capacity => _capacity;
    
    public T this[int index]
    {
        get {
            if (index < 0 || index >= _length) throw new IndexOutOfRangeException("The specified index is out of the bounds of the deque.");
            return _buffer[_head + index];
        }
        set {
            if (index < 0 || index >= _length) throw new IndexOutOfRangeException("The specified index is out of the bounds of the deque.");
            _buffer[_head + index] = value;
        }
    }

    public Deque() : this(DefaultCapacity)
    {
    }

    public Deque(int capacity)
    {
        if (capacity < 0)
        {
            throw new ArgumentException("The capacity needs to be greater than 0.");
        }

        _capacity = capacity;
        _buffer = new T[_capacity];
        _head = _capacity / 2;
        _length = 0;
    }

    private void ResizeBuffer()
    {
        _capacity <<= 1;

        T[] newBuffer = new T[_capacity];
        int newHead = (_capacity - _length) / 2;
        Array.Copy(_buffer, _head, newBuffer, newHead, _length);

        _head = newHead;
        _buffer = newBuffer;
    }

    public void PushFront(T item)
    {
        if (_length == 0)
        {
            _length++;
            _buffer[_head] = item;
        }
        else
        {
            if (_head == 0)
            {
                ResizeBuffer();
            }

            _head--;
            _buffer[_head] = item;
            _length++;
        }
    }

    public void PushBack(T item)
    {
        if (_head + _length >= _capacity)
        {
            ResizeBuffer();
        }

        _buffer[_head + _length] = item;
        _length++;
    }

    public T PopFront()
    {
        if (_length == 0)
        {
            throw new InvalidOperationException("Deque is empty.");
        }

        T front = _buffer[_head];
        _head++;
        _length--;

        return front;
    }

    public T PopBack()
    {
        if (_length == 0)
        {
            throw new InvalidOperationException("Deque is empty.");
        }

        T back = _buffer[_head + _length - 1];
        _length--;

        return back;
    }

    public T PeekFront()
    {
        if (_length == 0)
        {
            throw new InvalidOperationException("Deque is empty.");
        }

        return _buffer[_head];
    }

    public T PeekBack()
    {
        if (_length == 0)
        {
            throw new InvalidOperationException("Deque is empty.");
        }

        return _buffer[_head + _length - 1];
    }

    public bool TryPeekFront(out T value)
    {
        if (_length == 0)
        {
            value = default;
            return false;
        }

        value = _buffer[_head];
        return true;
    }

    public bool TryPeekBack(out T value)
    {
        if (_length == 0)
        {
            value = default;
            return false;
        }

        value = _buffer[_head + _length - 1];
        return true;
    }

    public Span<T> AsSpan()
    {
        return _buffer.AsSpan(_head, _length);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return (new ArraySegment<T>(_buffer, _head, _length)).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}