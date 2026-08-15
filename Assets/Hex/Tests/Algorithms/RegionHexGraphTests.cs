using System;
using System.Linq;
using NUnit.Framework;

namespace Sheedon.Hex.Tests.Algorithms
{
    /**
     * 验证有限区域到图契约的实时适配与邻居过滤行为。
     */
    public sealed class RegionHexGraphTests
    {
        /**
         * 确认图只返回当前区域内的理论邻居，并保持稳定方向顺序。
         */
        [Test]
        public void GetNeighbors_FiltersRegionAndPreservesDirectionOrder()
        {
            var center = HexCoord.Zero;
            var east = HexTopology.GetNeighbor(center, HexDirection.E);
            var northwest = HexTopology.GetNeighbor(center, HexDirection.NW);
            var graph = new RegionHexGraph(new HexRegion(new[] { center, east, northwest }));

            var neighbors = graph.GetNeighbors(center).ToArray();

            Assert.That(neighbors, Is.EqualTo(new[] { east, northwest }));
            Assert.That(graph.GetNeighbors(new HexCoord(9, 9)), Is.Empty);
        }

        /**
         * 确认适配器会实时反映绑定区域的增加和移除操作。
         */
        [Test]
        public void Graph_ReflectsLiveRegionMembership()
        {
            var region = new HexRegion(new[] { HexCoord.Zero });
            var graph = new RegionHexGraph(region);
            var east = HexTopology.GetNeighbor(HexCoord.Zero, HexDirection.E);

            region.Add(east);
            Assert.That(graph.Contains(east), Is.True);
            Assert.That(graph.GetNeighbors(HexCoord.Zero), Is.EqualTo(new[] { east }));

            region.Remove(east);
            Assert.That(graph.Contains(east), Is.False);
            Assert.That(graph.GetNeighbors(HexCoord.Zero), Is.Empty);
        }

        /**
         * 验证空区域依赖会在构造边界立即失败。
         */
        [Test]
        public void Constructor_NullRegion_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new RegionHexGraph(null));
        }
    }
}
