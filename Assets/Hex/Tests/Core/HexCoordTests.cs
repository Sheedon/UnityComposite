using NUnit.Framework;

namespace Sheedon.Hex.Tests.Core
{
/**
 * 验证轴向坐标结构体的构造、比较和算术运算行为。
 */
    public sealed class HexCoordTests
    {
/**
 * 确认构造函数会正确保存 q/r，并计算出对应的 s 分量。
 */
        [Test]
        public void Constructor_ComputesCubeS()
        {
            var coord = new HexCoord(4, -7);

            Assert.That(coord.Q, Is.EqualTo(4));
            Assert.That(coord.R, Is.EqualTo(-7));
            Assert.That(coord.S, Is.EqualTo(3));
        }

/**
 * 验证相等比较和哈希值依赖 q/r，同时保持运行时语义一致。
 */
        [Test]
        public void EqualityAndHashCode_UseQAndR()
        {
            var first = new HexCoord(-3, 5);
            var same = new HexCoord(-3, 5);
            var different = new HexCoord(-3, 4);

            Assert.That(first, Is.EqualTo(same));
            Assert.That(first.GetHashCode(), Is.EqualTo(same.GetHashCode()));
            Assert.That(first == same, Is.True);
            Assert.That(first != different, Is.True);
        }

/**
 * 验证加减运算符和一元取反运算符在轴向坐标上的正确组合逻辑。
 */
        [Test]
        public void Operators_CombineAxialCoordinates()
        {
            var left = new HexCoord(3, -2);
            var right = new HexCoord(-5, 4);

            Assert.That(left + right, Is.EqualTo(new HexCoord(-2, 2)));
            Assert.That(left - right, Is.EqualTo(new HexCoord(8, -6)));
            Assert.That(-left, Is.EqualTo(new HexCoord(-3, 2)));
        }
    }
}
