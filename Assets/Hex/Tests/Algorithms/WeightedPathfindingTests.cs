using System;
using System.Linq;
using NUnit.Framework;

namespace Sheedon.Hex.Tests.Algorithms
{
    /**
     * 验证 Dijkstra 与 A* 的加权最短路径、启发函数和 Cost 契约。
     */
    public sealed class WeightedPathfindingTests
    {
        /**
         * 确认 Dijkstra 会避开步数更少但 Cost 更高的路线。
         */
        [Test]
        public void Dijkstra_DifferentCosts_ChoosesLowerTotalCost()
        {
            var start = HexCoord.Zero;
            var expensive = new HexCoord(1, 0);
            var goal = new HexCoord(2, 0);
            var detour = new[] { new HexCoord(1, -1), new HexCoord(2, -1) };
            var graph = new RegionHexGraph(new HexRegion(new[]
            {
                start,
                expensive,
                goal,
                detour[0],
                detour[1]
            }));
            var rule = new AlgorithmTestTraversalRule(
                getCost: (from, to) => to == expensive ? 10 : 1);

            var result = Dijkstra.FindPath(graph, rule, start, goal);

            Assert.That(result.Status, Is.EqualTo(HexPathStatus.Success));
            Assert.That(result.Path, Is.EqualTo(new[] { start, detour[0], detour[1], goal }));
            Assert.That(result.TotalCost, Is.EqualTo(3));
        }

        /**
         * 验证相同图与规则下，默认 A* 和 Dijkstra 得到一致的最低 Cost。
         */
        [Test]
        public void AStarAndDijkstra_ReturnSameMinimumCost()
        {
            var region = HexShapes.CreateHexagon(HexCoord.Zero, 3);
            var graph = new RegionHexGraph(region);
            var blocked = new HexCoord(0, 0);
            var rule = new AlgorithmTestTraversalRule(
                (from, to) => to != blocked,
                (from, to) => to.R == 1 ? 3 : 1);
            var start = new HexCoord(-3, 0);
            var goal = new HexCoord(3, 0);

            var dijkstra = Dijkstra.FindPath(graph, rule, start, goal);
            var aStar = AStar.FindPath(graph, rule, start, goal);

            Assert.That(dijkstra.Status, Is.EqualTo(HexPathStatus.Success));
            Assert.That(aStar.Status, Is.EqualTo(HexPathStatus.Success));
            Assert.That(aStar.TotalCost, Is.EqualTo(dijkstra.TotalCost));
            Assert.That(aStar.Path.First(), Is.EqualTo(start));
            Assert.That(aStar.Path.Last(), Is.EqualTo(goal));
        }

        /**
         * 确认调用方可以提供非负且可采纳的自定义启发函数。
         */
        [Test]
        public void AStar_CustomHeuristic_IsUsed()
        {
            var graph = new RegionHexGraph(HexShapes.CreateHexagon(HexCoord.Zero, 2));
            var estimateCalls = 0;

            var result = AStar.FindPath(
                graph,
                new AlgorithmTestTraversalRule(),
                HexCoord.Zero,
                new HexCoord(2, 0),
                (from, goal) =>
                {
                    estimateCalls++;
                    return HexTopology.Distance(from, goal);
                });

            Assert.That(result.Status, Is.EqualTo(HexPathStatus.Success));
            Assert.That(result.TotalCost, Is.EqualTo(2));
            Assert.That(estimateCalls, Is.GreaterThan(0));
        }

        /**
         * 验证非正边 Cost 与负启发值会在算法边界立即被拒绝。
         */
        [Test]
        public void InvalidCostOrHeuristic_Throws()
        {
            var graph = new RegionHexGraph(new HexRegion(new[] { HexCoord.Zero, new HexCoord(1, 0) }));
            var invalidCostRule = new AlgorithmTestTraversalRule(getCost: (from, to) => 0);

            Assert.Throws<InvalidOperationException>(() =>
                Dijkstra.FindPath(graph, invalidCostRule, HexCoord.Zero, new HexCoord(1, 0)));
            Assert.Throws<InvalidOperationException>(() =>
                AStar.FindPath(
                    graph,
                    new AlgorithmTestTraversalRule(),
                    HexCoord.Zero,
                    new HexCoord(1, 0),
                    (from, goal) => -1));
        }

        /**
         * 确认累积 Cost 超出 Int32 时明确抛出溢出异常。
         */
        [Test]
        public void Dijkstra_TotalCostOverflow_Throws()
        {
            var graph = new RegionHexGraph(new HexRegion(new[]
            {
                HexCoord.Zero,
                new HexCoord(1, 0),
                new HexCoord(2, 0)
            }));
            var rule = new AlgorithmTestTraversalRule(
                getCost: (from, to) => from == HexCoord.Zero ? int.MaxValue : 1);

            Assert.Throws<OverflowException>(() =>
                Dijkstra.FindPath(graph, rule, HexCoord.Zero, new HexCoord(2, 0)));
        }
    }
}
