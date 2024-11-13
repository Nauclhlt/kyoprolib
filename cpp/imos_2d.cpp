template <typename T>
class Imos2D
{
private:
    vector<vector<T>> _data;
    int _width;
    int _height;

public:
    Imos2D(vector<vector<T>> data)
    {
        _data = data;
        _height = data.size();
        _width = data[0].size();
    }

    Imos2D(int h, int w)
    {
        _data.resize(h, vector<T>(w, 0));
        _width = w;
        _height = h;
    }

    void AddQuery(int startX, int startY, int endX, int endY, T value)
    {
        _data[startY][startX] += value;
        if (endX < _width)
        {
            _data[startY][endX] -= value;
        }
        if (endY < _height)
        {
            _data[endY][startX] -= value;
        }
        if (endX < _width && endY < _height)
        {
            _data[endY][endX] += value;
        }
    }

    void AddQueryLen(int x, int y, int w, int h, T value)
    {
        AddQuery(x, y, x + w, y + h, value);
    }

    void Accumulate()
    {
        for (int x = 1; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                _data[y][x] += _data[y][x - 1];
            }
        }

        for (int y = 1; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                _data[y][x] += _data[y - 1][x];
            }
        }
    }

    vector<vector<T>> GetData()
    {
        return _data;
    }
};