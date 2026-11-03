namespace Olve.Fuzzy;

internal sealed class Condition : ICondition
{
    private readonly Func<double> _evaluationFunc;

    internal Condition(IMemberFunction memberFunction, Func<double> func)
    {
        _evaluationFunc = () => memberFunction.Evaluate(func());
    }

    private Condition(Func<double> evaluationFunc)
    {
        _evaluationFunc = evaluationFunc;
    }

    double ICondition.Evaluate() => _evaluationFunc();

    internal static Condition And(ICondition a, ICondition b) =>
        new(() => Math.Min(a.Evaluate(), b.Evaluate()));

    internal static Condition Or(ICondition a, ICondition b) =>
        new(() => Math.Max(a.Evaluate(), b.Evaluate()));

    internal static Condition Not(ICondition a) =>
        new(() => 1 - a.Evaluate());
}
