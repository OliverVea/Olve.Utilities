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
}
