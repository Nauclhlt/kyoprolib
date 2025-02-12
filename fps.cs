// FPSを扱う用のクラス.
// 和・差: O(n)
// 積(畳み込み): O(nlogn)
// 先頭n項inv: O(nlogn)
// @author Nauclhlt.
// depends on: Convolution
public sealed class FPS
{
    private const long Mod = 998244353L;
    private static Convolution Conv = new(Mod);

    private long[] _coef;

    public long[] Coef => _coef;
    public int Length => _coef.Length;
    public int MaxExponent => _coef.Length - 1;

    public long this[int index]
    {
        get => _coef[index];
        set => _coef[index] = value;
    }

    public FPS(long[] sequence)
    {
        _coef = sequence;
    }

    private FPS(long[] sequence, int start, int length)
    {
        _coef = sequence;
    }

    public static FPS CopyCreate(long[] sequence)
    {
        long[] copy = new long[sequence.Length];
        Array.Copy(sequence, copy, sequence.Length);
        return new(copy);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long SafeMod(long a)
    {
        return ((a % Mod) + Mod) % Mod;
    }

    public void Resize(int length)
    {
        Array.Resize(ref _coef, length);
    }

    public FPS GetResized(int length)
    {
        long[] res = new long[length];
        Array.Copy(_coef, res, res.Length);
        return new(res);
    }

    public static FPS operator -(FPS target)
    {
        long[] res = new long[target.Length];
        for (int i = 0; i < target.Length; i++) res[i] = SafeMod(-target[i]);

        return new(res);
    }

    public static FPS operator +(FPS left, FPS right)
    {
        int len = int.Max(left.Length, right.Length);
        long[] res = new long[len];
        for (int i = 0; i < left.Length; i++) res[i] += left[i];
        for (int i = 0; i < right.Length; i++) res[i] = SafeMod(res[i] + right[i]);

        return new (res);
    }

    public static FPS operator -(FPS left, FPS right)
    {
        int len = int.Max(left.Length, right.Length);
        long[] res = new long[len];
        for (int i = 0; i < left.Length; i++) res[i] += left[i];
        for (int i = 0; i < right.Length; i++) res[i] = SafeMod(res[i] - right[i]);

        return new (res);
    }

    public static FPS operator *(FPS left, FPS right)
    {
        long[] res = Conv.CalcConvolution(left.Coef, right.Coef);
        int len = left.MaxExponent + right.MaxExponent + 1;
        Array.Resize(ref res, len);
        return new(res);
    }

    public FPS Inv(int n)
    {
        if (_coef[0] == 0) throw new Exception("No Inv");
        FPS g = new(new long[]{ Convolution.CalcPow(_coef[0], Mod - 2, Mod) });
        int k = 1;
        while (k < n)
        {
            k *= 2;
            FPS fk = GetResized(int.Min(Length, k)); // 1, -1
            FPS fg = fk * g;
            for (int i = 0; i < fg.Length; i++) fg[i] = SafeMod(-fg[i]);
            fg[0] = SafeMod(fg[0] + 2);
            g *= fg;
            g.Resize(k);
        }
        g.Resize(n);

        return g;
    }
}