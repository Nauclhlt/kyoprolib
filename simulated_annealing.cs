public abstract class SimulatedAnnealing<TScore, TState> where TScore : INumber<TScore>
{
    public double InitialTemperature
    {
        get; set;
    } = 100.0;

    public double FinalTemperature
    {
        get; set;
    } = 1.0;

    public long Duration
    {
        get; set;
    } = 1950;

    public long Attempts => _attempts;

    private Stopwatch _stopwatch;
    private long _attempts = 0;

    protected virtual double CalcTemperature(double progress, long elapsed)
    {
        return InitialTemperature + (FinalTemperature - InitialTemperature) * progress;
    }

    protected virtual double GetTransitionProbability(TScore delta, double temperature)
    {
        return Math.Exp(double.CreateSaturating(delta) / temperature);
    }

    protected virtual TScore GetDelta(TScore current, TScore next)
    {
        return next - current;
    }

    protected abstract TScore CalcScore(TState state);
    protected abstract TState GetNeighbor(TState current);

    public TState Execute(TState initialState, bool log = false)
    {
        _stopwatch = new ();

        _stopwatch.Start();

        Random random = new();

        TState current = initialState;
        TScore currentScore = CalcScore(current);

        while (_stopwatch.ElapsedMilliseconds < Duration)
        {
            TState neighbor = GetNeighbor(current);
            TScore neighborScore = CalcScore(neighbor);

            TScore delta = GetDelta(currentScore, neighborScore);

            double temp = CalcTemperature(_stopwatch.ElapsedMilliseconds / (double)Duration, _stopwatch.ElapsedMilliseconds);
            double prob = GetTransitionProbability(delta, temp);

            if (random.NextDouble() <= prob)
            {
                if (currentScore != neighborScore)
                {
                    Console.Error.WriteLine($"Transition  {currentScore} --> {neighborScore}");
                }
                current = neighbor;
                currentScore = neighborScore;
            }

            _attempts++;
        }

        _stopwatch.Stop();

        return current;
    }
}