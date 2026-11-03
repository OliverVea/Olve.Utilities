using Olve.Fuzzy.MemberFunctions;

namespace Olve.Fuzzy.Tests;

public class ErrorMessageTests
{
    public static IEnumerable<Func<(Action Act, string Message)>> Cases()
    {
        yield return () => (() =>
        {
            var input = new FuzzyInput(() => 0);
            input.Set(1, new Value(0));
            input.Set(1, new Value(0));
        }, "Member function of 1 is already set. Please use different values for different antecedents. (Parameter 'antecedent')");

        yield return () => (() => new FuzzyInput(() => 0).Is(2),
            "Member function of 2 is not implemented. (Parameter 'antecedent')");

        yield return () => (() => new FuzzyInput(() => 0).IsNot(3),
            "Member function of 3 is not implemented. (Parameter 'antecedent')");

        yield return () => (() =>
        {
            var output = new FuzzyOutput();
            output.Set(4, new Triangle(0, 1, 2));
            output.Set(4, new Triangle(0, 1, 2));
        }, "Member function of 4 is already set. Please use different values for different consequents. (Parameter 'consequent')");
    }

    [Test]
    [MethodDataSource(nameof(Cases))]
    public async Task Misuse_throws_ArgumentException_with_message((Action Act, string Message) testCase)
    {
        await Assert.That(testCase.Act).Throws<ArgumentException>().WithMessage(testCase.Message);
    }
}
