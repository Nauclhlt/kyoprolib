/// <summary>
/// CHMAX, CHMIN, ADDの3つをいい感じに合成する。
/// </summary>
/// <typeparam name="T">型: INumber</typeparam>
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

    /// <summary>
    /// aを合成する。計算量: O(1)
    /// </summary>
    /// <param name="a"></param>
    public void Compose(ChmaxChminAdd<T> a)
    {
        a.Chmax -= Add;
        a.Chmin -= Add;

        Chmax = T.Max(Chmax, a.Chmax);
        Chmin = T.Min(a.Chmin, T.Max(Chmin, a.Chmax));
        Add += a.Add;
    }

    /// <summary>
    /// chmaxを合成する。計算量: O(1)
    /// </summary>
    /// <param name="chmax"></param>
    public void ComposeChmax(T chmax)
    {
        chmax -= Add;
        Chmax = T.Max(Chmax, chmax);
        Chmin = T.Max(Chmin, chmax);
    }

    /// <summary>
    /// chminを合成する。計算量: O(1)
    /// </summary>
    /// <param name="chmin"></param>
    public void ComposeChmin(T chmin)
    {
        chmin -= Add;
        Chmin = T.Min(Chmin, chmin);
    }

    /// <summary>
    /// addを合成する。計算量: O(1)
    /// </summary>
    /// <param name="add"></param>
    public void ComposeAdd(T add)
    {
        Add += add;
    }

    /// <summary>
    /// xに対して変換を適用した値を返す。計算量: O(1)
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public readonly T Apply(T x)
    {
        return T.Min(T.Max(x, Chmax), Chmin) + Add;
    }

    public readonly bool Equals(ChmaxChminAdd<T> other)
    {
        return Chmax == other.Chmax && Chmin == other.Chmin && Add == other.Add;
    }
}