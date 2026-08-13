namespace Sheedon.Hex
{
    /**
     * 表示无限六边形网格中的轴向坐标，使用 q/r 两个整数来描述一个单元格的位置。
     */
    public readonly struct HexCoord : System.IEquatable<HexCoord>
    {
        /**
         * 原点坐标，表示网格中心位置。
         */
        public static readonly HexCoord Zero = new HexCoord(0, 0);

        /**
         * 使用 q 与 r 初始化一个轴向坐标。
         * @param q 横向轴向坐标。
         * @param r 纵向轴向坐标。
         */
        public HexCoord(int q, int r)
        {
            Q = q;
            R = r;
        }

        /**
         * 轴向坐标的 q 分量。
         */
        public int Q { get; }

        /**
         * 轴向坐标的 r 分量。
         */
        public int R { get; }

        /**
         * 计算立方坐标中的 s 分量，满足 q + r + s = 0。
         */
        public int S => checked(-Q - R);

        /**
         * 判断当前坐标与另一个坐标是否完全相同。
         * @param other 要比较的另一个六边形坐标。
         * @return 如果 q 与 r 都相等则返回 true。
         */
        public bool Equals(HexCoord other) => Q == other.Q && R == other.R;

        /**
         * 判断对象是否表示相同的六边形坐标。
         * @param obj 要比较的对象。
         * @return 如果对象是同一类型且坐标相等则返回 true。
         */
        public override bool Equals(object obj) => obj is HexCoord other && Equals(other);

        /**
         * 生成当前坐标的哈希值，用于集合和字典中比较。
         * @return 哈希值。
         */
        public override int GetHashCode()
        {
            unchecked
            {
                return (Q * 397) ^ R;
            }
        }

        /**
         * 将坐标转换为易读的字符串表示，例如 (1, -2)。
         * @return 坐标的字符串描述。
         */
        public override string ToString() => $"({Q}, {R})";

        /**
         * 对两个六边形坐标执行加法运算，得到相邻格子的偏移结果。
         * @param left 左侧坐标。
         * @param right 右侧坐标。
         * @return 相加后的坐标。
         */
        public static HexCoord operator +(HexCoord left, HexCoord right) =>
            new HexCoord(checked(left.Q + right.Q), checked(left.R + right.R));

        /**
         * 对两个六边形坐标执行减法运算，得到两点之间的差值坐标。
         * @param left 左侧坐标。
         * @param right 右侧坐标。
         * @return 差值后的坐标。
         */
        public static HexCoord operator -(HexCoord left, HexCoord right) =>
            new HexCoord(checked(left.Q - right.Q), checked(left.R - right.R));

        /**
         * 对坐标取反，返回其关于原点的镜像位置。
         * @param coord 需要取反的坐标。
         * @return 取反后的坐标。
         */
        public static HexCoord operator -(HexCoord coord) =>
            new HexCoord(checked(-coord.Q), checked(-coord.R));

        /**
         * 判断两个坐标是否相等。
         * @param left 左侧坐标。
         * @param right 右侧坐标。
         * @return 如果两坐标相等则返回 true。
         */
        public static bool operator ==(HexCoord left, HexCoord right) => left.Equals(right);

        /**
         * 判断两个坐标是否不相等。
         * @param left 左侧坐标。
         * @param right 右侧坐标。
         * @return 如果两坐标不相等则返回 true。
         */
        public static bool operator !=(HexCoord left, HexCoord right) => !left.Equals(right);
    }
}