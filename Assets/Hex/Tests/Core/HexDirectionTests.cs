using System;
using NUnit.Framework;

namespace Sheedon.Hex.Tests.Core
{
/**
 * 验证六边形方向枚举以及方向扩展方法的稳定性和正确性。
 */
    public sealed class HexDirectionTests
    {
/**
 * 确认枚举值与固定的六方向顺序完全一致。
 */
        [TestCase(HexDirection.NE, 0)]
        [TestCase(HexDirection.E, 1)]
        [TestCase(HexDirection.SE, 2)]
        [TestCase(HexDirection.SW, 3)]
        [TestCase(HexDirection.W, 4)]
        [TestCase(HexDirection.NW, 5)]
        public void DirectionValues_AreStable(HexDirection direction, int expectedValue)
        {
            Assert.That((int)direction, Is.EqualTo(expectedValue));
        }

/**
 * 验证相反方向必须是对称的，并且互逆关系成立。
 */
        [TestCase(HexDirection.NE, HexDirection.SW)]
        [TestCase(HexDirection.E, HexDirection.W)]
        [TestCase(HexDirection.SE, HexDirection.NW)]
        public void Opposite_ReturnsDirectionAcrossCenter(HexDirection direction, HexDirection expected)
        {
            Assert.That(direction.Opposite(), Is.EqualTo(expected));
            Assert.That(expected.Opposite(), Is.EqualTo(direction));
        }

/**
 * 验证前后方向方法会在边界处正确循环回绕。
 */
        [Test]
        public void NextAndPrevious_WrapAround()
        {
            Assert.That(HexDirection.NW.Next(), Is.EqualTo(HexDirection.NE));
            Assert.That(HexDirection.NE.Previous(), Is.EqualTo(HexDirection.NW));
        }

/**
 * 验证非法方向值会抛出参数异常，避免无效状态进入算法。
 */
        [Test]
        public void InvalidDirection_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => ((HexDirection)6).Opposite());
        }
    }
}
