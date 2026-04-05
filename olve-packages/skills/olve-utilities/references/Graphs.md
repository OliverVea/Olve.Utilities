# Graphs

API docs: [https://olivervea.github.io/Olve.Utilities/api/Olve.Utilities.Graphs.html](https://olivervea.github.io/Olve.Utilities/api/Olve.Utilities.Graphs.html)

Source: `src/Olve.Utilities/Graphs/`

## Node

```csharp
public readonly record struct Node(Id<Node> Id);
```

## DirectedEdge

```csharp
public readonly record struct DirectedEdge(Id<DirectedEdge> Id, Id<Node> From, Id<Node> To);
```

## DirectedGraph

ID-based directed graph with node and edge management.

```csharp
public class DirectedGraph
{
    // Properties
    public IReadOnlyCollection<Id<Node>> NodeIds { get; }
    public IReadOnlyCollection<Node> Nodes { get; }
    public IReadOnlyCollection<Id<DirectedEdge>> EdgeIds { get; }
    public IReadOnlyCollection<DirectedEdge> Edges { get; }

    // Node operations
    public Id<Node> CreateNode();
    public bool TryGetNode(Id<Node> nodeId, out Node node);
    public bool DeleteNode(Id<Node> nodeId);

    // Edge operations
    public Id<DirectedEdge> CreateEdge(Id<Node> from, Id<Node> to);
    public bool TryGetEdge(Id<DirectedEdge> edgeId, out DirectedEdge edge);
    public bool DeleteEdge(Id<DirectedEdge> edgeId);

    // Traversal
    public bool TryFollowEdge(Id<Node> from, Id<DirectedEdge> edgeId, out Id<Node> to);
    public bool TryGetIncomingEdges(Id<Node> nodeId, out IReadOnlySet<Id<DirectedEdge>>? edges);
    public bool TryGetOutgoingEdges(Id<Node> nodeId, out IReadOnlySet<Id<DirectedEdge>>? edges);
}
```

- `CreateNode` returns a new randomly-generated `Id<Node>`.
- `CreateEdge` returns a new `Id<DirectedEdge>` connecting two nodes.
- `DeleteNode` also removes all connected edges.
- `TryFollowEdge` verifies the edge starts at the given `from` node.
