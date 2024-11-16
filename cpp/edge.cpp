template <typename T>
struct Edge
{
    int From;
    int To;
    T Weight;

public:
    Edge(int from, int to, T weight)
    {
        From = from;
        To = to;
        Weight = weight;
    }
};