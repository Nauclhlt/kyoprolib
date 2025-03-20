/// <summary>
/// グリッド上でのunion-findを扱う。
/// </summary>
public sealed class GridUnionFind
{
    private UnionFind _uf;
    private int _width;
    private int _height;

    public int Width => _width;
    public int Height => _height;
    public UnionFind Inner => _uf;

    public GridUnionFind(int height, int width)
    {
        _width = width;
        _height = height;
        _uf = new UnionFind(width * height);
    }

    /// <summary>
    /// (x1, y1)と(x2, y2)を併合する。計算量: O(α(HW))
    /// </summary>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    public void Unite(int x1, int y1, int x2, int y2)
    {
        _uf.Unite(Id(x1, y1), Id(x2, y2));
    }

    /// <summary>
    /// (x1, y1)と(x2, y2)が同じ集合に属するかを返す。計算量: O(α(HW))
    /// </summary>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    /// <returns></returns>
    public bool Same(int x1, int y1, int x2, int y2)
    {
        return _uf.Same(Id(x1, y1), Id(x2, y2));
    }

    /// <summary>
    /// (x, y)が属する集合の代表元を返す。計算量: O(α(HW))
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public int Root(int x, int y)
    {
        return _uf.Root(Id(x, y));
    }

    /// <summary>
    /// y*W+xを返す。計算量: O(1)
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Id(int x, int y)
    {
        return y * _width + x;
    }
}