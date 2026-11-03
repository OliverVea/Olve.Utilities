namespace Olve.Fuzzy;

/// <summary>
/// Entry point for building fuzzy rules with a fluent syntax.
/// </summary>
/// <example>
/// <code>
/// FuzzyRule.If(health.Is(Low)).Then(flee.Is(High));
/// </code>
/// </example>
public static class FuzzyRule
{
    /// <summary>
    /// Starts a rule with <paramref name="condition"/> as its antecedent.
    /// </summary>
    /// <param name="condition">The condition of the rule.</param>
    /// <returns>A builder to combine the condition further or attach consequents.</returns>
    public static IRuleBuilder If(ICondition condition) => new FuzzyRuleBuilder(condition);
}
