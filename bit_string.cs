// 固定長のビット列を管理する.
// 先頭, 末尾への追加, ランダムアクセス等.
// 配列のインデックスにそのまま使える.
// @author Nauclhlt.
struct BitString32
{
    private static StringBuilder StrBuilder = new StringBuilder(64);

    private int _bit;
    private int _length;
    private readonly int _lengthMask;

    public int Bit => _bit;
    public int Length => _length;

    public bool this[int index]
    {
        get
        {
            if (index < 0 || index >= _length) return false;
            return (_bit & (1 << index)) != 0;
        }
        set
        {
            if (index < 0 || index >= _length) return;

            _bit |= 1 << index;
        }
    }

    public BitString32(int length, int bit)
    {
        if (length <= 0 || length > 32)
        {
            throw new Exception();
        }
        _bit = bit;
        _length = length;
        _lengthMask = 0;
        for (int i = 0; i < _length; i++)
        {
            _lengthMask |= 1 << i;
        }
    }

    public BitString32 Append(bool v)
    {
        if (v)
            return new BitString32(_length, (_bit >> 1) | (1 << (_length - 1)));
        else
            return new BitString32(_length, (_bit >> 1));
    }

    public BitString32 Prepend(bool v)
    {
        if (v)
            return new BitString32(_length, ((_bit << 1) | 1) & _lengthMask);
        else
            return new BitString32(_length, (_bit << 1) & _lengthMask);
    }
    
    public void Clear()
    {
        _bit = 0;
    }

    public override string ToString()
    {
        StrBuilder.Clear();
        for (int i = 0; i < _length; i++)
        {
            int bit = ((_bit & (1 << i)) >> i);
            StrBuilder.Append(bit);
        }

        return StrBuilder.ToString();
    }

    public static List<BitString32> GenerateAll(int length)
    {
        int n = 1 << length;
        List<BitString32> list = new List<BitString32>(n);
        for (int i = 0; i < n; i++)
        {
            list.Add(new BitString32(length, i));
        }

        return list;
    }

    public static implicit operator int(BitString32 bs)
    {
        return bs._bit;
    }
}