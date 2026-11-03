namespace Olve.Fuzzy.MemberFunctions;

/// <summary>
/// A crisp threshold: 1 at or above the value, otherwise 0.
/// </summary>
/// <param name="value">The threshold.</param>
public sealed class AtLeast(double value) : IMemberFunction
{
    /// <inheritdoc />
    public double Evaluate(double x) => x >= value ? 1 : 0;
}
