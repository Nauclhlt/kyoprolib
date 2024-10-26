public struct ChmaxChminAdd<T> : IEquatable<ChmaxChminAdd<T>> where T : INumber<T>
{
    public T Chmax;
    public T Chmin;
    public T Add;
    public ChmaxChminAdd(T chmax, T chmin, T add)
    {
        Chmax = chmax;
        Chmin = chmin;
        Add = add;
    }

    public void Composite(ChmaxChminAdd<T> a)
    {
        a.Chmax -= Add;
        a.Chmin -= Add;

        Chmax = T.Max(Chmax, a.Chmax);
        Chmin = T.Min(a.Chmin, T.Max(Chmin, a.Chmax));
        Add += a.Add;
    }

    public void CompositeChmax(T chmax)
    {
        chmax -= Add;
        Chmax = T.Max(Chmax, chmax);
        Chmin = T.Max(Chmin, chmax);
    }

    public void CompositeChmin(T chmin)
    {
        chmin -= Add;
        Chmin = T.Min(Chmin, chmin);
    }

    public void CompositeAdd(T add)
    {
        Add += add;
    }

    public readonly T Apply(T x)
    {
        return T.Min(T.Max(x, Chmax), Chmin) + Add;
    }

    public readonly bool Equals(ChmaxChminAdd<T> other)
    {
        return Chmax == other.Chmax && Chmin == other.Chmin && Add == other.Add;
    }
}