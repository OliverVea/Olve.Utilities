using Olve.Utilities.Graphs;
using Olve.Utilities.Ids;

namespace Olve.Utilities.Tests.Graphs;

public class DirectedGraphTests
{
    [Test]
    public async Task GetNode_OnExistingNode_SucceedsWithNodeValue()
    {
        // Arrange
        var (graph, nodeIds) = CreateGraphWithNodes(1);
        await Assert.That(nodeIds.Length).IsEqualTo(1);
        var nodeId = nodeIds[0];

        // Act
        var nodeResult = graph.GetNode(nodeId);

        // Assert
        await Assert.That(nodeResult.Succeeded).IsTrue();
        var node = nodeResult.Value;
        await Assert.That(node.Id).IsEqualTo(nodeId);
    }

    [Test]
    public async Task GetNode_OnNonExistentNode_Fails()
    {
        // Arrange
        var graph = new DirectedGraph();
        var fakeNodeId = Id<Node>.New();

        // Act
        var result = graph.GetNode(fakeNodeId);

        // Assert
        await Assert.That(result.Succeeded).IsFalse();
    }

    [Test]
    public async Task CreateEdge_CreatesEdgeSuccessfully()
    {
        // Arrange
        var (graph, nodeIds) = CreateGraphWithNodes(2);
        var from = nodeIds[0];
        var to = nodeIds[1];

        // Act
        var edgeResult = graph.CreateEdge(from, to);

        // Assert
        await Assert.That(edgeResult.Succeeded).IsTrue();
        var edgeId = edgeResult.Value;
        var edgeGetResult = graph.GetEdge(edgeId);
        await Assert.That(edgeGetResult.Succeeded).IsTrue();

        var edge = edgeGetResult.Value;
        await Assert.That(edge.From).IsEqualTo(from);
        await Assert.That(edge.To).IsEqualTo(to);
    }

    [Test]
    public async Task CreateEdge_SameFromTo_CreatesDistinctEdges()
    {
        // Arrange
        var (graph, nodeIds) = CreateGraphWithNodes(2);
        var from = nodeIds[0];
        var to = nodeIds[1];

        // Act
        var edgeResult1 = graph.CreateEdge(from, to);
        var edgeResult2 = graph.CreateEdge(from, to);

        // Assert
        await Assert.That(edgeResult1.Succeeded).IsTrue();
        await Assert.That(edgeResult2.Succeeded).IsTrue();
        await Assert.That(edgeResult1.Value).IsNotEqualTo(edgeResult2.Value);
    }

    [Test]
    public async Task CreateEdge_OppositeDirection_Succeeds()
    {
        // Arrange
        var (graph, nodeIds) = CreateGraphWithNodes(2);
        var a = nodeIds[0];
        var b = nodeIds[1];

        // Act
        var edgeAB = graph.CreateEdge(a, b);
        var edgeBA = graph.CreateEdge(b, a);

        // Assert
        await Assert.That(edgeAB.Succeeded).IsTrue();
        await Assert.That(edgeBA.Succeeded).IsTrue();
        await Assert.That(edgeAB.Value).IsNotEqualTo(edgeBA.Value);
    }

    [Test]
    public async Task CreateEdges_FromOneToMany_Succeeds()
    {
        // Arrange
        var (graph, nodeIds) = CreateGraphWithNodes(5);
        var from = nodeIds[0];
        var others = nodeIds[1..];

        // Act & Assert
        foreach (var to in others)
        {
            var result = graph.CreateEdge(from, to);
            await Assert.That(result.Succeeded).IsTrue();
        }

        var outgoing = graph.GetOutgoingEdges(from);
        await Assert.That(outgoing.Succeeded).IsTrue();
        await Assert.That(outgoing.Value!.Count).IsEqualTo(others.Length);
    }

    [Test]
    public async Task GetEdge_OnExistingEdge_Succeeds()
    {
        // Arrange
        var (graph, nodeIds) = CreateGraphWithNodes(2);
        var from = nodeIds[0];
        var to = nodeIds[1];
        var edgeResult = graph.CreateEdge(from, to);
        var edgeId = edgeResult.Value;

        // Act
        var result = graph.GetEdge(edgeId);

        // Assert
        await Assert.That(result.Succeeded).IsTrue();
        var edge = result.Value;
        await Assert.That(edge.From).IsEqualTo(from);
        await Assert.That(edge.To).IsEqualTo(to);
    }

