using System;
using System.Linq;
using NUnit.Framework;

namespace Sheedon.Hex.Tests.Map
{
    /**
     * 验证有限区域的成员管理、去重和遍历行为。
     */
    public sealed class HexRegionTests
    {
        [Test]
        public void AddRemoveAndContains_TrackMembership()
        {
            var region = new HexRegion();
            var coord = new HexCoord(-3, 5);

            Assert.That(region.Add(coord), Is.True);
            Assert.That(region.Add(coord), Is.False);
            Assert.That(region.Contains(coord), Is.True);
            Assert.That(region.Count, Is.EqualTo(1));
            Assert.That(region.Remove(coord), Is.True);
            Assert.That(region.Remove(coord), Is.False);
            Assert.That(region.Contains(coord), Is.False);
            Assert.That(region.Count, Is.Zero);
        }

        [Test]
        public void Constructor_DeduplicatesAndEnumeratesCoordinates()
        {
            var coordinates = new[]
            {
                new HexCoord(0, 0),
                new HexCoord(2, -1),
                new HexCoord(0, 0),
                new HexCoord(-4, 3)
            };

            var region = new HexRegion(coordinates);

            Assert.That(region.Count, Is.EqualTo(3));
            Assert.That(region, Is.EquivalentTo(coordinates.Distinct()));
        }

        [Test]
        public void Constructor_NullCoordinates_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new HexRegion(null));
        }
    }
}
