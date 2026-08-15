using System.Linq;
using NUnit.Framework;

namespace Sheedon.Hex.Tests.Algorithms
{
    /**
     * 验证 BFS 的最少步数路径、障碍绕行与失败状态。
     */
    public sealed class BreadthFirstSearchTests
    {
        /**
         * 确认直线图会返回包含起终点的最少步数路径。
         */
        [Test]
        public void FindPath_StraightLine_ReturnsShortestStepPath()
        {
            var coordinates = Enumerable.Range(0, 4).Select(q => new HexCoord(q, 0)).ToArray();
            var graph = new RegionHexGraph(new HexRegion(coordinates));

            var result = BreadthFirstSearch.FindPath(
                graph,
                new AlgorithmTestTraversalRule(),
                coordinates[0],
                coordinates[3]);

            Assert.That(result.Status, Is.EqualTo(HexPathStatus.Success));
            Assert.That(result.Path, Is.EqualTo(coordinates));
            Assert.That(result.TotalCost, Is.EqualTo(3));
        }

        /**
         * 验证直接路径被阻挡时，BFS 会选择更长但仍可通行的绕行路线。
         */
        [Test]
        public void FindPath_BlockedDirectNode_Detours()
        {
            var graph = new RegionHexGraph(HexShapes.CreateHexagon(HexCoord.Zero, 2));
            var blocked = new HexCoord(1, 0);
            var rule = new AlgorithmTestTraversalRule((from, to) => to != blocked);

            var result = BreadthFirstSearch.FindPath(graph, rule, HexCoord.Zero, new HexCoord(2, 0));

            Assert.That(result.Status, Is.EqualTo(HexPathStatus.Success));
            Assert.That(result.TotalCost, Is.EqualTo(3));
            Assert.That(result.Path.Contains(blocked), Is.False);
            Assert.That(result.Path.Zip(result.Path.Skip(1), HexTopology.Distance).All(distance => distance == 1), Is.True);
        }

        /**
         * 确认完全阻断时返回 NoPath，且失败结果不携带伪路径或 Cost。
         */
        [Test]
        public void FindPath_FullyBlocked_ReturnsNoPath()
        {
            var graph = new RegionHexGraph(new HexRegion(new[]
            {
                HexCoord.Zero,
                new HexCoord(1, 0),
                new HexCoord(2, 0)
            }));
            var rule = new AlgorithmTestTraversalRule((from, to) => to != new HexCoord(1, 0));

            var result = BreadthFirstSearch.FindPath(graph, rule, HexCoord.Zero, new HexCoord(2, 0));

            Assert.That(result.Status, Is.EqualTo(HexPathStatus.NoPath));
            Assert.That(result.Path, Is.Empty);
            Assert.That(result.TotalCost, Is.Zero);
        }

        /**
         * 验证相同起终点，以及起点或目标不存在时的精确状态。
         */
        [Test]
        public void FindPath_ReportsTrivialAndInvalidEndpoints()
        {
            var start = HexCoord.Zero;
            var goal = new HexCoord(1, 0);
            var graph = new RegionHexGraph(new HexRegion(new[] { start, goal }));
            var rule = new AlgorithmTestTraversalRule();

            var same = BreadthFirstSearch.FindPath(graph, rule, start, start);
            var invalidStart = BreadthFirstSearch.FindPath(graph, rule, new HexCoord(-1, 0), goal);
            var invalidGoal = BreadthFirstSearch.FindPath(graph, rule, start, new HexCoord(2, 0));

            Assert.That(same.Status, Is.EqualTo(HexPathStatus.Success));
            Assert.That(same.Path, Is.EqualTo(new[] { start }));
            Assert.That(same.TotalCost, Is.Zero);
            Assert.That(invalidStart.Status, Is.EqualTo(HexPathStatus.InvalidStart));
            Assert.That(invalidGoal.Status, Is.EqualTo(HexPathStatus.InvalidGoal));
        }
    }
}
