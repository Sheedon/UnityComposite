using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Sheedon.Hex.Tests.Map
{
    /**
     * 验证泛型数据层的区域约束、读写和动态成员行为。
     */
    public sealed class HexLayerTests
    {
        [Test]
        public void SetGetTryGetAndContains_OperateOnAssignedValues()
        {
            var first = new HexCoord(0, 0);
            var second = new HexCoord(1, -1);
            var region = new HexRegion(new[] { first, second });
            var layer = new HexLayer<string>(region);

            layer.Set(first, "plain");

            Assert.That(layer.Get(first), Is.EqualTo("plain"));
            Assert.That(layer.Contains(first), Is.True);
            Assert.That(layer.TryGet(first, out var value), Is.True);
            Assert.That(value, Is.EqualTo("plain"));
            Assert.That(layer.Contains(second), Is.False);
            Assert.That(layer.TryGet(second, out _), Is.False);

            layer.Set(first, "mountain");
            Assert.That(layer.Get(first), Is.EqualTo("mountain"));
        }

        [Test]
        public void Remove_ClearsAssignedValueWithoutChangingRegion()
        {
            var coord = HexCoord.Zero;
            var region = new HexRegion(new[] { coord });
            var layer = new HexLayer<int>(region);
            layer.Set(coord, 7);

            Assert.That(layer.Remove(coord), Is.True);
            Assert.That(region.Contains(coord), Is.True);
            Assert.That(layer.Contains(coord), Is.False);
            Assert.That(layer.TryGet(coord, out _), Is.False);
            Assert.Throws<KeyNotFoundException>(() => layer.Get(coord));
            Assert.That(layer.Remove(coord), Is.False);
        }

        [Test]
        public void Remove_AfterRegionRemoval_ClearsStaleStoredValue()
        {
            var coord = new HexCoord(2, -1);
            var region = new HexRegion(new[] { coord });
            var layer = new HexLayer<int>(region);
            layer.Set(coord, 42);

            region.Remove(coord);

            Assert.That(layer.Remove(coord), Is.True);
            Assert.That(layer.Remove(coord), Is.False);

            region.Add(coord);
            Assert.That(layer.Contains(coord), Is.False);
            Assert.That(layer.TryGet(coord, out _), Is.False);
        }

        [Test]
        public void InvalidOrUnassignedCoordinates_HaveDistinctBehavior()
        {
            var inside = HexCoord.Zero;
            var outside = new HexCoord(8, -3);
            var layer = new HexLayer<int>(new HexRegion(new[] { inside }));

            Assert.Throws<KeyNotFoundException>(() => layer.Get(inside));
            Assert.Throws<ArgumentOutOfRangeException>(() => layer.Get(outside));
            Assert.Throws<ArgumentOutOfRangeException>(() => layer.Set(outside, 1));
            Assert.That(layer.TryGet(outside, out _), Is.False);
            Assert.That(layer.Contains(outside), Is.False);
        }

        [Test]
        public void Enumeration_ReturnsOnlyAssignedCurrentMembers()
        {
            var first = new HexCoord(0, 0);
            var second = new HexCoord(1, 0);
            var region = new HexRegion(new[] { first, second });
            var layer = new HexLayer<int>(region);
            layer.Set(first, 10);
            layer.Set(second, 20);

            region.Remove(second);
            var entries = layer.ToArray();

            Assert.That(entries, Has.Length.EqualTo(1));
            Assert.That(entries[0].Key, Is.EqualTo(first));
            Assert.That(entries[0].Value, Is.EqualTo(10));
        }

        [Test]
        public void RemoveAndReAddCoordinate_DoesNotRestoreOldValue()
        {
            var coord = new HexCoord(-2, 4);
            var region = new HexRegion(new[] { coord });
            var layer = new HexLayer<int>(region);
            layer.Set(coord, 99);

            region.Remove(coord);
            region.Add(coord);

            Assert.That(layer.TryGet(coord, out _), Is.False);
            Assert.That(layer.Contains(coord), Is.False);
            Assert.Throws<KeyNotFoundException>(() => layer.Get(coord));
        }

        [Test]
        public void Constructor_NullRegion_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new HexLayer<int>(null));
        }
    }
}
