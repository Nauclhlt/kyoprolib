public abstract class BeamSearch<TScore, TState> where TScore : INumber<TScore>
{
    public int BeamWidth
    {
        get; set;
    } = 1;

    public long Duration
    {
        get; set;
    } = 1900;

    protected abstract TScore CalcScore(TState state);
    protected abstract void GenerateNextStates(TState current, in List<TState> buffer);

    private Stopwatch _stopwatch;

    public TState ExecuteTime(TState initialState)
    {
        List<TState> current = new(BeamWidth);
        current.Add(initialState);
        List<TState> buffer = new();
        PriorityQueue<TState, TScore> queue = new(ReverseComparer<TScore>.Default);

        _stopwatch = Stopwatch.StartNew();

        while (_stopwatch.ElapsedMilliseconds < Duration)
        {
            for (int i = 0; i < current.Count; i++)
            {
                buffer.Clear();
                GenerateNextStates(current[i], buffer);
                for (int j = 0; j < buffer.Count; j++)
                {
                    queue.Enqueue(buffer[j], CalcScore(buffer[j]));
                }
            }

            current.Clear();
            while (current.Count < BeamWidth && queue.Count > 0)
            {
                current.Add(queue.Dequeue());
            }

            queue.Clear();
        }

        return current[0];
    }

    public TState ExecuteCount(TState initialState, int count)
    {
        List<TState> current = new(BeamWidth);
        current.Add(initialState);
        List<TState> buffer = new();
        PriorityQueue<TState, TScore> queue = new(ReverseComparer<TScore>.Default);

        _stopwatch = Stopwatch.StartNew();

        for (int t = 0; t < count; t++)
        {
            for (int i = 0; i < current.Count; i++)
            {
                buffer.Clear();
                GenerateNextStates(current[i], buffer);
                for (int j = 0; j < buffer.Count; j++)
                {
                    queue.Enqueue(buffer[j], CalcScore(buffer[j]));
                }
            }

            current.Clear();
            while (current.Count < BeamWidth && queue.Count > 0)
            {
                current.Add(queue.Dequeue());
            }

            queue.Clear();
        }

        return current[0];
    }
}