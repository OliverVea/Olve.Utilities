namespace Olve.Fuzzy;

/// <summary>
/// A membership function mapping a crisp value to a degree of membership.
/// </summary>
public interface IMemberFunction
{
    /// <summary>
    /// Evaluates the degree of membership of <paramref name="x"/>.
    /// </summary>
    /// <param name="x">The crisp value.</param>
    /// <returns>The degree of membership, usually between 0 and 1.</returns>
    double Evaluate(double x);
}
