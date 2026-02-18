using System.Diagnostics.CodeAnalysis;
using Olve.Utilities.CollectionExtensions;
using Olve.Utilities.Ids;

namespace Olve.Utilities.Graphs;

/// <summary>
/// Represents a basic directed graph with support for node and edge management.
/// </summary>
public class DirectedGraph
{
    private readonly Dictionary<Id<Node>, Node> _nodes = new();
    private readonly Dictionary<Id<DirectedEdge>, DirectedEdge> _edges = new();
    
    private readonly Dictionary<Id<Node>, ISet<Id<DirectedEdge>>> _incomingEdgesForNode = new();
    private readonly Dictionary<Id<Node>, ISet<Id<DirectedEdge>>> _outgoingEdgesForNode = new();
    
    /// <summary>
    /// Gets all node IDs in the graph.
    /// </summary>
    public IReadOnlyCollection<Id<Node>> NodeIds => _nodes.Keys;

    /// <summary>
    /// Gets all nodes in the graph.
    /// </summary>
    public IReadOnlyCollection<Node> Nodes => _nodes.Values;

    /// <summary>
    /// Gets all edge IDs in the graph.
    /// </summary>
    public IReadOnlyCollection<Id<DirectedEdge>> EdgeIds => _edges.Keys;

    /// <summary>
    /// Gets all directed edges in the graph.
    /// </summary>      
    public IReadOnlyCollection<DirectedEdge> Edges => _edges.Values;

    /// <summary>
    /// Creates a new node and adds it to the graph.
    /// </summary>
    /// <returns>The ID of the newly created node.</returns>
    public Id<Node> CreateNode()
    {
        var nodeId = Id.New<Node>();
        Node node = new(nodeId);

        _nodes.Add(nodeId, node);

        return nodeId;
    }

    /// <summary>
    /// Tries to get the node with the specified ID.
    /// </summary>
    /// <param name="nodeId">The ID of the node to retrieve.</param>
    /// <param name="node">When this method returns, contains the node if found; otherwise <c>null</c>.</param>
    /// <returns><c>true</c> if the node was found; otherwise <c>false</c>.</returns>
    public bool TryGetNode(Id<Node> nodeId, [MaybeNullWhen(false)] out Node node)
    {
        return _nodes.TryGetValue(nodeId, out node);
    }

    /// <summary>
    /// Deletes the node with the specified ID and all its connected edges.
    /// </summary>
    /// <param name="nodeId">The ID of the node to delete.</param>
    /// <returns><c>true</c> if the node was found and deleted; otherwise <c>false</c>.</returns>
    public bool DeleteNode(Id<Node> nodeId)
    {
        if (!_nodes.ContainsKey(nodeId))
        {
            return false;
        }

        if (_incomingEdgesForNode.TryGetValue(nodeId, out var edgesToDelete))
        {
            foreach (var edgeId in edgesToDelete)
            {
                DeleteEdge(edgeId);
            }
        }

        if (_outgoingEdgesForNode.TryGetValue(nodeId, out edgesToDelete))
        {
            foreach (var edgeId in edgesToDelete)
            {
                DeleteEdge(edgeId);
            }
        }

        _incomingEdgesForNode.Remove(nodeId);
        _outgoingEdgesForNode.Remove(nodeId);

        _nodes.Remove(nodeId);

        return true;
    }

    /// <summary>
    /// Creates a new directed edge between two nodes.
    /// </summary>
    /// <param name="from">The ID of the source node.</param>
    /// <param name="to">The ID of the target node.</param>
    /// <returns>The ID of the created edge.</returns>
    public Id<DirectedEdge> CreateEdge(Id<Node> from, Id<Node> to)
    {
        var edgeId = Id.New<DirectedEdge>();
        DirectedEdge directedEdge = new(edgeId, from, to);

        _edges.Add(edgeId, directedEdge);

        AddOutgoingEdgeForNode(from, edgeId);
        AddIncomingEdgeForNode(to, edgeId);

        return edgeId;
    }

    private bool AddIncomingEdgeForNode(Id<Node> nodeId, Id<DirectedEdge> edgeId)
        => _incomingEdgesForNode.GetOrAdd(nodeId, () => new HashSet<Id<DirectedEdge>>()).Add(edgeId);

    private bool AddOutgoingEdgeForNode(Id<Node> nodeId, Id<DirectedEdge> edgeId)
        => _outgoingEdgesForNode.GetOrAdd(nodeId, () => new HashSet<Id<DirectedEdge>>()).Add(edgeId);

