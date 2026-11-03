namespace Olve.Fuzzy.MemberFunctions;

/// <summary>
/// A single crisp value: 1 within <paramref name="tolerance"/> of the value, otherwise 0.
/// </summary>
/// <param name="value">The value.</param>
/// <param name="tolerance">The tolerance, exclusive.</param>
public sealed class Value(double value, double tolerance = 0.00001d) : IMemberFunction
{
    /// <inheritdoc />
    public double Evaluate(double x) => Math.Abs(x - value) < tolerance ? 1 : 0;
}
