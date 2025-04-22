using System.Runtime.InteropServices;
using Olve.Utilities.Ids;

namespace Olve.Utilities.Graphs;

/// <summary>
///     Represents a directed edge in a graph.
/// </summary>
/// <param name="Id">The unique identifier for the edge.</param>
/// <param name="From"> The starting node of the edge.</param>
/// <param name="To">The ending node of the edge.</param>
[StructLayout(LayoutKind.Sequential)]
public readonly record struct DirectedEdge(Id<DirectedEdge> Id, Id<Node> From, Id<Node> To);
