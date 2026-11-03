namespace Olve.Fuzzy;

/// <summary>
/// Continues a fuzzy rule after <see cref="IRuleBuilder.Then"/>.
/// </summary>
public interface IRuleApplier
{
    /// <summary>
    /// Attaches the same condition to another <paramref name="consequent"/>.
    /// </summary>
    /// <param name="consequent">The additional consequent.</param>
    /// <returns>The current applier.</returns>
    IRuleApplier And(IConsequent consequent);

    /// <summary>
    /// Attaches the negation of the condition (1 - truth) to <paramref name="consequent"/>.
    /// </summary>
    /// <param name="consequent">The consequent activated when the condition is false.</param>
    /// <returns>An applier for the negated condition.</returns>
    IRuleApplier Else(IConsequent consequent);

    /// <summary>
    /// Starts a new condition that holds when the current condition is false and <paramref name="fuzzyCondition"/> is true.
    /// </summary>
    /// <param name="fuzzyCondition">The condition of the else-if branch.</param>
    /// <returns>A builder for the combined condition.</returns>
    IRuleBuilder ElseIf(ICondition fuzzyCondition);
}
