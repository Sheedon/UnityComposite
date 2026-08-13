using System;
using System.Linq;
using NUnit.Framework;

namespace Sheedon.Hex.Tests.Core
{
/**
 * 验证六边形拓扑中的相邻、距离、范围、环和螺旋遍历行为。
 */
    public sealed class HexTopologyTests
    {
/**
 * 确认邻居遍历顺序稳定，并按六个方向返回正确的相邻坐标。
 */
        [Test]
        public void GetNeighbors_ReturnsSixDirectionsInStableOrder()
        {
            var neighbors = HexTopology.GetNeighbors(new HexCoord(2, -3)).ToArray();

            Assert.That(neighbors, Is.EqualTo(new[]
            {
                new HexCoord(3, -4),
                new HexCoord(3, -3),
                new HexCoord(2, -2),
                new HexCoord(1, -2),
                new HexCoord(1, -3),
                new HexCoord(2, -4)
            }));
        }

/**
 * 验证距离计算在负坐标情况下仍然对称且保持正确值。
 */
        [Test]
        public void Distance_HandlesNegativeCoordinatesAndIsSymmetric()
        {
            var first = new HexCoord(-4, 1);
            var second = new HexCoord(2, -3);

            Assert.That(HexTopology.Distance(first, second), Is.EqualTo(6));
            Assert.That(HexTopology.Distance(second, first), Is.EqualTo(6));
        }

/**
 * 确认 GetRange 返回的格子数量与要求的半径范围一致，并且全部在半径内。
 */
        [TestCase(0, 1)]
        [TestCase(1, 7)]
        [TestCase(2, 19)]
        [TestCase(3, 37)]
        public void GetRange_ReturnsExpectedCellCount(int radius, int expectedCount)
        {
            var center = new HexCoord(-7, 4);
            var range = HexTopology.GetRange(center, radius).ToArray();

            Assert.That(range, Has.Length.EqualTo(expectedCount));
            Assert.That(range.Distinct().Count(), Is.EqualTo(expectedCount));
            Assert.That(range.All(coord => HexTopology.Distance(center, coord) <= radius), Is.True);
        }

/**
 * 验证 GetRing 仅返回恰好位于指定半径上的格子，不会混入更近或更远的坐标。
 */
        [TestCase(0, 1)]
        [TestCase(1, 6)]
        [TestCase(3, 18)]
        public void GetRing_ReturnsOnlyCellsAtRadius(int radius, int expectedCount)
        {
            var center = new HexCoord(5, -8);
            var ring = HexTopology.GetRing(center, radius).ToArray();

            Assert.That(ring, Has.Length.EqualTo(expectedCount));
            Assert.That(ring.Distinct().Count(), Is.EqualTo(expectedCount));
            Assert.That(ring.All(coord => HexTopology.Distance(center, coord) == radius), Is.True);
        }

/**
 * 确认螺旋遍历会先返回中心，再按层次输出同心环的格子。
 */
        [Test]
        public void GetSpiral_IsCenterFollowedByConcentricRings()
        {
            var center = new HexCoord(1, 2);
            var spiral = HexTopology.GetSpiral(center, 2).ToArray();

            Assert.That(spiral, Has.Length.EqualTo(19));
            Assert.That(spiral[0], Is.EqualTo(center));
            Assert.That(spiral.Skip(1).Take(6).All(coord => HexTopology.Distance(center, coord) == 1), Is.True);
            Assert.That(spiral.Skip(7).All(coord => HexTopology.Distance(center, coord) == 2), Is.True);
        }

/**
 * 验证负数半径会抛出异常，保证半径参数在拓扑计算前被合法化。
 */
        [Test]
        public void NegativeRadius_ThrowsWhenEnumerated()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => HexTopology.GetRange(HexCoord.Zero, -1).ToArray());
            Assert.Throws<ArgumentOutOfRangeException>(() => HexTopology.GetRing(HexCoord.Zero, -1).ToArray());
            Assert.Throws<ArgumentOutOfRangeException>(() => HexTopology.GetSpiral(HexCoord.Zero, -1).ToArray());
        }
    }
}
