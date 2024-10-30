// 円環を管理する.
// @author Nauclhlt.
public readonly struct Ring
{
    private readonly int _size;

    public int Size => _size;

    public Ring(int size)
    {
        _size = size;
    }

    public RingPoint GetPoint(int point)
    {
        return new RingPoint(this, point);
    }

    public static bool operator == (Ring left, Ring right)
    {
        return left._size == right._size;
    }

    public static bool operator != (Ring left, Ring right)
    {
        return !(left == right);
    }

    public override bool Equals(object obj)
    {
        if (obj is Ring other) return this == other;
        else return false;
    }

    public override int GetHashCode()
    {
        return _size.GetHashCode();
    }

    public int Mod(int p)
    {
        p %= _size;
        if (p < 0) p += _size;

        return p;
    }
}

// 円環上での座標を管理する.
public struct RingPoint
{
    private int _point;
    private Ring _ring;

    public RingPoint(Ring ring, int pt)
    {
        _ring = ring;
        _point = ring.Mod(pt);
    }

    public static RingPoint operator +(RingPoint left, int right)
    {
        return new RingPoint(left._ring, left._point + right);
    }

    public static RingPoint operator -(RingPoint left, int right)
    {
        return new RingPoint(left._ring, left._point - right);
    }

    public static RingPoint operator *(RingPoint left, int right)
    {
        return new RingPoint(left._ring, left._point * right);
    }

    public static RingPoint operator /(RingPoint left, int right)
    {
        return new RingPoint(left._ring, left._point / right);
    }

    public static RingPoint operator ++(RingPoint point)
    {
        return new RingPoint(point._ring, point._point + 1);
    }

    public static RingPoint operator --(RingPoint point)
    {
        return new RingPoint(point._ring, point._point - 1);
    }

    public static bool operator ==(RingPoint left, RingPoint right)
    {
        return left._ring == right._ring && left._point == right._point;
    }

    public static bool operator !=(RingPoint left, RingPoint right)
    {
        return !(left == right);
    }

    public static implicit operator int(RingPoint from)
    {
        return from._point;
    }

    public override readonly bool Equals(object obj)
    {
        if (obj is RingPoint other) return this == other;
        else return false;
    }

    public override readonly int GetHashCode()
    {
        return _ring.GetHashCode() ^ _point.GetHashCode();
    }

    public override readonly string ToString()
    {
        return $"{_point} on ring sized {_ring.Size}";
    }


    public readonly int RightDistanceTo(RingPoint target)
    {
        int from = _point;
        int to = target._point;
        if (to < from) to += _ring.Size;

        return to - from;
    }

    public readonly int LeftDistanceTo(RingPoint target)
    {
        int from = _point;
        int to = target._point;
        if (from < to) from += _ring.Size;

        return from - to;
    }

    public readonly bool IsBetween(RingPoint left, RingPoint right)
    {
        int a = left._point;
        int b = _point;
        int c = right._point;

        if (b < a) b += _ring.Size;
        if (c < a) c += _ring.Size;

        return b < c;
    }
}