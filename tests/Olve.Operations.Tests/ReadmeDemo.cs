using Microsoft.Extensions.DependencyInjection;
using Olve.Results;

namespace Olve.Operations.Tests;

public class ReadmeDemo
{
    [Test]
    public async Task SyncOperationExample()
    {
        var op = new GreetOperation();

        var result = op.Execute("Alice");

        await Assert.That(result.Succeeded).IsTrue();
    }

    [Test]
    public async Task SyncOperationWithResultExample()
    {
        var op = new DoubleOperation();

        var result = op.Execute(21);

        await Assert.That(result.Succeeded).IsTrue();
        await Assert.That(result.Value).IsEqualTo(42);
    }

    [Test]
    public async Task AsyncOperationExample()
    {
        var op = new SlowGreetOperation();

        var result = await op.ExecuteAsync("Bob");

        await Assert.That(result.Succeeded).IsTrue();
    }

    [Test]
    public async Task AsyncOperationWithResultExample()
    {
        var op = new AsyncDoubleOperation();

        var result = await op.ExecuteAsync(21);

        await Assert.That(result.Succeeded).IsTrue();
        await Assert.That(result.Value).IsEqualTo(42);
    }

    [Test]
    public async Task FactoryExample()
    {
        var services = new ServiceCollection();
        services.AddTransient<DoubleOperation>();
        services.AddTransient<DoubleOperation.Factory>();
        using var sp = services.BuildServiceProvider();

        var factory = sp.GetRequiredService<DoubleOperation.Factory>();
        var op = factory.Build();
        var result = op.Execute(21);

        await Assert.That(result.Value).IsEqualTo(42);
    }

    // --- Example types used above ---

    private class GreetOperation : IOperation<string>
    {
        public Result Execute(string request) => Result.Success();
    }

    private class DoubleOperation : IOperation<int, int>
    {
        public Result<int> Execute(int request) => request * 2;

        public class Factory(IServiceProvider sp)
            : OperationFactory<DoubleOperation, int, int>(sp);
    }

    private class SlowGreetOperation : IAsyncOperation<string>
    {
        public async Task<Result> ExecuteAsync(string request, CancellationToken ct = default)
        {
            await Task.Delay(1, ct);
            return Result.Success();
        }
    }

    private class AsyncDoubleOperation : IAsyncOperation<int, int>
    {
        public async Task<Result<int>> ExecuteAsync(int request, CancellationToken ct = default)
        {
            await Task.Delay(1, ct);
            return request * 2;
        }
    }
}
