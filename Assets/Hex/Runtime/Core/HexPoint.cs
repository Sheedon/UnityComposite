using System;

namespace Sheedon.Hex
{
    /**
     * 表示与 Unity 无关的二维平面点，用于六边形布局的坐标转换和绘制计算。
     */
    public readonly struct HexPoint : IEquatable<HexPoint>
    {
        /**
         * 原点坐标，表示二维空间中的 (0, 0)。
         */
        public static readonly HexPoint Zero = new HexPoint(0d, 0d);

        /**
         * 使用 x 与 y 初始化二维点。
         * @param x 水平方向坐标。
         * @param y 垂直方向坐标。
         */
        public HexPoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        /**
         * 点的 X 坐标。
         */
        public double X { get; }

        /**
         * 点的 Y 坐标。
         */
        public double Y { get; }

        /**
         * 判断当前点与另一个点是否拥有相同的坐标。
         * @param other 要比较的另一个点。
         * @return 若两点 x/y 坐标一致则返回 true。
         */
        public bool Equals(HexPoint other) => X.Equals(other.X) && Y.Equals(other.Y);

        /**
         * 判断对象是否与当前二维点等值。
         * @param obj 待比较对象。
         * @return 若对象为 HexPoint 且坐标相同则返回 true。
         */
        public override bool Equals(object obj) => obj is HexPoint other && Equals(other);

        /**
         * 生成当前点的哈希值，用于哈希集合、字典等场景。
         * @return 哈希值。
         */
        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        /**
         * 将点转换为易读的字符串，例如 (1.5, -2.0)。
         * @return 点的字符串表示。
         */
        public override string ToString() => $"({X}, {Y})";

        /**
         * 判断两个点在坐标上是否相等。
         * @param left 左侧点。
         * @param right 右侧点。
         * @return 若两点相等则返回 true。
         */
        public static bool operator ==(HexPoint left, HexPoint right) => left.Equals(right);

        /**
         * 判断两个点在坐标上是否不相等。
         * @param left 左侧点。
         * @param right 右侧点。
         * @return 若两点不等则返回 true。
         */
        public static bool operator !=(HexPoint left, HexPoint right) => !left.Equals(right);
    }
}