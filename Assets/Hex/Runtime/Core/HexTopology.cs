using System;
using System.Collections.Generic;

namespace Sheedon.Hex
{
    /**
     * 提供六边形拓扑关系的核心操作，包括邻居、距离、范围和环状遍历等逻辑。
     */
    public static class HexTopology
    {
        private static readonly HexCoord[] DirectionOffsets =
        {
            new HexCoord(1, -1), // NE
            new HexCoord(1, 0), // E
            new HexCoord(0, 1), // SE
            new HexCoord(-1, 1), // SW
            new HexCoord(-1, 0), // W
            new HexCoord(0, -1) // NW
        };

        private static readonly HexDirection[] RingWalkDirections =
        {
            HexDirection.E,
            HexDirection.NE,
            HexDirection.NW,
            HexDirection.W,
            HexDirection.SW,
            HexDirection.SE
        };

        /**
         * 获取某个六边形在指定方向上的相邻格子坐标。
         * @param coord 当前格子坐标。
         * @param direction 要前往的方向。
         * @return 相邻格子的坐标。
         */
        public static HexCoord GetNeighbor(HexCoord coord, HexDirection direction) =>
            coord + DirectionOffsets[direction.ToIndex()];

        /**
         * 获取当前格子周围所有六个相邻格子，按稳定方向顺序返回。
         * @param coord 当前格子坐标。
         * @return 六个相邻格子的枚举集合。
         */
        public static IEnumerable<HexCoord> GetNeighbors(HexCoord coord)
        {
            for (var i = 0; i < DirectionOffsets.Length; i++)
            {
                yield return coord + DirectionOffsets[i];
            }
        }

        /**
         * 计算两个六边形坐标之间的最短步数距离。
         * @param a 起始格子。
         * @param b 目标格子。
         * @return 两点之间的最短距离。
         */
        public static int Distance(HexCoord a, HexCoord b)
        {
            var deltaQ = (long)a.Q - b.Q;
            var deltaR = (long)a.R - b.R;
            var deltaS = -deltaQ - deltaR;
            var distance = (Math.Abs(deltaQ) + Math.Abs(deltaR) + Math.Abs(deltaS)) / 2L;

            if (distance > int.MaxValue)
            {
                throw new OverflowException("The distance is greater than Int32.MaxValue.");
            }

            return (int)distance;
        }

        /**
         * 获取以中心格子为圆心、半径为 radius 的所有格子集合。
         * @param center 中心格子。
         * @param radius 要覆盖的扩散半径。
         * @return 半径范围内的所有格子枚举。
         */
        public static IEnumerable<HexCoord> GetRange(HexCoord center, int radius)
        {
            ValidateRadius(radius);

            for (var deltaQ = -(long)radius; deltaQ <= radius; deltaQ++)
            {
                var minimumR = Math.Max(-(long)radius, -deltaQ - radius);
                var maximumR = Math.Min(radius, -deltaQ + radius);

                for (var deltaR = minimumR; deltaR <= maximumR; deltaR++)
                {
                    yield return center + new HexCoord(checked((int)deltaQ), checked((int)deltaR));
                }
            }
        }

        /**
         * 获取以中心格子为中心、距离为 radius 的环状边界格子。
         * @param center 中心格子。
         * @param radius 环的半径。
         * @return 该半径上的所有环边界格子。
         */
        public static IEnumerable<HexCoord> GetRing(HexCoord center, int radius)
        {
            ValidateRadius(radius);

            if (radius == 0)
            {
                yield return center;
                yield break;
            }

            var current = AddScaled(center, DirectionOffsets[(int)HexDirection.SW], radius);
            foreach (var direction in RingWalkDirections)
            {
                var offset = DirectionOffsets[(int)direction];
                for (var step = 0; step < radius; step++)
                {
                    yield return current;
                    current += offset;
                }
            }
        }

        /**
         * 按照中心格子开始，依次遍历离中心距离递增的螺旋状格子序列。
         * @param center 中心格子。
         * @param radius 最大环半径。
         * @return 按螺旋顺序排列的格子集合。
         */
        public static IEnumerable<HexCoord> GetSpiral(HexCoord center, int radius)
        {
            ValidateRadius(radius);

            for (var ringRadius = 0;; ringRadius++)
            {
                foreach (var coord in GetRing(center, ringRadius))
                {
                    yield return coord;
                }

                if (ringRadius == radius)
                {
                    yield break;
                }
            }
        }

        /**
         * 将一个偏移量按缩放系数应用到坐标上，便于环和螺旋计算。
         * @param coord 基准坐标。
         * @param offset 方向偏移量。
         * @param scale 缩放倍数。
         * @return 扩展后的坐标。
         */
        private static HexCoord AddScaled(HexCoord coord, HexCoord offset, int scale) =>
            new HexCoord(
                checked(coord.Q + (offset.Q * scale)),
                checked(coord.R + (offset.R * scale)));

        /**
         * 校验半径参数不得为负数。
         * @param radius 待校验半径。
         */
        private static void ValidateRadius(int radius)
        {
            if (radius < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(radius), radius, "Radius cannot be negative.");
            }
        }
    }
}