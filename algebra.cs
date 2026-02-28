public readonly struct PrimitiveGroup<T> : ICommutativeGroup<T> where T : INumber<T>
{
    public T Inverse(T x) => -x;
    public T Op(T x, T y) => x + y;
    public T Identity => T.Zero;
}

public sealed class AlgPrefixSum<TSet, T> where T : ICommutativeGroup<TSet>
{
    private static readonly T _group = default;
    private TSet[] _prefix;

    public AlgPrefixSum(TSet[] array)
    {
        _prefix = new TSet[array.Length + 1];
        _prefix[0] = _group.Identity;
        for (int i = 1; i <= array.Length; i++)
        {
            _prefix[i] = _group.Op(_prefix[i - 1], array[i - 1]);
        }
    }

    public TSet Sum(int l, int r)
    {
        return _group.Op(_prefix[r], _group.Inverse(_prefix[l]));
    }
}

public interface ICommutative<TSet> { }

public interface IInverse<TSet>
{
    public TSet Inverse(TSet x);
}

public interface IIdentity<TSet>
{
    public TSet Identity { get; }
}

public interface ISemiGroup<TSet>
{
    public TSet Op(TSet a, TSet b);
}

public interface IMonoid<TSet> : ISemiGroup<TSet>, IIdentity<TSet>
{
}

public interface IGroup<TSet> : IMonoid<TSet>, IInverse<TSet>
{
}

public interface ICommutativeGroup<TSet> : IGroup<TSet>, ICommutativeMonoid<TSet>, ICommutative<TSet>
{
}

public interface ICommutativeMonoid<TSet> : IMonoid<TSet>, ICommutative<TSet>
{
}

public interface ISemiRing<TSet, out TAdd, out TMul> where TAdd : ICommutativeMonoid<TSet> where TMul : IMonoid<TSet>
{
    public TAdd Addition { get; }
    public TMul Multiply { get; }
}

public interface IRing<TSet, out TAdd, out TMul> : ISemiRing<TSet, TAdd, TMul> where TAdd : ICommutativeGroup<TSet> where TMul : IMonoid<TSet>
{
}

public interface IField<TSet, out TAdd, out TMul> : IRing<TSet, TAdd, TMul> where TAdd : ICommutativeGroup<TSet> where TMul : ICommutativeGroup<TSet>
{
}