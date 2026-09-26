using Olve.Utilities.Stores;

namespace Olve.Utilities.Tests.Stores;

public class EventTests
{
    [Test]
    public async Task SubscriberCount_TracksSubscribeAndUnsubscribe()
    {
        var ev = new Event<int>();
        Action<int> handler = _ => { };

        ev.Subscribe(handler);
        await Assert.That(ev.SubscriberCount).IsEqualTo(1);

        ev.Unsubscribe(handler);
        await Assert.That(ev.SubscriberCount).IsEqualTo(0);
    }

    [Test]
    public async Task ConcurrentSubscribeAndUnsubscribe_LosesNoChanges()
    {
        // Non-atomic `_handlers += handler` loses writes under contention; the CAS loop does not.
        var ev = new Event<int>();
        const int threadCount = 8;
        using var start = new Barrier(threadCount);

        var threads = Enumerable.Range(0, threadCount).Select(_ => new Thread(() =>
        {
            start.SignalAndWait();
            for (var i = 0; i < 20_000; i++)
            {
                Action<int> handler = _ => { };
                ev.Subscribe(handler);
                ev.Unsubscribe(handler);
            }
        })).ToList();

        threads.ForEach(t => t.Start());
        threads.ForEach(t => t.Join());

        await Assert.That(ev.SubscriberCount).IsEqualTo(0);
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
}
