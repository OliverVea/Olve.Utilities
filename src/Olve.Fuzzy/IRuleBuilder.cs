namespace Olve.Fuzzy;

/// <summary>
/// Builds the condition of a fuzzy rule, started by <see cref="FuzzyRule.If"/>.
/// </summary>
public interface IRuleBuilder
{
    /// <summary>
    /// Attaches the rule's condition to <paramref name="consequent"/>.
    /// </summary>
    /// <param name="consequent">The consequent activated by the condition.</param>
    /// <returns>An applier for adding more consequents or else branches.</returns>
    IRuleApplier Then(IConsequent consequent);

    /// <summary>
    /// Combines the current condition with <paramref name="fuzzyCondition"/> using fuzzy AND (minimum).
    /// </summary>
    /// <param name="fuzzyCondition">The condition to combine with.</param>
    /// <returns>A builder for the combined condition.</returns>
    IRuleBuilder And(ICondition fuzzyCondition);

    /// <summary>
    /// Combines the current condition with <paramref name="fuzzyCondition"/> using fuzzy OR (maximum).
    /// </summary>
    /// <param name="fuzzyCondition">The condition to combine with.</param>
    /// <returns>A builder for the combined condition.</returns>
    IRuleBuilder Or(ICondition fuzzyCondition);
}
