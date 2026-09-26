using Olve.Utilities.CollectionExtensions;
using Olve.Utilities.Collections;
using Olve.Utilities.Graphs;
using Olve.Utilities.Ids;
using Olve.Utilities.Lookup;
using Olve.Utilities.Paginations;
using Olve.Utilities.Stores;
using Olve.Utilities.StringFormatting;

namespace Olve.Utilities.Tests;

public class ReadmeDemo
{
    record User;
    record Order;

    [Test]
    public async Task TypedIds()
    {
        // Create a random typed ID
        var userId = Id.New<User>();

        // Deterministic ID from a name (UUIDv5)
        var aliceId = Id.FromName<User>("alice");
        var aliceId2 = Id.FromName<User>("alice"); // same as aliceId

        // Parse from string
        Id.TryParse<User>(userId.Value.ToString(), out var parsed); // parsed == userId

        await Assert.That(aliceId).IsEqualTo(aliceId2);
        await Assert.That(parsed).IsEqualTo(userId);
    }

    [Test]
    public async Task BidirectionalDictionaryExample()
    {
        var dict = new BidirectionalDictionary<string, int>();

        dict.Set("alice", 1);
        dict.Set("bob", 2);

        // Look up in both directions
        dict.TryGet("alice", out var id);    // 1
        dict.TryGet(2, out var name);        // "bob"

        await Assert.That(id).IsEqualTo(1);
        await Assert.That(name).IsEqualTo("bob");
    }

    [Test]
    public async Task OneToManyLookupExample()
    {
        var lookup = new OneToManyLookup<string, int>();

        // A parent maps to many children
        lookup.Set("alice", 1, true);
        lookup.Set("alice", 2, true);
        lookup.Set("bob", 3, true);

        // Get all values for a key
        lookup.TryGet("alice", out var aliceValues); // { 1, 2 }

        // Reverse lookup: which key owns this value?
        lookup.TryGet(1, out var owner); // "alice"

        await Assert.That(aliceValues!).Count().IsEqualTo(2);
        await Assert.That(owner).IsEqualTo("alice");
    }

    [Test]
    public async Task ManyToManyLookupExample()
    {
        var enrollment = new ManyToManyLookup<string, int>();

        enrollment.Set("alice", 101, true);
        enrollment.Set("alice", 102, true);
        enrollment.Set("bob", 101, true);

        // Get all course IDs for a student
        enrollment.TryGet("alice", out var aliceCourses); // { 101, 102 }

        // Get all students in a course
        enrollment.TryGet(101, out var mathStudents); // { "alice", "bob" }

        await Assert.That(aliceCourses!).Count().IsEqualTo(2);
        await Assert.That(mathStudents!).Count().IsEqualTo(2);
    }

    [Test]
    public async Task FixedSizeQueueExample()
    {
        var queue = new FixedSizeQueue<string>(maxSize: 3);

        queue.Enqueue("a");
        queue.Enqueue("b");
        queue.Enqueue("c");
        queue.Enqueue("d"); // "a" is dropped, queue is now { "b", "c", "d" }

        queue.TryDequeue(out var first); // "b"

        // configure back-pressure behavior
        var strict = new FixedSizeQueue<string>(maxSize: 2, FullQueueBehavior.Throw);
        strict.Enqueue("x");
        strict.Enqueue("y");
        // strict.Enqueue("z"); // throws InvalidOperationException

        var dropping = new FixedSizeQueue<string>(maxSize: 2, FullQueueBehavior.DropNewest);
        dropping.Enqueue("x");
        dropping.Enqueue("y");
        var accepted = dropping.Enqueue("z"); // false — "z" is rejected

        // TryEnqueue fails when full, regardless of policy
        var tried = strict.TryEnqueue("z"); // false — queue is at capacity

        await Assert.That(queue.Count).IsEqualTo(2);
        await Assert.That(first).IsEqualTo("b");
        await Assert.That(accepted).IsFalse();
        await Assert.That(tried).IsFalse();
    }

    [Test]
    public async Task FormatTimeAgoExample()
    {
        var now = new DateTimeOffset(2025, 6, 15, 12, 0, 0, TimeSpan.Zero);
        var then = new DateTimeOffset(2025, 6, 13, 12, 0, 0, TimeSpan.Zero);

        var text = DateTimeFormatter.FormatTimeAgo(now, then); // "2 days ago"

        await Assert.That(text).IsEqualTo("2 days ago");
    }

