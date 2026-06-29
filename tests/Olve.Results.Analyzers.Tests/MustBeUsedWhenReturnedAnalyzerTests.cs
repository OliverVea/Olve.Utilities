namespace Olve.Results.Analyzers.Tests;

/// <summary>
/// Behavioural spec for ORES001 (<c>MustBeUsedWhenReturned</c>). Each test is a documented edge case:
/// the bare-statement and discard rules, and — the regression this suite was created to guard — the
/// distinction between awaiting a task-like wrapper around a result (a discard, flagged) and awaiting a
/// custom awaitable that merely yields a result on completion such as a TUnit assertion (an observation,
/// not flagged).
/// </summary>
public class MustBeUsedWhenReturnedAnalyzerTests
{
    // ----- Bare statements: the value is dropped on the floor -----

    [Test]
    public async Task BareInvocation_ReturningResult_IsFlagged()
    {
        var diagnostics = await AnalyzerRunner.GetOres001Async("Api.Sync();");

        await Assert.That(diagnostics.Length).IsEqualTo(1);
    }

    [Test]
    public async Task BareInvocation_ReturningGenericResult_IsFlagged()
    {
        var diagnostics = await AnalyzerRunner.GetOres001Async("Api.SyncGeneric();");

        await Assert.That(diagnostics.Length).IsEqualTo(1);
    }

    [Test]
    public async Task BareInvocation_ReturningUnmarkedType_IsNotFlagged()
    {
        var diagnostics = await AnalyzerRunner.GetOres001Async("Api.Plain();");

        await Assert.That(diagnostics.Length).IsEqualTo(0);
    }

    // ----- Observed: assigned or explicitly discarded -----

    [Test]
    public async Task AssignedToLocal_IsNotFlagged()
    {
        var diagnostics = await AnalyzerRunner.GetOres001Async("var observed = Api.Sync();");

        await Assert.That(diagnostics.Length).IsEqualTo(0);
    }

    [Test]
    public async Task ExplicitDiscard_IsNotFlagged()
    {
        var diagnostics = await AnalyzerRunner.GetOres001Async("_ = Api.Sync();");

        await Assert.That(diagnostics.Length).IsEqualTo(0);
    }

    // ----- await of a task-like wrapper around a result: still a discard -----

    [Test]
    public async Task AwaitedTaskOfResult_IsFlagged()
    {
        var diagnostics = await AnalyzerRunner.GetOres001Async("await Api.AsyncTask();");

        await Assert.That(diagnostics.Length).IsEqualTo(1);
    }

    [Test]
    public async Task AwaitedValueTaskOfResult_IsFlagged()
    {
        var diagnostics = await AnalyzerRunner.GetOres001Async("await Api.AsyncValueTask();");

        await Assert.That(diagnostics.Length).IsEqualTo(1);
    }

    [Test]
    public async Task AwaitedConfiguredTaskOfResult_IsFlagged()
    {
        // Regression guard: gating on the awaited operand type must still cover ConfiguredTaskAwaitable<T>,
        // i.e. `await foo.ConfigureAwait(false);` is a discard like any other.
        var diagnostics = await AnalyzerRunner.GetOres001Async("await Api.AsyncTask().ConfigureAwait(false);");

        await Assert.That(diagnostics.Length).IsEqualTo(1);
    }

    [Test]
    public async Task AwaitedTaskOfResult_Assigned_IsNotFlagged()
    {
        var diagnostics = await AnalyzerRunner.GetOres001Async("var observed = await Api.AsyncTask();");

        await Assert.That(diagnostics.Length).IsEqualTo(0);
    }

    [Test]
    public async Task AwaitedPlainTask_IsNotFlagged()
    {
        var diagnostics = await AnalyzerRunner.GetOres001Async("await Api.PlainTask();");

        await Assert.That(diagnostics.Length).IsEqualTo(0);
    }

    // ----- The motivating regression: awaiting a fluent assertion yields a Result but is an observation -----

    [Test]
    public async Task AwaitedCustomAwaitableYieldingResult_IsNotFlagged()
    {
        // `await Api.Assertion();` awaits a non-task-like awaitable whose GetResult() returns a marked
        // Result — the exact shape of `await Assert.That(result).Failed();`. The result was the asserted
        // input, not a returned value, so it must NOT be flagged.
        var diagnostics = await AnalyzerRunner.GetOres001Async("await Api.Assertion();");

        await Assert.That(diagnostics.Length).IsEqualTo(0);
    }
}
