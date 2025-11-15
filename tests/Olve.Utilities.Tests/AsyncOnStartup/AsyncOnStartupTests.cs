using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Olve.Utilities.AsyncOnStartup;

namespace Olve.Utilities.Tests.AsyncOnStartup;

public class AsyncOnStartupTests
{
    [Test]
    [NotInParallel(nameof(AsyncOnStartupTests))]
    public async Task RunAsyncOnStartup_WithSingleOperation_ExecutesOperation()
    {
        // Arrange
        var services = new ServiceCollection();

        var service = new StartupOperation();

        services.AddSingleton<IAsyncOnStartup>(service);

        var serviceProvider = services.BuildServiceProvider();

        var executedBefore = service.Executed;

        // Act
        await serviceProvider.RunAsyncOnStartup();

        // Assert
        var executedAfter = service.Executed;

        await Assert
            .That(executedBefore)
            .IsFalse();
        await Assert
            .That(executedAfter)
            .IsTrue();
    }

    [Test]
    public async Task RunAsyncOnStartup_WithSeveralTasksOfDifferentPriority_ExecutesInCorrectOrder()
    {
        // Arrange
        var services = new ServiceCollection();

        var executed = -1;
        var dict = new ConcurrentDictionary<int, int>();

        var startupOperations = Enumerable
            .Range(0, 100)
            .Select(x => new StartupOperation(x, () => { dict[x] = Interlocked.Increment(ref executed); }))
            .Shuffle()
            .ToList();

        foreach (var operation in startupOperations)
        {
            services.AddSingleton<IAsyncOnStartup>(operation);
        }

        var serviceProvider = services.BuildServiceProvider();

        // Act
        await serviceProvider.RunAsyncOnStartup();

        // Assert
        foreach (var (priority, executionOrder) in dict)
        {
            await Assert
                .That(executionOrder)
                .IsEqualTo(priority);
        }
    }


    private class StartupOperation(int priority = 0, Action? onExecution = null) : IAsyncOnStartup
    {
        public bool Executed { get; private set; }
        public int Priority => priority;

        public Task OnStartupAsync(CancellationToken cancellationToken = default)
        {
            onExecution?.Invoke();
            Executed = true;
            return Task.CompletedTask;
        }
    }
}