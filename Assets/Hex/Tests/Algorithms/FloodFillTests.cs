using NUnit.Framework;

namespace Sheedon.Hex.Tests.Algorithms
{
    /**
     * 验证 FloodFill 对连通分量、通行规则和无效起点的处理。
     */
    public sealed class FloodFillTests
    {
        /**
         * 确认搜索只返回与起点相连的节点，不会跨越图中的空间断层。
         */
        [Test]
        public void FindConnected_DisconnectedGraph_ReturnsStartComponent()
        {
            var graph = new RegionHexGraph(new HexRegion(new[]
            {
                HexCoord.Zero,
                new HexCoord(1, 0),
                new HexCoord(5, 0),
                new HexCoord(6, 0)
            }));

            var connected = FloodFill.FindConnected(graph, new AlgorithmTestTraversalRule(), HexCoord.Zero);

            Assert.That(connected, Is.EquivalentTo(new[] { HexCoord.Zero, new HexCoord(1, 0) }));
        }

        /**
         * 验证通行规则可以切断原本相邻的节点。
         */
        [Test]
        public void FindConnected_BlockedNode_StopsExpansion()
        {
            var blocked = new HexCoord(1, 0);
            var graph = new RegionHexGraph(new HexRegion(new[]
            {
                HexCoord.Zero,
                blocked,
                new HexCoord(2, 0)
            }));
            var rule = new AlgorithmTestTraversalRule((from, to) => to != blocked);

            var connected = FloodFill.FindConnected(graph, rule, HexCoord.Zero);

            Assert.That(connected, Is.EqualTo(new[] { HexCoord.Zero }));
        }

        /**
         * 确认无效起点返回空集合。
         */
        [Test]
        public void FindConnected_InvalidStart_ReturnsEmpty()
        {
            var graph = new RegionHexGraph(new HexRegion(new[] { HexCoord.Zero }));

            var connected = FloodFill.FindConnected(
                graph,
                new AlgorithmTestTraversalRule(),
                new HexCoord(4, 4));

            Assert.That(connected, Is.Empty);
        }
    }
}
