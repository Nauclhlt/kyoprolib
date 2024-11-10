// Suffix Array.
// 部分文字列の存在判定、数え上げ.
// 一応線形時間での構築.
public sealed class SuffixArray
{
    private string _source;
    private int _length;
    private int[] _suffixArray;

    public SuffixArray(string source)
    {
        _source = source;
        _length = _source.Length;

        Build();
    }

    public bool Contains(string s)
    {
        int left = 0;
        int right = _length - 1;
        StringComparer comp = StringComparer.Ordinal;

        while (right > left)
        {
            int mid = left + (right - left) / 2;

            string suffix = _source.Substring(_suffixArray[mid]);
            if (comp.Compare(s, suffix) < 0)
            {
                right = mid;
            }
            else
            {
                left = mid + 1;
            }
        }

        return _source.Substring(_suffixArray[left]).StartsWith(s, StringComparison.Ordinal);
    }

    public int CountOf(string s)
    {
        int lower = 0;
        {
            int left = 0;
            int right = _length - 1;
            StringComparer comp = StringComparer.Ordinal;

            while (right > left)
            {
                int mid = left + (right - left) / 2;

                string suffix = _source.Substring(_suffixArray[mid], int.Min(_length - _suffixArray[mid], s.Length));
                if (comp.Compare(s, suffix) > 0)
                {
                    left = mid + 1;
                }
                else
                {
                    right = mid;
                }
            }

            lower = left;
        }

        int upper = 0;
        {
            int left = 0;
            int right = _length;
            StringComparer comp = StringComparer.Ordinal;

            while (right > left)
            {
                int mid = left + (right - left) / 2;

                string suffix = _source.Substring(_suffixArray[mid], int.Min(_length - _suffixArray[mid], s.Length));
                if (comp.Compare(s, suffix) >= 0)
                {
                    left = mid + 1;
                }
                else
                {
                    right = mid;
                }
            }

            upper = left;
        }

        if (lower >= upper) return 0;
        else return upper - lower;
    }

    private void Build()
    {
        int[] str = new int[_length + 1];
        for (int i = 0; i < _length + 1; i++)
        {
            if (i == _length)
            {
                str[i] = 0;
            }
            else
            {
                str[i] = _source[i] - '$';
            }
        }

        int[] temp = SAIS(str, 128);
        _suffixArray = new int[_length];
        Array.Copy(temp, 1, _suffixArray, 0, _length);

        temp = null;
    }

    public override string ToString()
    {
        return $"[{string.Join(' ', _suffixArray)}]";
    }

    private int[] SAIS(int[] str, int charCount)
    {
        int n = str.Length;

        bool[] isS = new bool[n];
        isS[n - 1] = true;
        bool[] isLMS = new bool[n];
        List<int> lms = new();

        for (int i = n - 2; i >= 0; i--)
        {
            isS[i] = str[i] < str[i + 1] || (str[i] == str[i + 1] && isS[i + 1]);
        }

        for (int i = 0; i < n; i++)
        {
            if (isS[i] && (i == 0 || !isS[i - 1]))
            {
                isLMS[i] = true;
                lms.Add(i);
            }
        }

        int[] psa = InducedSort(str, lms, isS, charCount);
        int[] orderedLMS = new int[lms.Count];
        int index = 0;
        Console.Out.Flush();
        for (int i = 0; i < psa.Length; i++)
        {
            if (isLMS[psa[i]])
            {
                orderedLMS[index++] = psa[i];
            }
        }

        psa[orderedLMS[0]] = 0;
        int rank = 0;
        if (orderedLMS.Length > 1) psa[orderedLMS[1]] = ++rank;
        for (int i = 1; i < orderedLMS.Length - 1; i++)
        {
            bool diff = false;
            for (int j = 0; j < n; j++)
            {
                int p = orderedLMS[i] + j;
                int q = orderedLMS[i + 1] + j;
                if (str[p] != str[q] || isLMS[p] != isLMS[q]) 
                {
                    diff = true;
                    break;
                }
                if (j > 0 && isLMS[p])
                {
                    break;
                }
            }

            psa[orderedLMS[i + 1]] = diff ? ++rank : rank;
        }

        
        int[] nstr = new int[lms.Count];
        index = 0;
        for (int i = 0; i < n; i++)
        {
            if (isLMS[i])
            {
                nstr[index++] = psa[i];
            }
        }

        int[] lmssa;
        if (rank + 1 == lms.Count)
        {
            lmssa = orderedLMS;
        }
        else
        {
            lmssa = SAIS(nstr, rank + 1);
            for (int i = 0; i < lmssa.Length; i++)
            {
                lmssa[i] = lms[lmssa[i]];
            }
        }

        return InducedSort(str, lmssa, isS, charCount);
    }

    private int[] InducedSort(int[] str, List<int> lms, bool[] isS, int charCount)
    {
        int n = str.Length;

        int[] buckets = new int[n];

        int[] chars = new int[charCount + 1];
        for (int i = 0; i < n; i++)
        {
            chars[str[i] + 1]++;
        }

        for (int i = 0; i < charCount; i++)
        {
            chars[i + 1] += chars[i];
        }

        int[] count = new int[charCount];
        // LMSをbucketに後ろから詰める
        for (int i = lms.Count - 1; i >= 0; i--) {
            int c = str[lms[i]];
            buckets[chars[c + 1] - 1 - count[c]++] = lms[i];
        }

        Array.Clear(count);
        for (int i = 0; i < n; i++)
        {
            if (buckets[i] == 0 || isS[buckets[i] - 1]) continue;
            
            int c = str[buckets[i] - 1];
            buckets[chars[c] + count[c]++] = buckets[i] - 1;
        }

        Array.Clear(count);
        for (int i = n - 1; i >= 0; i--)
        {
            if (buckets[i] == 0 || !isS[buckets[i] - 1]) continue;
            int c = str[buckets[i] - 1];
            buckets[chars[c + 1] - 1 - count[c]++] = buckets[i] - 1;
        }

        return buckets;
    }

    private int[] InducedSort(int[] str, int[] lms, bool[] isS, int charCount)
    {
        int n = str.Length;

        int[] buckets = new int[n];

        int[] chars = new int[charCount + 1];
        for (int i = 0; i < n; i++)
        {
            chars[str[i] + 1]++;
        }

        for (int i = 0; i < charCount; i++)
        {
            chars[i + 1] += chars[i];
        }

        int[] count = new int[charCount];
        // LMSをbucketに後ろから詰める
        for (int i = lms.Length - 1; i >= 0; i--) {
            int c = str[lms[i]];
            buckets[chars[c + 1] - 1 - count[c]++] = lms[i];
        }

        Array.Clear(count);
        for (int i = 0; i < n; i++)
        {
            if (buckets[i] == 0 || isS[buckets[i] - 1]) continue;
            
            int c = str[buckets[i] - 1];
            buckets[chars[c] + count[c]++] = buckets[i] - 1;
        }

        Array.Clear(count);
        for (int i = n - 1; i >= 0; i--)
        {
            if (buckets[i] == 0 || !isS[buckets[i] - 1]) continue;
            int c = str[buckets[i] - 1];
            buckets[chars[c + 1] - 1 - count[c]++] = buckets[i] - 1;
        }

        return buckets;
    }
}