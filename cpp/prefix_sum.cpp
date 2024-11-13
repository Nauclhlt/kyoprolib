template <typename T>
class PrefixSum
{
private:
    vector<T> _sums;

public:
    PrefixSum(vector<T>& sequence)
    {
        _sums.resize(sequence.size() + 1);

        _sums[0] = 0;
        for (int i = 0; i < (int)sequence.size(); i++)
        {
            _sums[i + 1] = _sums[i] + sequence[i];
        }
    }

    T Sum(int l, int r)
    {
        return _sums[r] - _sums[l];
    }

    T AllSum()
    {
        return _sums[_sums.size() - 1];
    }

    vector<T>& GetArray()
    {
        return _sums;
    }
};