    [Test]
    public async Task GetEdge_OnNonExistentEdge_Fails()
    {
        // Arrange
        var graph = new DirectedGraph();
        var fakeEdgeId = Id<DirectedEdge>.New();

        // Act
        var result = graph.GetEdge(fakeEdgeId);

        // Assert
        await Assert.That(result.Succeeded).IsFalse();
    }

    [Test]
    public async Task DeleteEdge_OnExistingEdge_Succeeds()
    {
        // Arrange
        var (graph, nodeIds) = CreateGraphWithNodes(2);
        var from = nodeIds[0];
        var to = nodeIds[1];
        var edgeResult = graph.CreateEdge(from, to);
        var edgeId = edgeResult.Value;

        // Act
        var deletionResult = graph.DeleteEdge(edgeId);

        // Assert
        await Assert.That(deletionResult.Succeeded).IsTrue();
        // Try to get the deleted edge, should fail.
        var getEdgeResult = graph.GetEdge(edgeId);
        await Assert.That(getEdgeResult.Succeeded).IsFalse();
    }

    [Test]
    public async Task DeleteEdge_OnNonExistentEdge_Fails()
    {
        // Arrange
        var graph = new DirectedGraph();
        var fakeEdgeId = Id<DirectedEdge>.New();

        // Act
        var deletionResult = graph.DeleteEdge(fakeEdgeId);

        // Assert
        await Assert.That(deletionResult.Succeeded).IsFalse();
    }

    [Test]
    public async Task DeleteNode_WithConnectedEdges_DeletesEdges()
    {
        // Arrange
        var (graph, nodeIds) = CreateGraphWithNodes(3);
        var nodeA = nodeIds[0];
        var nodeB = nodeIds[1];
        var nodeC = nodeIds[2];

        // Create edges: A->B and B->C
        var edgeResult1 = graph.CreateEdge(nodeA, nodeB);
        var edgeResult2 = graph.CreateEdge(nodeB, nodeC);
        var edgeId1 = edgeResult1.Value;
        var edgeId2 = edgeResult2.Value;

        // Pre-assert: edges exist
        await Assert.That(graph.GetEdge(edgeId1).Succeeded).IsTrue();
        await Assert.That(graph.GetEdge(edgeId2).Succeeded).IsTrue();

        // Act: Delete node B, which should remove both its incoming and outgoing edges.
        var deletionResult = graph.DeleteNode(nodeB);

        // Assert
        await Assert.That(deletionResult.Succeeded).IsTrue();
        await Assert.That(graph.GetNode(nodeB).Succeeded).IsFalse();
        await Assert.That(graph.GetEdge(edgeId1).Succeeded).IsFalse();
        await Assert.That(graph.GetEdge(edgeId2).Succeeded).IsFalse();
    }

    [Test]
    public async Task FollowEdge_OnValidEdge_ReturnsTargetNodeId()
    {
        // Arrange
        var (graph, nodeIds) = CreateGraphWithNodes(2);
        var from = nodeIds[0];
        var to = nodeIds[1];
        var edgeResult = graph.CreateEdge(from, to);
        var edgeId = edgeResult.Value;

        // Act
        var followResult = graph.FollowEdge(from, edgeId);

        // Assert
        await Assert.That(followResult.Succeeded).IsTrue();
        await Assert.That(followResult.Value).IsEqualTo(to);
    }

    [Test]
    public async Task FollowEdge_OnInvalidSource_Fails()
    {
        // Arrange
        var (graph, nodeIds) = CreateGraphWithNodes(2);
        var from = nodeIds[0];
        var to = nodeIds[1];
        var edgeResult = graph.CreateEdge(from, to);
        var edgeId = edgeResult.Value;

        // Act
        var followResult = graph.FollowEdge(to, edgeId);

        // Assert
        await Assert.That(followResult.Succeeded).IsFalse();
    }

