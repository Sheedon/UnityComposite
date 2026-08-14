using System;
using System.Collections.Generic;

namespace Sheedon.Hex
{
    /**
     * 提供常用有限地图形状的构建方法，所有方法最终都返回普通 HexRegion。
     */
    public static class HexShapes
    {
        private const double CircleTolerance = 1e-12;

        /**
         * 使用 Hex Distance 创建大六边形区域。
         * @param center 区域中心坐标。
         * @param radius 六边形半径，0 表示只包含中心格。
         * @return 创建完成的区域。
         */
        public static HexRegion CreateHexagon(HexCoord center, int radius) =>
            new HexRegion(HexTopology.GetRange(center, radius));

        /**
         * 使用 Flat Top odd-q 偏移行列创建矩形区域，origin 表示左上角格子。
         * @param origin 矩形左上角的六边形坐标。
         * @param width 矩形列数，必须大于 0。
         * @param height 矩形行数，必须大于 0。
         * @return 创建完成的区域。
         */
        public static HexRegion CreateRectangle(HexCoord origin, int width, int height)
        {
            ValidatePositive(width, nameof(width));
            ValidatePositive(height, nameof(height));
            return new HexRegion(EnumerateRectangle(origin, width, height));
        }

        /**
         * 使用 Flat Top odd-q 偏移行列创建等宽等高的正方形区域。
         * @param origin 正方形左上角的六边形坐标。
         * @param size 正方形行数与列数，必须大于 0。
         * @return 创建完成的区域。
         */
        public static HexRegion CreateSquare(HexCoord origin, int size)
        {
            ValidatePositive(size, nameof(size));
            return new HexRegion(EnumerateRectangle(origin, size, size));
        }

        /**
         * 按 Flat Top 布局中格子中心的真实二维距离创建圆形区域。
         * @param center 圆心所在的六边形坐标。
         * @param radius 二维布局单位中的圆半径，必须为有限非负数。
         * @param layout 用于计算格子中心位置的布局。
         * @return 创建完成的区域。
         */
        public static HexRegion CreateCircle(HexCoord center, double radius, HexLayout layout)
        {
            if (layout == null)
            {
                throw new ArgumentNullException(nameof(layout));
            }

            if (double.IsNaN(radius) || double.IsInfinity(radius) || radius < 0d)
            {
                throw new ArgumentOutOfRangeException(nameof(radius), radius, "Radius must be finite and cannot be negative.");
            }

            if (radius == 0d)
            {
                return new HexRegion(new[] { center });
            }

            var candidateRadiusValue = Math.Ceiling(radius / (1.5d * layout.HexSize));
            if (double.IsInfinity(candidateRadiusValue) || candidateRadiusValue > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(radius), radius, "Radius is too large for Int32 hex coordinates.");
            }

            return new HexRegion(EnumerateCircle(center, radius, layout, (int)candidateRadiusValue));
        }

        /**
         * 使用闭区间 Q/R 边界创建轴向坐标平行四边形。
         * @param minimumQ 最小 Q 坐标。
         * @param maximumQ 最大 Q 坐标。
         * @param minimumR 最小 R 坐标。
         * @param maximumR 最大 R 坐标。
         * @return 创建完成的区域。
         */
        public static HexRegion CreateParallelogram(
            int minimumQ,
            int maximumQ,
            int minimumR,
            int maximumR)
        {
            if (minimumQ > maximumQ)
            {
                throw new ArgumentException("Minimum Q cannot be greater than maximum Q.", nameof(minimumQ));
            }

            if (minimumR > maximumR)
            {
                throw new ArgumentException("Minimum R cannot be greater than maximum R.", nameof(minimumR));
            }

            return new HexRegion(EnumerateParallelogram(minimumQ, maximumQ, minimumR, maximumR));
        }

        /**
         * 从任意坐标集合创建不规则区域，重复坐标会自动合并。
         * @param coordinates 要加入区域的坐标集合。
         * @return 创建完成的区域。
         */
        public static HexRegion CreateFromCoordinates(IEnumerable<HexCoord> coordinates) =>
            new HexRegion(coordinates);

        private static IEnumerable<HexCoord> EnumerateRectangle(HexCoord origin, int width, int height)
        {
            var originColumn = origin.Q;
            var originRow = ToOddQRow(origin);

            for (long columnOffset = 0; columnOffset < width; columnOffset++)
            {
                var column = checked((int)(originColumn + columnOffset));
                for (long rowOffset = 0; rowOffset < height; rowOffset++)
                {
                    var row = checked((int)(originRow + rowOffset));
                    yield return FromOddQ(column, row);
                }
            }
        }

        private static IEnumerable<HexCoord> EnumerateCircle(
            HexCoord center,
            double radius,
            HexLayout layout,
            int candidateRadius)
        {
            var centerPoint = layout.HexToPoint(center);
            EnsureFinite(centerPoint.X, nameof(center));
            EnsureFinite(centerPoint.Y, nameof(center));

            foreach (var coord in HexTopology.GetRange(center, candidateRadius))
            {
                var point = layout.HexToPoint(coord);
                var normalizedX = (point.X - centerPoint.X) / radius;
                var normalizedY = (point.Y - centerPoint.Y) / radius;
                var normalizedDistanceSquared = (normalizedX * normalizedX) + (normalizedY * normalizedY);

                if (normalizedDistanceSquared <= 1d + CircleTolerance)
                {
                    yield return coord;
                }
            }
        }

        private static IEnumerable<HexCoord> EnumerateParallelogram(
            int minimumQ,
            int maximumQ,
            int minimumR,
            int maximumR)
        {
            for (var q = (long)minimumQ; q <= maximumQ; q++)
            {
                for (var r = (long)minimumR; r <= maximumR; r++)
                {
                    yield return new HexCoord((int)q, (int)r);
                }
            }
        }

        private static int ToOddQRow(HexCoord coord) =>
            checked(coord.R + ((coord.Q - (coord.Q & 1)) / 2));

        private static HexCoord FromOddQ(int column, int row) =>
            new HexCoord(column, checked(row - ((column - (column & 1)) / 2)));

        private static void ValidatePositive(int value, string parameterName)
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(parameterName, value, "Value must be greater than zero.");
            }
        }

        private static void EnsureFinite(double value, string parameterName)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                throw new ArgumentOutOfRangeException(parameterName, "Coordinate produces a non-finite layout point.");
            }
        }
    }
}
