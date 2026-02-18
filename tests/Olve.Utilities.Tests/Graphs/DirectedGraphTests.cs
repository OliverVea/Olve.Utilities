    using Olve.Utilities.Graphs;
    using Olve.Utilities.Ids;
    using TUnit.Assertions;
    using TUnit.Assertions.Extensions;
    using TUnit.Core;

    namespace Olve.Utilities.Tests.Graphs;

    public class DirectedGraphTests
    {
        [Test]
        public async Task TryGetNode_OnExistingNode_ReturnsTrue()
        {
            // Arrange
            var (graph, nodeIds) = CreateGraphWithNodes(1);
            await Assert.That(nodeIds.Length).IsEqualTo(1);
            var nodeId = nodeIds[0];

            // Act
            var found = graph.TryGetNode(nodeId, out var node);

            // Assert
            await Assert.That(found).IsTrue();
            await Assert.That(node.Id).IsEqualTo(nodeId);
        }

        [Test]
        public async Task TryGetNode_OnNonExistentNode_ReturnsFalse()
        {
            // Arrange
            var graph = new DirectedGraph();
            var fakeNodeId = Id.New<Node>();

            // Act
            var found = graph.TryGetNode(fakeNodeId, out _);

            // Assert
            await Assert.That(found).IsFalse();
        }

        [Test]
        public async Task CreateEdge_CreatesEdgeSuccessfully()
        {
            // Arrange
            var (graph, nodeIds) = CreateGraphWithNodes(2);
            var from = nodeIds[0];
            var to = nodeIds[1];

            // Act
            var edgeId = graph.CreateEdge(from, to);

            // Assert
            var found = graph.TryGetEdge(edgeId, out var edge);
            await Assert.That(found).IsTrue();
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
            var edgeId1 = graph.CreateEdge(from, to);
            var edgeId2 = graph.CreateEdge(from, to);

            // Assert
            await Assert.That(edgeId1).IsNotEqualTo(edgeId2);
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
            await Assert.That(edgeAB).IsNotEqualTo(edgeBA);
        }

        [Test]
        public async Task CreateEdges_FromOneToMany_Succeeds()
        {
            // Arrange
            var (graph, nodeIds) = CreateGraphWithNodes(5);
            var from = nodeIds[0];
            var others = nodeIds[1..];

            // Act
            foreach (var to in others)
            {
                graph.CreateEdge(from, to);
            }

            // Assert
            var found = graph.TryGetOutgoingEdges(from, out var outgoing);
            await Assert.That(found).IsTrue();
            await Assert.That(outgoing!.Count).IsEqualTo(others.Length);
        }

        [Test]
        public async Task TryGetEdge_OnExistingEdge_ReturnsTrue()
        {
            // Arrange
            var (graph, nodeIds) = CreateGraphWithNodes(2);
            var from = nodeIds[0];
            var to = nodeIds[1];
            var edgeId = graph.CreateEdge(from, to);

            // Act
            var found = graph.TryGetEdge(edgeId, out var edge);

            // Assert
            await Assert.That(found).IsTrue();
            await Assert.That(edge.From).IsEqualTo(from);
            await Assert.That(edge.To).IsEqualTo(to);
        }

        [Test]
        public async Task TryGetEdge_OnNonExistentEdge_ReturnsFalse()
        {
            // Arrange
            var graph = new DirectedGraph();
            var fakeEdgeId = Id.New<DirectedEdge>();

            // Act
            var found = graph.TryGetEdge(fakeEdgeId, out _);

            // Assert
            await Assert.That(found).IsFalse();
        }

        [Test]
        public async Task DeleteEdge_OnExistingEdge_Succeeds()
        {
            // Arrange
            var (graph, nodeIds) = CreateGraphWithNodes(2);
            var from = nodeIds[0];
            var to = nodeIds[1];
            var edgeId = graph.CreateEdge(from, to);

            // Act
            var deleted = graph.DeleteEdge(edgeId);

            // Assert
            await Assert.That(deleted).IsTrue();
            // Try to get the deleted edge, should fail.
            var found = graph.TryGetEdge(edgeId, out _);
            await Assert.That(found).IsFalse();
        }

        [Test]
        public async Task DeleteEdge_OnNonExistentEdge_Fails()
        {
            // Arrange
            var graph = new DirectedGraph();
            var fakeEdgeId = Id.New<DirectedEdge>();

            // Act
            var deleted = graph.DeleteEdge(fakeEdgeId);

            // Assert
            await Assert.That(deleted).IsFalse();
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
            var edgeId1 = graph.CreateEdge(nodeA, nodeB);
            var edgeId2 = graph.CreateEdge(nodeB, nodeC);

            // Pre-assert: edges exist
            await Assert.That(graph.TryGetEdge(edgeId1, out _)).IsTrue();
            await Assert.That(graph.TryGetEdge(edgeId2, out _)).IsTrue();

            // Act: Delete node B, which should remove both its incoming and outgoing edges.
            var deleted = graph.DeleteNode(nodeB);

            // Assert
            await Assert.That(deleted).IsTrue();
            await Assert.That(graph.TryGetNode(nodeB, out _)).IsFalse();
            await Assert.That(graph.TryGetEdge(edgeId1, out _)).IsFalse();
            await Assert.That(graph.TryGetEdge(edgeId2, out _)).IsFalse();
        }

        [Test]
        public async Task TryFollowEdge_OnValidEdge_ReturnsTargetNodeId()
        {
            // Arrange
            var (graph, nodeIds) = CreateGraphWithNodes(2);
            var from = nodeIds[0];
            var to = nodeIds[1];
            var edgeId = graph.CreateEdge(from, to);

            // Act
            var found = graph.TryFollowEdge(from, edgeId, out var target);

            // Assert
            await Assert.That(found).IsTrue();
            await Assert.That(target).IsEqualTo(to);
        }

        [Test]
        public async Task TryFollowEdge_OnInvalidSource_ReturnsFalse()
        {
            // Arrange
            var (graph, nodeIds) = CreateGraphWithNodes(2);
            var from = nodeIds[0];
            var to = nodeIds[1];
            var edgeId = graph.CreateEdge(from, to);

            // Act
            var found = graph.TryFollowEdge(to, edgeId, out _);

            // Assert
            await Assert.That(found).IsFalse();
        }

        [Test]
        public async Task TryGetIncomingEdges_ReturnsCorrectEdges()
        {
            // Arrange
            var (graph, nodeIds) = CreateGraphWithNodes(3);
            var nodeA = nodeIds[0];
            var nodeB = nodeIds[1];
            var nodeC = nodeIds[2];

            // Create edges: A->C and B->C
            var edgeId1 = graph.CreateEdge(nodeA, nodeC);
            var edgeId2 = graph.CreateEdge(nodeB, nodeC);

            // Act
            var found = graph.TryGetIncomingEdges(nodeC, out var incomingEdges);

            // Assert
            await Assert.That(found).IsTrue();
            await Assert.That(incomingEdges!.Contains(edgeId1)).IsTrue();
            await Assert.That(incomingEdges.Contains(edgeId2)).IsTrue();
        }

        [Test]
        public async Task TryGetOutgoingEdges_ReturnsCorrectEdges()
        {
            // Arrange
            var (graph, nodeIds) = CreateGraphWithNodes(3);
            var nodeA = nodeIds[0];
            var nodeB = nodeIds[1];
            var nodeC = nodeIds[2];

            // Create edges: A->B and A->C
            var edgeId1 = graph.CreateEdge(nodeA, nodeB);
            var edgeId2 = graph.CreateEdge(nodeA, nodeC);

            // Act
            var found = graph.TryGetOutgoingEdges(nodeA, out var outgoingEdges);

            // Assert
            await Assert.That(found).IsTrue();
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
                    if (from.Equals(to)) continue;
                    var edgeId = graph.CreateEdge(from, to);
                    createdEdges.Add(edgeId);
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
            graph.TryGetIncomingEdges(a, out var incomingA);
            graph.TryGetOutgoingEdges(a, out var outgoingA);

            // Assert
            await Assert.That(incomingA!.All(e => !outgoingA!.Contains(e))).IsTrue();
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
                nodeIds[i] = graph.CreateNode();
            }

            return (graph, nodeIds);
        }
    }