    /// <summary>
    /// Tries to get the directed edge with the specified ID.
    /// </summary>
    /// <param name="edgeId">The ID of the edge to retrieve.</param>
    /// <param name="edge">When this method returns, contains the edge if found; otherwise <c>null</c>.</param>
    /// <returns><c>true</c> if the edge was found; otherwise <c>false</c>.</returns>
    public bool TryGetEdge(Id<DirectedEdge> edgeId, [MaybeNullWhen(false)] out DirectedEdge edge)
    {
        return _edges.TryGetValue(edgeId, out edge);
    }

    /// <summary>
    /// Deletes the directed edge with the specified ID.
    /// </summary>
    /// <param name="edgeId">The ID of the edge to delete.</param>
    /// <returns><c>true</c> if the edge was found and deleted; otherwise <c>false</c>.</returns>
    public bool DeleteEdge(Id<DirectedEdge> edgeId)
    {
        if (!_edges.TryGetValue(edgeId, out var edge))
        {
            return false;
        }

        RemoveFromOutgoingEdges(edge.From, edgeId);
        RemoveFromIncomingEdges(edge.To, edgeId);

        _edges.Remove(edgeId);

        return true;
    }

    private bool RemoveFromIncomingEdges(Id<Node> nodeId, Id<DirectedEdge> edgeId)
    {
        if (!_incomingEdgesForNode.TryGetValue(nodeId, out var edges))
        {
            return false;
        }

        if (!edges.Remove(edgeId))
        {
            return false;
        }

        if (edges.Count == 0)
        {
            _incomingEdgesForNode.Remove(nodeId);
        }

        return true;
    }

    private bool RemoveFromOutgoingEdges(Id<Node> nodeId, Id<DirectedEdge> edgeId)
    {
        if (!_outgoingEdgesForNode.TryGetValue(nodeId, out var edges))
        {
            return false;
        }

        if (!edges.Remove(edgeId))
        {
            return false;
        }

        if (edges.Count == 0)
        {
            _outgoingEdgesForNode.Remove(nodeId);
        }

        return true;
    }

    /// <summary>
    /// Tries to follow a directed edge from the given source node.
    /// </summary>
    /// <param name="from">The ID of the source node.</param>
    /// <param name="edgeId">The ID of the edge to follow.</param>
    /// <param name="to">When this method returns, contains the ID of the target node if the edge was followed successfully.</param>
    /// <returns><c>true</c> if the edge exists and starts at <paramref name="from"/>; otherwise <c>false</c>.</returns>
    public bool TryFollowEdge(Id<Node> from, Id<DirectedEdge> edgeId, out Id<Node> to)
    {
        if (_edges.TryGetValue(edgeId, out var edge) && edge.From == from)
        {
            to = edge.To;
            return true;
        }

        to = default;
        return false;
    }
    
    /// <summary>
    /// Tries to get the set of incoming edge IDs for the specified node.
    /// </summary>
    /// <param name="nodeId">The ID of the node.</param>
    /// <param name="edges">When this method returns, contains the set of incoming edge IDs if found; otherwise <c>null</c>.</param>
    /// <returns><c>true</c> if the node has incoming edges; otherwise <c>false</c>.</returns>
    public bool TryGetIncomingEdges(Id<Node> nodeId, [NotNullWhen(true)] out IReadOnlySet<Id<DirectedEdge>>? edges)
    {
        if (_incomingEdgesForNode.TryGetValue(nodeId, out var set))
        {
            edges = (IReadOnlySet<Id<DirectedEdge>>)set;
            return true;
        }

        edges = null;
        return false;
    }

    /// <summary>
    /// Tries to get the set of outgoing edge IDs for the specified node.
    /// </summary>
    /// <param name="nodeId">The ID of the node.</param>
    /// <param name="edges">When this method returns, contains the set of outgoing edge IDs if found; otherwise <c>null</c>.</param>
    /// <returns><c>true</c> if the node has outgoing edges; otherwise <c>false</c>.</returns>
    public bool TryGetOutgoingEdges(Id<Node> nodeId, [NotNullWhen(true)] out IReadOnlySet<Id<DirectedEdge>>? edges)
    {
        if (_outgoingEdgesForNode.TryGetValue(nodeId, out var set))
        {
            edges = (IReadOnlySet<Id<DirectedEdge>>)set;
            return true;
        }

        edges = null;
        return false;
    }
}