    [Test]
    public async Task PaginationExample()
    {
        var items = new[] { "alice", "bob", "charlie" };

        var page = new Page<string>(
            Items: items[..2],
            PageNumber: 0,
            PageSize: 2,
            TotalCount: items.Length);

        // page.HasNextPage == true
        // page.TotalPages == 2

        await Assert.That(page.HasNextPage).IsTrue();
        await Assert.That(page.TotalPages).IsEqualTo(2);
        await Assert.That(page.Items.Count).IsEqualTo(2);
    }

    [Test]
    public async Task DirectedGraphExample()
    {
        var graph = new DirectedGraph();

        // Create nodes
        var nodeA = graph.CreateNode();
        var nodeB = graph.CreateNode();
        var nodeC = graph.CreateNode();

        // Create edges: A -> B, A -> C
        graph.CreateEdge(nodeA, nodeB);
        graph.CreateEdge(nodeA, nodeC);

        // Query outgoing edges
        graph.TryGetOutgoingEdges(nodeA, out var edges); // 2 edges

        await Assert.That(edges!).Count().IsEqualTo(2);
        await Assert.That(graph.Nodes).Count().IsEqualTo(3);
    }

    [Test]
    public async Task DictionaryExtensionsExample()
    {
        var cache = new Dictionary<string, List<int>>();

        // GetOrAdd: get existing value or create it
        var list = cache.GetOrAdd("scores", () => []);
        list.Add(100);

        var same = cache.GetOrAdd("scores", () => []); // same reference as list

        // TryUpdate: update only if the key exists
        var updated = cache.TryUpdate("scores", old => [..old, 200]); // true
        var missed = cache.TryUpdate("missing", _ => []); // false

        await Assert.That(same).IsSameReferenceAs(list);
        await Assert.That(updated).IsTrue();
        await Assert.That(missed).IsFalse();
        await Assert.That(cache["scores"]).Count().IsEqualTo(2);
    }

    [Test]
    public async Task OffsetPaginationExample()
    {
        var users = new[] { "alice", "bob", "charlie", "dave", "eve" };

        // e.g. bound from a request body like { "offset": 1, "limit": 2 }
        var request = new OffsetPagination(Offset: 1, Limit: 2).Clamp(defaultLimit: 20, maxLimit: 100);

        var slice = users.Paginate(request);
        // slice.Items == ["bob", "charlie"], slice.TotalCount == 5
        // slice.HasMore == true, slice.Next == OffsetPagination { Offset = 3, Limit = 2 }

        // Page-based pagination always converts to offset/limit...
        var fromPage = new Pagination(Page: 2, PageSize: 2).ToOffsetPagination(); // Offset = 4, Limit = 2

        // ...but offset/limit only converts back when the offset is a multiple of the limit
        request.TryToPagination(out _); // false
        fromPage.TryToPagination(out var pagination); // true, pagination == Pagination { Page = 2, PageSize = 2 }

        await Assert.That(slice.Items).IsEquivalentTo(["bob", "charlie"]);
        await Assert.That(slice.TotalCount).IsEqualTo(5);
        await Assert.That(slice.Next).IsEqualTo(new OffsetPagination(3, 2));
        await Assert.That(fromPage).IsEqualTo(new OffsetPagination(4, 2));
        await Assert.That(request.TryToPagination(out _)).IsFalse();
        await Assert.That(pagination).IsEqualTo(new Pagination(2, 2));
    }

    private record Train(Id<Train> Id, string Line, int Order) : IHasId<Id<Train>>;

    [Test]
    public async Task EntityStoreExample()
    {
        // record Train(Id<Train> Id, string Line, int Order) : IHasId<Id<Train>>;
        EntityStore<Train> trains = [];

        // Keep indexes and views as long as you read them, e.g. in a field next to the store
        var byLine = trains.CreateIndex(t => t.Line);
        var byOrder = trains.CreateOrderedView(Comparer<Train>.Create((a, b) => a.Order.CompareTo(b.Order)));

        // Events carry the committed values
        trains.OnDeleted.Subscribe(e => Console.WriteLine($"{e.Entity.Line} train removed"));

        var express = new Train(Id.New<Train>(), "red", 2);
        trains.Set(express);
        trains.Set(new Train(Id.New<Train>(), "red", 1));

        // Mutate is an atomic read-modify-write; indexes follow key changes
        var mutated = trains.Mutate(express.Id, t => t with { Line = "blue", Order = 3 });

        var redTrains = byLine.GetForKey("red"); // 1 id
        var first = byOrder[0]; // the Order = 1 train

        await Assert.That(mutated.Succeeded).IsTrue();
        await Assert.That(redTrains.Count).IsEqualTo(1);
        await Assert.That(first.Order).IsEqualTo(1);
    }
}
