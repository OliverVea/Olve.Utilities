using Olve.Utilities.CollectionExtensions;
using Olve.Utilities.Collections;
using Olve.Utilities.Graphs;
using Olve.Utilities.Ids;
using Olve.Utilities.Paginations;
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

        await Assert.That(aliceValues!).HasCount().EqualTo(2);
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

        await Assert.That(aliceCourses!).HasCount().EqualTo(2);
        await Assert.That(mathStudents!).HasCount().EqualTo(2);
    }

    [Test]
    public async Task FixedSizeQueueExample()
    {
        var queue = new FixedSizeQueue<string>(maxSize: 3);

        queue.Enqueue("a");
        queue.Enqueue("b");
        queue.Enqueue("c");
        queue.Enqueue("d"); // "a" is dropped, queue is now { "b", "c", "d" }

        await Assert.That(queue.Count).IsEqualTo(3);
        await Assert.That(queue.TryDequeue(out var first) && first == "b").IsTrue();
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
        var pagination = new Pagination(Page: 0, PageSize: 2);

        var result = new PaginatedResult<string>(
            items: items[..2],
            pagination: pagination,
            totalCount: items.Length);

        // result.HasNextPage == true
        // result.TotalPages == 2

        await Assert.That(result.HasNextPage).IsTrue();
        await Assert.That(result.TotalPages).IsEqualTo(2);
        await Assert.That(result.Count).IsEqualTo(2);
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

        await Assert.That(edges!).HasCount().EqualTo(2);
        await Assert.That(graph.Nodes).HasCount().EqualTo(3);
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
        await Assert.That(cache["scores"]).HasCount().EqualTo(2);
    }
}
