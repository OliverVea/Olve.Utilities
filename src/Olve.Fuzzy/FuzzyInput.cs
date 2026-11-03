namespace Olve.Fuzzy;

/// <summary>
/// A fuzzy input variable: a crisp value read through a function, and member functions keyed by antecedent.
/// </summary>
/// <param name="valueFunction">Returns the current crisp value of the input. It is called on every evaluation.</param>
public sealed class FuzzyInput(Func<double> valueFunction)
{
    private readonly Dictionary<int, IMemberFunction> _memberFunctions = new();

    /// <summary>
    /// Gets the current crisp value of the input.
    /// </summary>
    public double Value => valueFunction();

    /// <summary>
    /// Gets the member functions of the input, keyed by antecedent.
    /// </summary>
    public IReadOnlyDictionary<int, IMemberFunction> MemberFunctions => _memberFunctions;

    /// <summary>
    /// Sets the member function for <paramref name="antecedent"/>.
    /// </summary>
    /// <param name="antecedent">The key of the antecedent, e.g. a constant for "low".</param>
    /// <param name="memberFunction">The member function of the antecedent.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="antecedent"/> is already set.</exception>
    public void Set(int antecedent, IMemberFunction memberFunction)
    {
        if (_memberFunctions.ContainsKey(antecedent))
            throw new ArgumentException($"Member function of {antecedent} is already set. Please use different values for different antecedents.", nameof(antecedent));

        _memberFunctions.Add(antecedent, memberFunction);
    }

    /// <summary>
    /// Creates a condition that is true to the degree the input is a member of <paramref name="antecedent"/>.
    /// </summary>
    /// <param name="antecedent">The key of the antecedent.</param>
    /// <returns>The condition.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="antecedent"/> is not set.</exception>
    public ICondition Is(int antecedent)
    {
        if (_memberFunctions.TryGetValue(antecedent, out var memberFunction))
            return new Condition(memberFunction, valueFunction);

        throw new ArgumentException($"Member function of {antecedent} is not implemented.", nameof(antecedent));
    }

    /// <summary>
    /// Creates a condition that is the negation (1 - truth) of <see cref="Is"/>.
    /// </summary>
    /// <param name="antecedent">The key of the antecedent.</param>
    /// <returns>The negated condition.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="antecedent"/> is not set.</exception>
    public ICondition IsNot(int antecedent)
    {
        if (_memberFunctions.TryGetValue(antecedent, out var memberFunction))
            return Condition.Not(new Condition(memberFunction, valueFunction));

        throw new ArgumentException($"Member function of {antecedent} is not implemented.", nameof(antecedent));
    }
}
