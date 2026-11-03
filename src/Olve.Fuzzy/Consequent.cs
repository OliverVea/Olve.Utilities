namespace Olve.Fuzzy;

internal sealed class Consequent(IHasCentroid memberFunction) : IConsequent
{
    private readonly List<ICondition> _fuzzyConditions = [];

    void IConsequent.Add(ICondition condition) => _fuzzyConditions.Add(condition);

    internal Centroid GetCentroid()
    {
        if (_fuzzyConditions.Count == 0) return new Centroid(0, 0);

        var value = _fuzzyConditions.Select(x => x.Evaluate()).Max();
        return memberFunction.GetCentroid(value);
    }
}
