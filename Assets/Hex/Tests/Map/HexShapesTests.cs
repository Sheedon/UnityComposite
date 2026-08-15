using System;
using System.Linq;
using NUnit.Framework;

namespace Sheedon.Hex.Tests.Map
{
    /**
     * 验证各类 Shape 会构造正确的有限区域，并保持彼此独立的几何语义。
     */
    public sealed class HexShapesTests
    {
        [TestCase(0, 1)]
        [TestCase(1, 7)]
        [TestCase(2, 19)]
        public void CreateHexagon_UsesHexDistance(int radius, int expectedCount)
        {
            var center = new HexCoord(-2, 3);
            var region = HexShapes.CreateHexagon(center, radius);

            Assert.That(region.Count, Is.EqualTo(expectedCount));
            Assert.That(region.All(coord => HexTopology.Distance(center, coord) <= radius), Is.True);
        }

        [Test]
        public void CreateRectangle_UsesOddRColumnsFromNegativeOrigin()
        {
            var region = HexShapes.CreateRectangle(new HexCoord(-1, -1), 2, 2);

            Assert.That(region, Is.EquivalentTo(new[]
            {
                new HexCoord(-1, -1),
                new HexCoord(0, -1),
                new HexCoord(-2, 0),
                new HexCoord(-1, 0)
            }));
        }

        [Test]
        public void CreateSquare_IsEqualSizedRectangle()
        {
            var origin = new HexCoord(2, -3);
            var square = HexShapes.CreateSquare(origin, 3);
            var rectangle = HexShapes.CreateRectangle(origin, 3, 3);

            Assert.That(square.Count, Is.EqualTo(9));
            Assert.That(square, Is.EquivalentTo(rectangle));
        }

        [Test]
        public void CreateParallelogram_UsesInclusiveAxialBounds()
        {
            var region = HexShapes.CreateParallelogram(-1, 1, 2, 3);

            Assert.That(region.Count, Is.EqualTo(6));
            Assert.That(region.All(coord =>
                coord.Q >= -1 && coord.Q <= 1 && coord.R >= 2 && coord.R <= 3), Is.True);
        }

        [Test]
        public void CreateCircle_UsesGeometricCenterDistance()
        {
            var center = HexCoord.Zero;
            var layout = new HexLayout(1d);
            var circle = HexShapes.CreateCircle(center, 3d, layout);

            Assert.That(circle.Count, Is.EqualTo(13));
            Assert.That(circle.Contains(new HexCoord(2, -1)), Is.True);
            Assert.That(circle.Contains(new HexCoord(2, 0)), Is.False);
            Assert.That(circle.All(coord =>
            {
                var point = layout.HexToPoint(coord);
                return (point.X * point.X) + (point.Y * point.Y) <= 9d + 1e-10;
            }), Is.True);
        }

        [Test]
        public void CreateCircle_RespectsLayoutScaleAndZeroRadius()
        {
            var center = new HexCoord(4, -2);
            var unitCircle = HexShapes.CreateCircle(center, 3d, new HexLayout(1d));
            var scaledCircle = HexShapes.CreateCircle(center, 6d, new HexLayout(2d, new HexPoint(7d, -9d)));
            var zeroCircle = HexShapes.CreateCircle(center, 0d, new HexLayout(1d));

            Assert.That(scaledCircle, Is.EquivalentTo(unitCircle));
            Assert.That(zeroCircle, Is.EquivalentTo(new[] { center }));
        }

        [Test]
        public void CreateFromCoordinates_CreatesIrregularDeduplicatedRegion()
        {
            var coordinates = new[]
            {
                new HexCoord(0, 0),
                new HexCoord(1, -1),
                new HexCoord(4, 2),
                new HexCoord(1, -1)
            };

            var region = HexShapes.CreateFromCoordinates(coordinates);

            Assert.That(region.Count, Is.EqualTo(3));
            Assert.That(region, Is.EquivalentTo(coordinates.Distinct()));
        }

        [Test]
        public void InvalidShapeArguments_Throw()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => HexShapes.CreateHexagon(HexCoord.Zero, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => HexShapes.CreateRectangle(HexCoord.Zero, 0, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => HexShapes.CreateRectangle(HexCoord.Zero, 1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => HexShapes.CreateSquare(HexCoord.Zero, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => HexShapes.CreateCircle(HexCoord.Zero, -1d, new HexLayout(1d)));
            Assert.Throws<ArgumentNullException>(() => HexShapes.CreateCircle(HexCoord.Zero, 1d, null));
            Assert.Throws<ArgumentException>(() => HexShapes.CreateParallelogram(1, 0, 0, 1));
            Assert.Throws<ArgumentException>(() => HexShapes.CreateParallelogram(0, 1, 1, 0));
            Assert.Throws<ArgumentNullException>(() => HexShapes.CreateFromCoordinates(null));
        }
    }
}