    [Test]
    public async Task GetIncomingEdges_ReturnsCorrectEdges()
    {
        // Arrange
        var (graph, nodeIds) = CreateGraphWithNodes(3);
        var nodeA = nodeIds[0];
        var nodeB = nodeIds[1];
        var nodeC = nodeIds[2];

        // Create edges: A->C and B->C
        var edgeResult1 = graph.CreateEdge(nodeA, nodeC);
        var edgeResult2 = graph.CreateEdge(nodeB, nodeC);
        var edgeId1 = edgeResult1.Value;
        var edgeId2 = edgeResult2.Value;

        // Act
        var incomingEdgesResult = graph.GetIncomingEdges(nodeC);

        // Assert
        await Assert.That(incomingEdgesResult.Succeeded).IsTrue();
        var incomingEdges = incomingEdgesResult.Value;
        await Assert.That(incomingEdges!.Contains(edgeId1)).IsTrue();
        await Assert.That(incomingEdges.Contains(edgeId2)).IsTrue();
    }

    [Test]
    public async Task GetOutgoingEdges_ReturnsCorrectEdges()
    {
        // Arrange
        var (graph, nodeIds) = CreateGraphWithNodes(3);
        var nodeA = nodeIds[0];
        var nodeB = nodeIds[1];
        var nodeC = nodeIds[2];

        // Create edges: A->B and A->C
        var edgeResult1 = graph.CreateEdge(nodeA, nodeB);
        var edgeResult2 = graph.CreateEdge(nodeA, nodeC);
        var edgeId1 = edgeResult1.Value;
        var edgeId2 = edgeResult2.Value;

        // Act
        var outgoingEdgesResult = graph.GetOutgoingEdges(nodeA);

        // Assert
        await Assert.That(outgoingEdgesResult.Succeeded).IsTrue();
        var outgoingEdges = outgoingEdgesResult.Value;
        await Assert.That(outgoingEdges!.Contains(edgeId1)).IsTrue();
        await Assert.That(outgoingEdges.Contains(edgeId2)).IsTrue();
    }

    [Test]
    public async Task CreateFullMesh_AllNodesConnectedToEachOther()
    {
        // Arrange
        var (graph, nodeIds) = CreateGraphWithNodes(4);
        var expectedEdgeCount = nodeIds.Length * (nodeIds.Length - 1);

        var createdEdges = new HashSet<Id<DirectedEdge>>();

        // Act: create directed edge between every pair of distinct nodes
        foreach (var from in nodeIds)
        {
            foreach (var to in nodeIds)
            {
                if (from.Equals(to))
                    continue;
                var result = graph.CreateEdge(from, to);
                await Assert.That(result.Succeeded).IsTrue();
                createdEdges.Add(result.Value);
            }
        }

        // Assert: all expected edges are created
        await Assert.That(createdEdges.Count).IsEqualTo(expectedEdgeCount);
    }

    [Test]
    public async Task IncomingOutgoing_DoNotOverlap()
    {
        // Arrange
        var (graph, nodeIds) = CreateGraphWithNodes(3);
        var a = nodeIds[0];
        var b = nodeIds[1];
        var c = nodeIds[2];

        graph.CreateEdge(a, b);
        graph.CreateEdge(b, c);
        graph.CreateEdge(c, a);

        // Act
        var incomingA = graph.GetIncomingEdges(a);
        var outgoingA = graph.GetOutgoingEdges(a);

        // Assert
        await Assert.That(incomingA.Value!.All(e => !outgoingA.Value!.Contains(e))).IsTrue();
    }

    /// <summary>
    /// Helper method to create a graph with a given number of nodes.
    /// </summary>
    private (DirectedGraph, Id<Node>[]) CreateGraphWithNodes(int nodeCount)
    {
        var graph = new DirectedGraph();
        var nodeIds = new Id<Node>[nodeCount];

        for (var i = 0; i < nodeCount; i++)
        {
            if (graph.CreateNode().TryPickProblems(out var problems, out var nodeId))
            {
                Assert.Fail(string.Join(", ", problems.Select(x => x.ToDebugString())));
            }
            nodeIds[i] = nodeId;
        }

        return (graph, nodeIds);
    }
}
