template <typename T>
class PrefixSum2D
{
private:
    vector<vector<T>> _sums;

public:
    PrefixSum2D(vector<vector<T>>& sequence)
    {
        int height = sequence.size();
        int width = sequence[0].size();

        _sums.resize(height + 1, vector<T>(width + 1, 0));

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                _sums[y + 1][x + 1] = _sums[y + 1][x] + _sums[y][x + 1] - _sums[y][x] + sequence[y][x];
            }
        }
    }

    T Sum(int startX, int startY, int endX, int endY)
    {
        return _sums[endY][endX] + _sums[startY][startX] - _sums[startY][endX] - _sums[endY][startX];
    }

    T AllSum()
    {
        return _sums[(int)_sums.size() - 1][(int)_sums[0].size() - 1];
    }
};