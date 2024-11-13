template <typename T>
class Imos
{
private:
    vector<T> _data;

public:
    Imos(int length)
    {
        _data.resize(length, 0);
    }

    Imos(vector<T>& array)
    {
        _data = array;
    }

    void AddQueryLen(int start, int length, T value)
    {
        AddQuery(start, start + length, value);
    }

    void AddQuery(int start, int end, T value)
    {
        _data[start] += value;
        if (end < (int)_data.size())
        {
            _data[end] -= value;
        }
    }

    void Accumulate()
    {
        for (int i = 1; i < (int)_data.size(); i++)
        {
            _data[i] += _data[i - 1];
        }
    }

    vector<T> GetData()
    {
        return _data;
    }
};