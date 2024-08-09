// 有向および無向辺を保持する.
// @author Nauclhlt.
public readonly struct Edge<T> : IEquatable<Edge<T>>, IComparable<Edge<T>> where T : struct, INumber<T>
{
    public readonly int To;
    public readonly int From;
    public readonly T Weight;

    public Edge(int to, T weight)
    {
        this.To = to;
        this.Weight = weight;
    }

    public Edge(int from, int to, T weight)
    {
        this.To = to;
        this.From = from;
        this.Weight = weight;
    }

    public override bool Equals(object obj)
    {
        if (obj is Edge<T> edge)
        {
            return this.Equals(edge);
        }
        else
        {
            return false;
        }
    }

    public int CompareTo(Edge<T> other)
    {
        return Weight.CompareTo(other.Weight);
    }

    public bool Equals(Edge<T> edge)
    {
        return To == edge.To && From == edge.From && Weight == edge.Weight;
    }

    public override int GetHashCode()
    {
        return (To, From, Weight).GetHashCode();
    }

    public static bool operator ==(Edge<T> left, Edge<T> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Edge<T> left, Edge<T> right)
    {
        return !left.Equals(right);
    }
}