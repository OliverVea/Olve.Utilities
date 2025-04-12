using Olve.Results;
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
    /// <returns>
    /// A <see cref="Result{T}"/> containing the ID of the newly created node if successful,
    /// or a problem description if the update function fails.
    /// </returns>
    public Result<Id<Node>> CreateNode()
    {
        var nodeId = Id<Node>.New();
        Node node = new(nodeId);
        
        _nodes.Add(nodeId, node);
        
        return nodeId;
    }

    /// <summary>
    /// Gets the node with the specified ID.
    /// </summary>
    /// <param name="nodeId">The ID of the node to retrieve.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing the node if found; otherwise, a problem result.
    /// </returns>
    public Result<Node> GetNode(Id<Node> nodeId)
    {
        if (!_nodes.TryGetValue(nodeId, out var node))
        {
            return new ResultProblem("Could not find node with id '{0}'", nodeId);
        }

        return node;
    }

    /// <summary>
    /// Deletes the node with the specified ID and all its connected edges.
    /// </summary>
    /// <param name="nodeId">The ID of the node to delete.</param>
    /// <returns>
    /// A <see cref="DeletionResult"/> indicating whether the node was successfully deleted or not found.
    /// </returns>
    public DeletionResult DeleteNode(Id<Node> nodeId)
    {
        if (!_nodes.ContainsKey(nodeId))
        {
            return DeletionResult.NotFound();
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

        return DeletionResult.Success();
    }

    /// <summary>
    /// Creates a new directed edge between two nodes.
    /// </summary>
    /// <param name="from">The ID of the source node.</param>
    /// <param name="to">The ID of the target node.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing the ID of the created edge if successful,
    /// or a problem result if the operation fails.
    /// </returns>
    public Result<Id<DirectedEdge>> CreateEdge(Id<Node> from, Id<Node> to)
    {
        var edgeId = Id<DirectedEdge>.New();
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
    /// Gets the directed edge with the specified ID.
    /// </summary>
    /// <param name="edgeId">The ID of the edge to retrieve.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing the edge if found; otherwise, a problem result.
    /// </returns>
    public Result<DirectedEdge> GetEdge(Id<DirectedEdge> edgeId)
    {
        if (!_edges.TryGetValue(edgeId, out var edge))
        {
            return new ResultProblem("Could not find edge with id '{0}'.", edgeId);
        }

        return edge;
    }

    /// <summary>
    /// Deletes the directed edge with the specified ID.
    /// </summary>
    /// <param name="edgeId">The ID of the edge to delete.</param>
    /// <returns>
    /// A <see cref="DeletionResult"/> indicating whether the edge was successfully deleted or not found.
    /// </returns>
    public DeletionResult DeleteEdge(Id<DirectedEdge> edgeId)
    {
        if (!_edges.TryGetValue(edgeId, out var edge))
        {
            return DeletionResult.NotFound();
        }

        RemoveFromOutgoingEdges(edge.From, edgeId);
        RemoveFromIncomingEdges(edge.To, edgeId);
        
        _edges.Remove(edgeId);
        
        return DeletionResult.Success();
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
    /// Follows a directed edge from the given source node.
    /// </summary>
    /// <param name="from">The ID of the source node.</param>
    /// <param name="edgeId">The ID of the edge to follow.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing the ID of the target node if the edge exists and starts at the specified source node;
    /// otherwise, a problem result.
    /// </returns>
    public Result<Id<Node>> FollowEdge(Id<Node> from, Id<DirectedEdge> edgeId)
    {
        if (!_edges.TryGetValue(edgeId, out var edge))
        {
            return new ResultProblem("Could not find edge with id '{0}'.", edgeId);
        }
        
        if (edge.From != from)
        {
            return new ResultProblem("Edge '{0}' does not start at node '{1}'.", edgeId, from);
        }
        
        return edge.To;
    }
    
    /// <summary>
    /// Gets the set of incoming edge IDs for the specified node.
    /// </summary>
    /// <param name="nodeId">The ID of the node.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing the set of incoming edge IDs,
    /// or an empty set if none exist.
    /// </returns>
    public Result<IReadOnlySet<Id<DirectedEdge>>> GetIncomingEdges(Id<Node> nodeId)
    {
        if (!_incomingEdgesForNode.TryGetValue(nodeId, out var edges))
        {
            return Result.Success<IReadOnlySet<Id<DirectedEdge>>>(new HashSet<Id<DirectedEdge>>());
        }

        return Result.Success((IReadOnlySet<Id<DirectedEdge>>)edges);
    }
    
    /// <summary>
    /// Gets the set of outgoing edge IDs for the specified node.
    /// </summary>
    /// <param name="nodeId">The ID of the node.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing the set of outgoing edge IDs,
    /// or an empty set if none exist.
    /// </returns>
    public Result<IReadOnlySet<Id<DirectedEdge>>> GetOutgoingEdges(Id<Node> nodeId)
    {
        if (!_outgoingEdgesForNode.TryGetValue(nodeId, out var edges))
        {
            return Result.Success<IReadOnlySet<Id<DirectedEdge>>>(new HashSet<Id<DirectedEdge>>());
        }

        return Result.Success((IReadOnlySet<Id<DirectedEdge>>)edges);
    }
}