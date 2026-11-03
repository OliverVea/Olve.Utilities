namespace Olve.Fuzzy;

/// <summary>
/// A fuzzy output variable: member functions keyed by consequent, defuzzified with the centroid method.
/// </summary>
/// <remarks>
/// Rules attach their conditions to the output's consequents, so the output holds the rule set.
/// Build a new output (and its rules) per rule set; an output is not safe to share across threads.
/// </remarks>
public sealed class FuzzyOutput
{
    private readonly Dictionary<int, Consequent> _fuzzyActions = new();

    /// <summary>
    /// Gets the consequent for <paramref name="consequent"/>, to be used with <see cref="IRuleBuilder.Then"/>.
    /// </summary>
    /// <param name="consequent">The key of the consequent.</param>
    /// <returns>The consequent.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when <paramref name="consequent"/> is not set.</exception>
    public IConsequent Is(int consequent) => _fuzzyActions[consequent];

    /// <summary>
    /// Evaluates all rules attached to this output and defuzzifies the result.
    /// </summary>
    /// <returns>The crisp output value, or 0 when no rule is active.</returns>
    public double Evaluate()
    {
        var centroids = _fuzzyActions.Values.Select(x => x.GetCentroid());
        return centroids.Sum().Value;
    }

    /// <summary>
    /// Sets the member function for <paramref name="consequent"/>.
    /// </summary>
    /// <param name="consequent">The key of the consequent, e.g. a constant for "high".</param>
    /// <param name="memberFunction">The member function of the consequent.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="consequent"/> is already set.</exception>
    public void Set(int consequent, IHasCentroid memberFunction)
    {
        if (_fuzzyActions.ContainsKey(consequent))
            throw new ArgumentException($"Member function of {consequent} is already set. Please use different values for different consequents.", nameof(consequent));

        _fuzzyActions[consequent] = new Consequent(memberFunction);
    }
}
