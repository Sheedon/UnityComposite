using System;
using NUnit.Framework;

namespace Sheedon.Hex.Tests.Core
{
/**
 * 验证六边形布局在坐标转换和角点计算中的几何行为。
 */
    public sealed class HexLayoutTests
    {
/**
 * 确认使用尖顶布局公式时，坐标转换会考虑原点位置和尺寸参数。
 */
        [Test]
        public void HexToPoint_UsesPointyTopFormulaAndOrigin()
        {
            var layout = new HexLayout(2d, new HexPoint(10d, -5d));

            var point = layout.HexToPoint(new HexCoord(1, 0));

            Assert.That(point.X, Is.EqualTo(10d + (2d * Math.Sqrt(3d))).Within(1e-10));
            Assert.That(point.Y, Is.EqualTo(-5d).Within(1e-10));
        }

/**
 * 验证六边形坐标与二维点之间的往返转换在负坐标场景下保持一致。
 */
        [Test]
        public void HexAndPointConversions_RoundTripNegativeCoordinates()
        {
            var layout = new HexLayout(3.5d, new HexPoint(-8d, 11d));

            for (var q = -8; q <= 8; q++)
            {
                for (var r = -8; r <= 8; r++)
                {
                    var coord = new HexCoord(q, r);
                    Assert.That(layout.PointToHex(layout.HexToPoint(coord)), Is.EqualTo(coord));
                }
            }
        }

/**
 * 确认每个角点到中心的距离都等于六边形半径。
 */
        [Test]
        public void GetCorner_ReturnsPointOneRadiusFromCenter()
        {
            var layout = new HexLayout(4d);
            var center = layout.HexToPoint(HexCoord.Zero);

            for (var cornerIndex = 0; cornerIndex < 6; cornerIndex++)
            {
                var corner = layout.GetCorner(HexCoord.Zero, cornerIndex);
                var distance = Math.Sqrt(
                    ((corner.X - center.X) * (corner.X - center.X)) +
                    ((corner.Y - center.Y) * (corner.Y - center.Y)));

                Assert.That(distance, Is.EqualTo(4d).Within(1e-10));
            }
        }

/**
 * 确认尖顶布局的首个角点相对水平方向旋转三十度，不会退化为平顶布局。
 */
        [Test]
        public void GetCorner_UsesPointyTopAngleOffset()
        {
            var layout = new HexLayout(2d);

            var corner = layout.GetCorner(HexCoord.Zero, 0);

            Assert.That(corner.X, Is.EqualTo(Math.Sqrt(3d)).Within(1e-10));
            Assert.That(corner.Y, Is.EqualTo(1d).Within(1e-10));
        }

/**
 * 验证六个方向的边中心都指向对应邻居，避免表现层维护第二套方向角点表。
 */
        [Test]
        public void GetEdgeCorners_MatchesPointyTopNeighborDirections()
        {
            var layout = new HexLayout(2d, new HexPoint(3d, -4d));
            var coord = new HexCoord(-2, 3);
            var center = layout.HexToPoint(coord);

            foreach (HexDirection direction in Enum.GetValues(typeof(HexDirection)))
            {
                HexPoint start;
                HexPoint end;
                layout.GetEdgeCorners(coord, direction, out start, out end);
                var neighborCenter = layout.HexToPoint(
                    HexTopology.GetNeighbor(coord, direction));
                var edgeMidpoint = new HexPoint(
                    (start.X + end.X) * 0.5d,
                    (start.Y + end.Y) * 0.5d);

                var edgeDirectionX = edgeMidpoint.X - center.X;
                var edgeDirectionY = edgeMidpoint.Y - center.Y;
                var neighborDirectionX = neighborCenter.X - center.X;
                var neighborDirectionY = neighborCenter.Y - center.Y;
                var cross = (edgeDirectionX * neighborDirectionY) -
                            (edgeDirectionY * neighborDirectionX);
                var dot = (edgeDirectionX * neighborDirectionX) +
                          (edgeDirectionY * neighborDirectionY);

                Assert.That(cross, Is.EqualTo(0d).Within(1e-10), direction.ToString());
                Assert.That(dot, Is.GreaterThan(0d), direction.ToString());
            }
        }

/**
 * 验证相邻六边形从相反方向取得完全相同的公共边。
 */
        [Test]
        public void GetEdgeCorners_AdjacentHexesShareExactlyTheSameEdge()
        {
            var layout = new HexLayout(1.75d);
            var coord = new HexCoord(-1, 0);

            foreach (HexDirection direction in Enum.GetValues(typeof(HexDirection)))
            {
                HexPoint start;
                HexPoint end;
                layout.GetEdgeCorners(coord, direction, out start, out end);

                HexPoint neighborStart;
                HexPoint neighborEnd;
                layout.GetEdgeCorners(
                    HexTopology.GetNeighbor(coord, direction),
                    direction.Opposite(),
                    out neighborStart,
                    out neighborEnd);

                Assert.That(start.X, Is.EqualTo(neighborEnd.X).Within(1e-10));
                Assert.That(start.Y, Is.EqualTo(neighborEnd.Y).Within(1e-10));
                Assert.That(end.X, Is.EqualTo(neighborStart.X).Within(1e-10));
                Assert.That(end.Y, Is.EqualTo(neighborStart.Y).Within(1e-10));
            }
        }

/**
 * 验证非法布局参数会抛出异常，避免生成无意义的网格配置。
 */
        [Test]
        public void InvalidConfiguration_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new HexLayout(0d));
            Assert.Throws<ArgumentOutOfRangeException>(() => new HexLayout(double.NaN));
            Assert.Throws<ArgumentOutOfRangeException>(() => new HexLayout(1d, new HexPoint(double.PositiveInfinity, 0d)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new HexLayout(1d).GetCorner(HexCoord.Zero, 6));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                HexPoint start;
                HexPoint end;
                new HexLayout(1d).GetEdgeCorners(
                    HexCoord.Zero,
                    (HexDirection)6,
                    out start,
                    out end);
            });
        }
    }
}
