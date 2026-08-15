using System;
using NUnit.Framework;

namespace Sheedon.Hex.Tests.Algorithms
{
    /**
     * 验证 CostRange 的最小 Cost、预算边界及无效输入行为。
     */
    public sealed class CostRangeTests
    {
        /**
         * 确认单位 Cost 下的范围与 Hex Distance 半径一致，并保存最小到达代价。
         */
        [Test]
        public void Find_UniformCost_ReturnsExpectedRangeAndCosts()
        {
            var graph = new RegionHexGraph(HexShapes.CreateHexagon(HexCoord.Zero, 2));

            var result = CostRange.Find(graph, new AlgorithmTestTraversalRule(), HexCoord.Zero, 1);

            Assert.That(result.IsStartValid, Is.True);
            Assert.That(result.Count, Is.EqualTo(7));
            Assert.That(result.GetMinimumCost(HexCoord.Zero), Is.Zero);
            Assert.That(result.GetMinimumCost(new HexCoord(1, 0)), Is.EqualTo(1));
            Assert.That(result.Contains(new HexCoord(2, 0)), Is.False);
        }

        /**
         * 验证加权边会消耗预算，超出最大 Cost 的节点不会进入结果。
         */
        [Test]
        public void Find_WeightedEdges_RespectsMaximumCost()
        {
            var graph = new RegionHexGraph(new HexRegion(new[]
            {
                HexCoord.Zero,
                new HexCoord(1, 0),
                new HexCoord(2, 0)
            }));
            var rule = new AlgorithmTestTraversalRule(getCost: (from, to) => 2);

            var result = CostRange.Find(graph, rule, HexCoord.Zero, 3);

            Assert.That(result.Contains(new HexCoord(1, 0)), Is.True);
            Assert.That(result.GetMinimumCost(new HexCoord(1, 0)), Is.EqualTo(2));
            Assert.That(result.Contains(new HexCoord(2, 0)), Is.False);
        }

        /**
         * 确认无效起点返回可识别的空结果，负预算和非法边 Cost 会抛出异常。
         */
        [Test]
        public void Find_InvalidInputs_AreReported()
        {
            var graph = new RegionHexGraph(new HexRegion(new[] { HexCoord.Zero, new HexCoord(1, 0) }));
            var rule = new AlgorithmTestTraversalRule();

            var invalidStart = CostRange.Find(graph, rule, new HexCoord(9, 9), 2);

            Assert.That(invalidStart.IsStartValid, Is.False);
            Assert.That(invalidStart.Count, Is.Zero);
            Assert.Throws<ArgumentOutOfRangeException>(() => CostRange.Find(graph, rule, HexCoord.Zero, -1));
            Assert.Throws<InvalidOperationException>(() =>
                CostRange.Find(
                    graph,
                    new AlgorithmTestTraversalRule(getCost: (from, to) => 0),
                    HexCoord.Zero,
                    2));
        }
    }
}
