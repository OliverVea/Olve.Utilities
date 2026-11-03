namespace Olve.Fuzzy.MemberFunctions;

/// <summary>
/// A ramp from 0 at <c>min</c> to 1 at <c>max</c>, flat outside. When <c>max</c> is below <c>min</c>, the ramp descends.
/// </summary>
public sealed class Line : IMemberFunction
{
    private readonly double _min;
    private readonly double _max;
    private readonly bool _reversed;

    /// <summary>
    /// Creates a ramp that is 0 at <paramref name="min"/> and 1 at <paramref name="max"/>.
    /// </summary>
    /// <param name="min">Where the membership is 0.</param>
    /// <param name="max">Where the membership is 1.</param>
    public Line(double min, double max)
    {
        _reversed = max < min;
        (_min, _max) = _reversed ? (max, min) : (min, max);
    }

    /// <inheritdoc />
    public double Evaluate(double x)
    {
        if (x < _min) return _reversed ? 1 : 0;
        if (x > _max) return _reversed ? 0 : 1;

        var slope = 1 / (_max - _min);

        return _reversed
            ? 1 - (x - _min) * slope
            : (x - _min) * slope;
    }
}
