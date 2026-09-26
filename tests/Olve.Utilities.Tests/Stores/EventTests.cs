using Olve.Utilities.Stores;

namespace Olve.Utilities.Tests.Stores;

public class EventTests
{
    [Test]
    public async Task Unsubscribe_StopsHandlerFromRunning()
    {
        var ev = new Event<int>();
        var calls = 0;
        Action<int> handler = _ => calls++;

        ev.Subscribe(handler);
        ev.Invoke(1);
        ev.Unsubscribe(handler);
        ev.Invoke(2);

        await Assert.That(calls).IsEqualTo(1);
    }

    [Test]
    public async Task ConcurrentSubscribeAndUnsubscribe_LosesNoChanges()
    {
        // Non-atomic `_handlers += handler` loses writes under contention; the CAS loop does not.
        var ev = new Event<int>();
        var calls = 0;
        const int threadCount = 8;
        using var start = new Barrier(threadCount);

        var threads = Enumerable.Range(0, threadCount).Select(_ => new Thread(() =>
        {
            start.SignalAndWait();
            for (var i = 0; i < 20_000; i++)
            {
                var n = i; // distinct closure per handler, so unsubscribe can't remove a sibling's
                Action<int> handler = _ => calls += n + 1;
                ev.Subscribe(handler);
                ev.Unsubscribe(handler);
            }
        })).ToList();

        threads.ForEach(t => t.Start());
        threads.ForEach(t => t.Join());

        ev.Invoke(0); // a lost unsubscribe leaves a handler behind, which runs here
        await Assert.That(calls).IsEqualTo(0);
    }

    [Test]
    public async Task Invoke_NoHandlers_NoOp()
    {
        var ev = new Event<int>();

        await Assert.That(() => ev.Invoke(1)).ThrowsNothing();
    }

    [Test]
    public async Task Invoke_RunsHandlersInSubscriptionOrder()
    {
        var ev = new Event<int>();
        var calls = new List<string>();
        ev.Subscribe(_ => calls.Add("a"));
        ev.Subscribe(_ => calls.Add("b"));
        ev.Subscribe(_ => calls.Add("c"));

        ev.Invoke(1);

        await Assert.That(calls).IsEquivalentTo(["a", "b", "c"], TUnit.Assertions.Enums.CollectionOrdering.Matching);
    }

    [Test]
    public async Task Invoke_OneHandlerThrows_LaterHandlersStillRun()
    {
        var ev = new Event<int>();
        var calls = new List<string>();
        ev.Subscribe(_ => calls.Add("a"));
        ev.Subscribe(_ => throw new InvalidOperationException("boom"));
        ev.Subscribe(_ => calls.Add("c"));

        try
        {
            ev.Invoke(1);
        }
        catch
        {
            // Asserted separately in Invoke_HandlerThrows_DoesNotPropagate; here only the siblings matter.
        }

        await Assert.That(calls).IsEquivalentTo(["a", "c"], TUnit.Assertions.Enums.CollectionOrdering.Matching);
    }

    [Test]
    public async Task Invoke_HandlerThrows_DoesNotPropagate()
    {
        var ev = new Event<int>();
        ev.Subscribe(_ => throw new InvalidOperationException("boom"));

        await Assert.That(() => ev.Invoke(1)).ThrowsNothing();
    }

    [Test]
    [NotInParallel]
    public async Task Invoke_HandlerThrows_ForwardsToSink()
    {
        var ev = new Event<int>();
        var thrown = new InvalidOperationException("boom");
        ev.Subscribe(_ => throw thrown);

        var received = new List<Exception>();
        EventDispatch.OnHandlerException = ex =>
        {
            lock (received)
            {
                received.Add(ex);
            }
        };
        try
        {
            ev.Invoke(1);
        }
        finally
        {
            EventDispatch.OnHandlerException = null;
        }

        await Assert.That(received).Contains(thrown);
    }

    [Test]
    [NotInParallel]
    public async Task Invoke_SinkThrows_DoesNotPropagateAndLaterHandlersStillRun()
    {
        var ev = new Event<int>();
        var laterRan = false;
        ev.Subscribe(_ => throw new InvalidOperationException("boom"));
        ev.Subscribe(_ => laterRan = true);

        EventDispatch.OnHandlerException = _ => throw new InvalidOperationException("sink boom");
        try
        {
            await Assert.That(() => ev.Invoke(1)).ThrowsNothing();
        }
        finally
        {
            EventDispatch.OnHandlerException = null;
        }

        await Assert.That(laterRan).IsTrue();
    }

    [Test]
    public async Task Invoke_StructPayload_DoesNotAllocate()
    {
        var ev = new Event<EntityUpdated<string, int>>();
        var seen = 0;
        ev.Subscribe(e => seen += e.Id);
        ev.Subscribe(e => seen += e.After.Length);
        var payload = new EntityUpdated<string, int>(1, "before", "after");
        ev.Invoke(payload); // warm up

        var before = GC.GetAllocatedBytesForCurrentThread();
        for (var i = 0; i < 100; i++) ev.Invoke(payload);
        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        await Assert.That(allocated).IsEqualTo(0);
        await Assert.That(seen).IsGreaterThan(0);
    }

    [Test]
    public async Task NonGeneric_Invoke_RunsHandlersInOrder_AndIsolatesThrows()
    {
        var ev = new Event();
        var calls = new List<int>();
        Action third = () => calls.Add(3);
        ev.Subscribe(() => calls.Add(1));
        ev.Subscribe(() => throw new InvalidOperationException("boom"));
        ev.Subscribe(third);

        ev.Invoke();
        ev.Unsubscribe(third);
        ev.Invoke();

        await Assert.That(calls).IsEquivalentTo([1, 3, 1], TUnit.Assertions.Enums.CollectionOrdering.Matching);
    }
}
