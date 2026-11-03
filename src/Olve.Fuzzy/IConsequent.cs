namespace Olve.Fuzzy;

/// <summary>
/// A fuzzy consequent: one member function of a <see cref="FuzzyOutput"/> that rules can activate.
/// </summary>
/// <remarks>
/// Consequents are created by <see cref="FuzzyOutput.Is"/>. They cannot be implemented outside this library.
/// </remarks>
public interface IConsequent
{
    internal void Add(ICondition condition);
}
