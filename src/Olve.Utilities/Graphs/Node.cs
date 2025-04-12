using Olve.Utilities.Ids;

namespace Olve.Utilities.Graphs;

/// <summary>
/// Represents a node in a graph.
/// </summary>
/// <param name="Id">The unique identifier for the node.</param>
public readonly record struct Node(Id<Node> Id);