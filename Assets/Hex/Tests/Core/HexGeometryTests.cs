using System;
using System.Linq;
using NUnit.Framework;

namespace Sheedon.Hex.Tests.Core
{
/**
 * 验证几何型工具在轴向坐标四舍五入和线性路径生成上的逻辑。
 */
    public sealed class HexGeometryTests
    {
/**
 * 确认浮点坐标会被正确舍入到最近的合法六边形格子。
 */
        [TestCase(0.0, 0.0, 0, 0)]
        [TestCase(0.9, -0.2, 1, 0)]
        [TestCase(-1.2, 2.1, -1, 2)]
        public void Round_ReturnsNearestValidAxialCoordinate(
            double q,
            double r,
            int expectedQ,
            int expectedR)
        {
            Assert.That(HexGeometry.Round(q, r), Is.EqualTo(new HexCoord(expectedQ, expectedR)));
        }

/**
 * 确认直线路径会从起点到终点依次覆盖相邻格子，且长度符合距离规则。
 */
        [Test]
        public void GetLine_IncludesEndpointsAndAdjacentSteps()
        {
            var start = new HexCoord(-2, 1);
            var goal = new HexCoord(3, -2);
            var line = HexGeometry.GetLine(start, goal).ToArray();

            Assert.That(line, Has.Length.EqualTo(HexTopology.Distance(start, goal) + 1));
            Assert.That(line.First(), Is.EqualTo(start));
            Assert.That(line.Last(), Is.EqualTo(goal));

            for (var i = 1; i < line.Length; i++)
            {
                Assert.That(HexTopology.Distance(line[i - 1], line[i]), Is.EqualTo(1));
            }
        }

/**
 * 验证从某个坐标到自身的线段应只返回该坐标本身。
 */
        [Test]
        public void GetLine_FromCoordinateToItself_ReturnsOneCell()
        {
            var coord = new HexCoord(-3, -4);

            Assert.That(HexGeometry.GetLine(coord, coord), Is.EqualTo(new[] { coord }));
        }

/**
 * 验证非有限数值会触发异常，避免将无效数据带入邻接计算。
 */
        [Test]
        public void Round_NonFiniteCoordinate_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => HexGeometry.Round(double.NaN, 0d));
            Assert.Throws<ArgumentOutOfRangeException>(() => HexGeometry.Round(0d, double.PositiveInfinity));
        }
    }
}
