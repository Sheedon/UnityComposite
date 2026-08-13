using System;
using System.Collections.Generic;

namespace Sheedon.Hex
{
    /**
     * 提供将连续轴向坐标转换为离散六边形格子坐标的几何运算方法。
     */
    public static class HexGeometry
    {
        /**
         * 将浮点轴向坐标四舍五入到最接近的六边形格子坐标。
         * @param q 轴向坐标 q 的浮点值。
         * @param r 轴向坐标 r 的浮点值。
         * @return 最近的离散六边形坐标。
         */
        public static HexCoord Round(double q, double r)
        {
            EnsureFinite(q, nameof(q));
            EnsureFinite(r, nameof(r));

            var s = -q - r;
            var roundedQ = Math.Round(q);
            var roundedR = Math.Round(r);
            var roundedS = Math.Round(s);

            var qDifference = Math.Abs(roundedQ - q);
            var rDifference = Math.Abs(roundedR - r);
            var sDifference = Math.Abs(roundedS - s);

            if (qDifference > rDifference && qDifference > sDifference)
            {
                roundedQ = -roundedR - roundedS;
            }
            else if (rDifference > sDifference)
            {
                roundedR = -roundedQ - roundedS;
            }

            return new HexCoord(ToInt32(roundedQ), ToInt32(roundedR));
        }

        /**
         * 获取从起点到终点之间的直线型六边形路径，包含起点和终点。
         * @param from 起点坐标。
         * @param to 终点坐标。
         * @return 从起点到终点的所有格子坐标。
         */
        public static IReadOnlyList<HexCoord> GetLine(HexCoord from, HexCoord to)
        {
            var stepCount = HexTopology.Distance(from, to);
            var line = new HexCoord[checked(stepCount + 1)];

            if (stepCount == 0)
            {
                line[0] = from;
                return line;
            }

            for (var step = 0; step <= stepCount; step++)
            {
                var t = step / (double)stepCount;
                line[step] = Round(Lerp(from.Q, to.Q, t), Lerp(from.R, to.R, t));
            }

            return line;
        }

        /**
         * 在两个浮点值之间按比例插值，常用于线段上的坐标采样。
         * @param from 起始值。
         * @param to 目标值。
         * @param t 插值参数，范围 0 到 1。
         * @return 插值后的值。
         */
        private static double Lerp(double from, double to, double t) =>
            from + ((to - from) * t);

        /**
         * 将浮点值转换为 Int32，并在溢出时抛出异常。
         * @param value 要转换的浮点值。
         * @return 对应的 Int32 值。
         */
        private static int ToInt32(double value)
        {
            if (value < int.MinValue || value > int.MaxValue)
            {
                throw new OverflowException("The rounded coordinate is outside the Int32 range.");
            }

            return (int)value;
        }

        /**
         * 校验坐标值必须为有限数值，避免 NaN 或无穷大带来的错误结果。
         * @param value 待校验的值。
         * @param parameterName 参数名。
         */
        private static void EnsureFinite(double value, string parameterName)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                throw new ArgumentOutOfRangeException(parameterName, value, "Coordinate values must be finite.");
            }
        }
    }
}