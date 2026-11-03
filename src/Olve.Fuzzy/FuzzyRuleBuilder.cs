namespace Olve.Fuzzy;

internal sealed class FuzzyRuleBuilder(ICondition condition) : IRuleApplier, IRuleBuilder
{
    public IRuleApplier Then(IConsequent consequent)
    {
        consequent.Add(condition);
        return this;
    }

    public IRuleApplier And(IConsequent consequent)
    {
        consequent.Add(condition);
        return this;
    }

    public IRuleApplier Else(IConsequent consequent)
    {
        var invertedCondition = Condition.Not(condition);
        consequent.Add(invertedCondition);
        return new FuzzyRuleBuilder(invertedCondition);
    }

    public IRuleBuilder ElseIf(ICondition fuzzyCondition)
    {
        var invertedCondition = Condition.Not(condition);
        return new FuzzyRuleBuilder(Condition.And(invertedCondition, fuzzyCondition));
    }

    public IRuleBuilder And(ICondition fuzzyCondition) =>
        new FuzzyRuleBuilder(Condition.And(condition, fuzzyCondition));

    public IRuleBuilder Or(ICondition fuzzyCondition) =>
        new FuzzyRuleBuilder(Condition.Or(condition, fuzzyCondition));
}
