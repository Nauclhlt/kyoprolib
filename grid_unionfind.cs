// グリッド上で使えるUnionFind. (x, y) -> H * y + xと変換される.
// Depends on: UnionFind
// @author Nauclhlt.
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

    public void Unite(int x1, int y1, int x2, int y2)
    {
        _uf.Unite(Id(x1, y1), Id(x2, y2));
    }

    public bool Same(int x1, int y1, int x2, int y2)
    {
        return _uf.Same(Id(x1, y1), Id(x2, y2));
    }

    public int Root(int x, int y)
    {
        return _uf.Root(Id(x, y));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Id(int x, int y)
    {
        return y * _width + x;
    }
}