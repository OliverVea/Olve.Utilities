namespace Olve.Fuzzy;

/// <summary>
/// A fuzzy condition (antecedent) that evaluates to a degree of truth, usually between 0 and 1.
/// </summary>
/// <remarks>
/// Conditions are created by <see cref="FuzzyInput.Is"/> and <see cref="FuzzyInput.IsNot"/>
/// and combined with the rule builder. They cannot be implemented outside this library.
/// </remarks>
public interface ICondition
{
    internal double Evaluate();
}
